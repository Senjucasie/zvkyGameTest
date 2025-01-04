using Spin.Api;
using System.Collections;
using UnityEngine;
using static PaylineController;

namespace FSM.GamePlay.State
{
    public class NormalState : GamePlayBaseState
    {
        public NormalState(GamePlayStateMachine gameplaystatemachine) : base(gameplaystatemachine) { Name = StateName.Normal; }

        private bool _showPaylineInLoop;
        private double _currentSpinCreditWon;
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
            EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.normalData);
            EventManager.InvokeNormalStateStartedEvent();
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            EventManager.SpinResponseEvent += OnSpinDataFetched;
            EventManager.SpinButtonClickedEvent += OnSpinClick;
            EventManager.OnFreeSpinIntroEnded += SwitchToScatterState;
            EventManager.BonusStateEnterEvent += SwitchToBonusState;
            BottomUIPanel.SlamStopReelsEvent += OnSlamStop;
        }

        private void SwitchToBonusState()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.BonusGameState);
        }
        private void SwitchToScatterState()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.ScatterGameState);
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

        private void OnSpinClick()
        {
            EventManager.InvokeSetButtonForResume();
            _reelManager.IsWaitingForIndependentSpecialSymbol = false;
            SlotGameEngineStarter.IsSlamStop = false;
            _gamePlayStateMachine.StopAllCoroutines();
            _reelManager._currentSpinState = SpinState.Idle;
            _reelManager.ClearExpandingWild();

            //change this to the playline controller rightnow using the enum from the controller
            _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;
            _paylineController.WinTint.SetActive(false);
            _showPaylineInLoop = _reelManager.SystemSetting.ShowPaylinesInLoop;
            _paylineController.ResetPayLine();
            _paylineController.StopWinAnimation();
            Audiomanager.Instance._sfxAudioSource.Stop();
            EventManager.InvokeOnClickResetData();
            _gamePlayStateMachine.StartCoroutine(SpinCoroutine());
        }
        private IEnumerator SpinCoroutine()
        {
            _spinCoroutine = _gamePlayStateMachine.StartCoroutine(_reelManager.InitiateSpinCoroutine(this.Name));
            yield return _spinCoroutine;
        }

        private void OnSpinDataFetched()
        {
            if(SlotGameEngineStarter.IsTurboCached)
            {
                OnSlamStop();
                SlotGameEngineStarter.IsTurboCached = false;
                return;
            }

            float normaldelay = _reelManager.SpinStopDelay + (_reelManager.ReelRotationOffset * _reelManager.ReelSetting.Columns);
            float turbodelay = _reelManager.ReelSetting.TurboSpinDelay;
            _showReelRoutine = _gamePlayStateMachine.StartCoroutine(SlotGameEngineStarter.TurboEnabled ? ShowReels(turbodelay) : ShowReels(normaldelay));
        }

        private IEnumerator ShowReels(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            _showReelRoutine = null;

            // PRE EXPANDED WILD
            if (GameFeaturesManager.Instance.HasPreExpandedWild)
                _reelManager.SetPreExpandedWild();

            // EXPANDING WILD
            if (GameFeaturesManager.Instance.HasExpandingWild)
                _reelManager.SetExpandingWild();

            _reelManager.SetReelsForAnticipation();
            _reelManager.SetOutcomeSymbols();
            _gamePlayStateMachine.StartCoroutine(StopReels(_reelManager.ReelRotationOffset, _reelManager.SystemSetting.AnticipationDuration));
        }

        private IEnumerator StopReels(float reelstopldelay = 0, float scatteranticipationdelay = 0)
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

                if (reel.isSpinning) // TODO: Have to add a scenario to check this condition as we need reels to spin for handling the resume request
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
                EventManager.InvokeOnSpinComplete();
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

            if (normalpayline || featurePayline || scatterpayline || bonuspayline)
                _gamePlayStateMachine.StartCoroutine(ShowWinCoroutine(normalpayline, scatterpayline, bonuspayline, featurePayline));

            GameApiManager.Instance.SendSpinCompleteRequest();

            yield return null;
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

        private void SetTotalAmount(double amount) => _currentSpinCreditWon = amount;

        private bool HasScatterPayline()
        {
            Scatter scatterData = GameApiManager.Instance.ApiData.GetScatterData();
            if (scatterData.count <= minScatterForFreeGame) return false;

            _paylineController._scatterPayline = new(scatterData.position);
            //EventManager.InvokeFreeSpinGameStart();
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

        private IEnumerator ShowWinCoroutine(bool normalpayline, bool scatterpayline, bool bonuspayline, bool featurepayline)
        {
            bool hasShownPaylineOnce = false;

            _paylineController.WinTint.SetActive(true);
            if (_paylineController.CurrentPayLineState == PaylineController.PayLineState.NotShowing)
            {
                _paylineController.CurrentPayLineState = PaylineController.PayLineState.FirstIteration;
                EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.stateSwitchingData);
            }
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
            if (featurepayline)
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
                _showPaylineInLoop = false;
                _reelManager.ClearExpandingWild();
                yield return _gamePlayStateMachine.
                    StartCoroutine(_paylineController.ShowScatterPayline());
            }
            if (bonuspayline)
            {
                _showPaylineInLoop = false;
                _reelManager.ClearExpandingWild();
                yield return _gamePlayStateMachine.
                    StartCoroutine(_paylineController.ShowBonusPayline());
            }
            while (_showPaylineInLoop)
            {
                _paylineController.CurrentPayLineState = PaylineController.PayLineState.Looping;
                EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.normalData);

                if (featurepayline)
                    yield return _gamePlayStateMachine.
                        StartCoroutine(_paylineController.ShowFeaturePayline());
                else
                    yield return _gamePlayStateMachine.
                        StartCoroutine(_paylineController.ShowNormalPayline());

            }

            _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;
            // EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.normalData);
            _paylineController.WinTint.SetActive(false);

        }

        public override void Exit()
        {
            SlotGameEngineStarter.IsSlamStop = false;
            _reelManager.IsWaitingForIndependentSpecialSymbol = false;
            _reelManager._currentSpinState = SpinState.Idle;
            EventManager.InvokeResetWinData();
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            EventManager.SpinResponseEvent -= OnSpinDataFetched;
            EventManager.SpinButtonClickedEvent -= OnSpinClick;
            EventManager.OnFreeSpinIntroEnded -= SwitchToScatterState;
            EventManager.BonusStateEnterEvent -= SwitchToBonusState;
            BottomUIPanel.SlamStopReelsEvent -= OnSlamStop;
        }
    }
}
