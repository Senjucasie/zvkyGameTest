using Spin.Api;
using System.Collections;
using UnityEngine;

namespace FSM.GamePlay.State
{
    public class AutoPlayState : GamePlayBaseState
    {
        public AutoPlayState(GamePlayStateMachine gameplaystatemachine) : base(gameplaystatemachine)
        { Name = StateName.Auto; }

        private double _currentSpinCreditWon;
        private int _currentAutoPlayCount;
        private const int minScatterForFreeGame = 2;

        private Coroutine _showReelRoutine;
        private Coroutine _spinCoroutine;
        private ReelManager _reelManager;
        private PaylineController _paylineController;

        public override void Enter()
        {
            SlotGameEngineStarter.CurrentState = Name;
            _reelManager = _gamePlayStateMachine.ReelManager;
            _paylineController = _gamePlayStateMachine.PaylineController;
            EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.autoData);
            EventManager.InvokeAutoStateStartedEvent();
            _currentAutoPlayCount = 0;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            EventManager.OnAutoSpinPlayEvent += InitAutoSpin;
            EventManager.SpinResponseEvent += OnSpinDataFetched;
            BottomUIPanel.AutoPlayStopButtonEvent += SetAutoPlayCountToZero;
            EventManager.OnFreeSpinIntroEnded += SwitchToScatterState;
            EventManager.BonusStateEnterEvent += SwitchToBonusState;
            BottomUIPanel.SlamStopReelsEvent += OnSlamStop;
        }

        private void OnSlamStop()
        {
            _gamePlayStateMachine.StopCoroutine(_spinCoroutine);
            SlotGameEngineStarter.IsSlamStop = true;
            if (_showReelRoutine != null)
            {
                _gamePlayStateMachine.StopCoroutine(_showReelRoutine);
                _gamePlayStateMachine.StartCoroutine(ShowReels());
            }
            _reelManager.IsWaitingForIndependentSpecialSymbol = false;

            StopReelsImmediately();
        }

        private void SwitchToBonusState()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.BonusGameState);
        }

        private void SwitchToScatterState()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.ScatterGameState);
        }

        private void InitAutoSpin(int count)
        {
            _currentAutoPlayCount = count;
            StartAutoSpin();
        }
        private void StartAutoSpin()
        {
            if (EconomyManager.HasSufficientBalance())
            {
                InitSpin();
                _spinCoroutine = _gamePlayStateMachine.StartCoroutine(SpinCoroutine());
            }
            else
            {
                ErrorHandler.Instance.ShowErrorPopup(ErrorType.LOW_BALANCE);
                TransitionToNormalSate();
            }
        }
        private void InitSpin()
        {
            SlotGameEngineStarter.IsSlamStop = false;
            _gamePlayStateMachine.StopAllCoroutines();
            _reelManager._currentSpinState = SpinState.Idle;
            _reelManager.ClearExpandingWild();

            _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;
            _paylineController.WinTint.SetActive(false);
            _paylineController.ResetPayLine();
            _paylineController.StopWinAnimation();
            Audiomanager.Instance._sfxAudioSource.Stop();
            EventManager.InvokeOnClickResetData();
        }

        private IEnumerator SpinCoroutine()
        {
            UpdateAutoPlayCount();

            if (_currentAutoPlayCount >= 0)
            {
                EconomyManager.OnUpdateCurrentBalance();
                EventManager.InvokeOnAutoSpinPlayed(_currentAutoPlayCount);
            }

            yield return _reelManager.InitiateSpinCoroutine(this.Name);
        }

        private void UpdateAutoPlayCount() => --_currentAutoPlayCount;

        private void SetAutoPlayCountToZero() => _currentAutoPlayCount = 0;

        private void OnSpinDataFetched()
        {
            if (SlotGameEngineStarter.IsTurboCached)
            {
                OnSlamStop();
                SlotGameEngineStarter.IsTurboCached = false;
                return;
            }

            float normaldelay = _reelManager.ReelSetting.SpinStopDelay + (_reelManager.ReelRotationOffset * _reelManager.ReelSetting.Columns);
            float turbodelay = _reelManager.ReelSetting.TurboSpinDelay;
            _showReelRoutine = _gamePlayStateMachine.StartCoroutine(SlotGameEngineStarter.TurboEnabled ? ShowReels(turbodelay) : ShowReels(normaldelay));
        }

        private IEnumerator ShowReels(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            _showReelRoutine = null;
            if (GameFeaturesManager.Instance.HasPreExpandedWild)
                _reelManager.SetPreExpandedWild();
            _reelManager.SetOutcomeSymbols();
            _reelManager.SetReelsForAnticipation();
            _gamePlayStateMachine.StartCoroutine(StopReels(_reelManager.ReelRotationOffset, _reelManager.SystemSetting.AnticipationDuration));
        }

        private IEnumerator StopReels(float reelstopldelay, float scatteranticipationdelay)
        {
            Audiomanager.Instance.StopSfx(SFX.ReelSpin);
            int reelstopcount = 0;

            foreach (Reel reel in _reelManager.Reels)
            {
                if (reel.anticipateReelForSpecialSymbol)
                {
                    yield return _gamePlayStateMachine.StartCoroutine(
                                    _reelManager.StopReelsWithSpecialSymbol(
                                    reel, scatteranticipationdelay));
                }
                else
                {
                    yield return _gamePlayStateMachine.StartCoroutine(
                                    _reelManager.StopReelsWithoutSpecialSymbol(
                                    reelstopldelay));
                }
                if (reel.isSpinning)
                    reel.SpinStop(() => OnReelStop(++reelstopcount));
            }
        }

        private void StopReelsImmediately()
        {
            int reelstopcount = 0;
            foreach (Reel reel in _reelManager.Reels)
            {
                reel.SpinStop(() => OnReelStop(++reelstopcount));
                reel.StopSpinRoutine();
                reel.blurredStrip.StopSpinIllusion();
            }
            Audiomanager.Instance.StopSfx(SFX.ReelSpin);
        }

        private void OnReelStop(int reelstopcount)
        {
            if (reelstopcount == _reelManager.ReelSetting.Columns - 1 && !_reelManager.IsWaitingForIndependentSpecialSymbol)
            {
                EventManager.InvokeDisableSlamStop();
            }
            if (reelstopcount == _reelManager.ReelSetting.Columns)
            {
                _reelManager.OnAllReelStop(CheckPaylines);
            }

        }

        private IEnumerator CheckPaylines()
        {
            bool normalpayline = HasNormalPayLine();

            bool featurePayline = false;
            if (GameFeaturesManager.Instance.HasExpandingWild)
                featurePayline = HasFeaturePayline();

            bool scatterpayline = HasScatterPayline();
            bool bonuspayline = HasBonusPayline();

            GameApiManager.Instance.SendSpinCompleteRequest();

            if (normalpayline || featurePayline || scatterpayline || bonuspayline)
            {
                _gamePlayStateMachine.StartCoroutine(ShowWinCoroutine(normalpayline, featurePayline, scatterpayline, bonuspayline));
            }
            else
            {
                if (_currentAutoPlayCount > 0)
                {
                    yield return new WaitForSeconds(0.3f);
                    StartAutoSpin();
                }
                else
                    TransitionToNormalSate();
            }
        }
        private bool HasNormalPayLine()
        {
            Payline[] paylinesData = GameApiManager.Instance.ApiData.GetPaylineData();
            int paylineCount = paylinesData.Length;

            if (paylineCount > 0)
            {
                _paylineController.GeneratePayline(PaylineType.Normal, paylinesData, paylineCount);
                SetTotalAmount(GameApiManager.Instance.ApiData.GetMainSpinCreditsWon());
                EventManager.InvokeCheckWinCelebration(_currentSpinCreditWon);
                return true;
            }
            else
                return false;
        }

        private bool HasFeaturePayline()
        {
            Payline[] paylineData = GameApiManager.Instance.ApiData.GetFeaturePaylineData();
            if (paylineData == null) return false;

            int paylineCount = paylineData.Length;

            if (paylineCount > 0)
            {
                _paylineController.GeneratePayline(PaylineType.Feature, paylineData, paylineCount);
                SetTotalAmount(GameApiManager.Instance.ApiData.GetMainSpinCreditsWon());
                EventManager.InvokeCheckWinCelebration(_currentSpinCreditWon);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HasScatterPayline()
        {
            Scatter scatterData = GameApiManager.Instance.ApiData.GetScatterData();
            if (scatterData.count <= minScatterForFreeGame) return false;
            _paylineController._scatterPayline = new(scatterData.position);
            return true;
        }

        private bool HasBonusPayline()
        {
            _paylineController._bonusPayline = null;

            Bonus bonusData = GameApiManager.Instance.ApiData.GetBonusData();
            int bonusCount = bonusData.count;
            if (bonusCount > 2 && bonusData.bonusTrigger)
            {
                _paylineController._bonusPayline = new(bonusData.position);
                //EventManager.InvokeBonusSpinGameStart();
                return true;
            }
            return false;
        }
        private void TransitionToNormalSate()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.NormalGameState);
            EventManager.InvokeOnAutoSpinStop();
        }

        private IEnumerator ShowWinCoroutine(bool normalpayline, bool featurePayline, bool scatterpayline, bool bonuspayline)
        {
            bool hasShownPaylineOnce = false;
            _paylineController.WinTint.SetActive(true);
            _paylineController.CurrentPayLineState = PaylineController.PayLineState.FirstIteration;

            if (normalpayline)
            {
                if (GameFeaturesManager.Instance.HasPreExpandedWild)
                    _paylineController.PlayPreExpandedWild();

                hasShownPaylineOnce = true;
                _paylineController.ShowTotalWinAmountVisuals(_currentSpinCreditWon);
                yield return _gamePlayStateMachine.StartCoroutine(_paylineController.ShowNormalPayline());
                yield return CelebrationManager.Instance.ShowCelebrationPopupAndWait(_currentSpinCreditWon);

                if (GameFeaturesManager.Instance.HasPreExpandedWild)
                    _paylineController.StopPreExpandedWild();
            }

            if (featurePayline)
            {
                _reelManager.InstantiateExpandingWild();
                _paylineController.PlayExpandingWild();

                if (!hasShownPaylineOnce)
                    _paylineController.ShowTotalWinAmountVisuals(_currentSpinCreditWon);
                yield return _gamePlayStateMachine.StartCoroutine(_paylineController.ShowFeaturePayline());
                if (!hasShownPaylineOnce)
                    yield return CelebrationManager.Instance.ShowCelebrationPopupAndWait(_currentSpinCreditWon);
            }

            if (scatterpayline)
            {
                yield return _gamePlayStateMachine.
                    StartCoroutine(_paylineController.ShowScatterPayline());
                _paylineController.WinTint.SetActive(false);
                EventManager.InvokeOnAutoSpinStop();
                //EventManager.InvokeFreeSpinGameStart();
            }
            if (bonuspayline)
            {
                yield return _gamePlayStateMachine.
                    StartCoroutine(_paylineController.ShowBonusPayline());
                _paylineController.WinTint.SetActive(false);
                EventManager.InvokeOnAutoSpinStop();
            }

            if (!scatterpayline && !bonuspayline)
            {
                if (_currentAutoPlayCount > 0)
                {
                    yield return new WaitForSeconds(0.3f);
                    StartAutoSpin();
                }
                else
                {
                    TransitionToNormalSate();
                }
            }
        }

        private void SetTotalAmount(double amount) => _currentSpinCreditWon = amount;

        public override void Exit()
        {
            EventManager.InvokeResetWinData();
            SlotGameEngineStarter.IsSlamStop = false;
            _reelManager.IsWaitingForIndependentSpecialSymbol = false;
            _reelManager._currentSpinState = SpinState.Idle;
            _paylineController.WinTint.SetActive(false);
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            EventManager.OnFreeSpinIntroEnded -= SwitchToScatterState;
            EventManager.SpinResponseEvent -= OnSpinDataFetched;
            EventManager.OnAutoSpinPlayEvent -= InitAutoSpin;
            EventManager.BonusStateEnterEvent -= SwitchToBonusState;
            BottomUIPanel.AutoPlayStopButtonEvent -= SetAutoPlayCountToZero;
            BottomUIPanel.SlamStopReelsEvent -= OnSlamStop;
        }
    }

}
