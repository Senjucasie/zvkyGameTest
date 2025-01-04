using Spin.Api;
using System.Collections;
using UnityEngine;

namespace FSM.GamePlay.State
{
    public class ScatterState : GamePlayBaseState
    {
        public ScatterState(GamePlayStateMachine gameplaystatemachine) : base(gameplaystatemachine) 
        { Name = StateName.Scatter; }

        private int _remainingFreeSpin;
        private double _currntFreeSpinAmount;

        private Coroutine _spinCoroutine;
        private ReelManager _reelManager;
        private PaylineController _paylineController;
        private Coroutine _showReelRoutine;
        public override void Enter()
        {
            SlotGameEngineStarter.CurrentState = Name;
            _reelManager = _gamePlayStateMachine.ReelManager;
            _paylineController = _gamePlayStateMachine.PaylineController;
            EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.scatterData);
            EventManager.InvokeScatterStateStartedEvent();
            SubscribeEvents();
            InitializeGamestate();
            _gamePlayStateMachine.StartCoroutine(StartFreeSpinWithDelay(3f));
        }
        private void SubscribeEvents()
        {
            EventManager.FreeSpinGameEndEvent += ScatterGameEnd;
            EventManager.SpinResponseEvent += OnSpinDataFetched;
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
            StopReelsImmediately();
        }
        private void InitializeGamestate()
        {
            _remainingFreeSpin = GameApiManager.Instance.GetFreeSpinCount();
        }

        private IEnumerator StartFreeSpinWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartFreeSpin();
        }

        private void StartFreeSpin()
        {
            SlotGameEngineStarter.IsSlamStop = false;
            _gamePlayStateMachine.StopAllCoroutines();
            _reelManager._currentSpinState = SpinState.Idle;

            //change this to the playline controller rightnow using the enum from the controller
            _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;
            _paylineController.WinTint.SetActive(false);
            _paylineController.ResetPayLine();
            _paylineController.StopWinAnimation();
            Audiomanager.Instance._sfxAudioSource.Stop();
            EventManager.InvokeOnClickResetData();
            _gamePlayStateMachine.StartCoroutine(SpinCoroutine());
        }

        private IEnumerator SpinCoroutine()
        {
            _remainingFreeSpin = UpdateFreeSpinCount();
            EventManager.InvokeFreeSpinPlayed(_remainingFreeSpin);
            
            _spinCoroutine = _gamePlayStateMachine.StartCoroutine(_reelManager.InitiateSpinCoroutine(this.Name));
            yield return _spinCoroutine;

        }
        private int UpdateFreeSpinCount() => --_remainingFreeSpin;
        
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

        private IEnumerator ShowReels(float delay =0)
        {
            yield return new WaitForSeconds(delay);
            _showReelRoutine = null;
            _reelManager.SetOutcomeSymbols();
            _gamePlayStateMachine.StartCoroutine(StopReels(_reelManager.ReelRotationOffset));
        }

        private IEnumerator StopReels(float reelstopldelay)
        {
            Audiomanager.Instance.StopSfx(SFX.ReelSpin);
            int reelstopcount = 0;

            foreach (Reel reel in _reelManager.Reels)
            {
                yield return _gamePlayStateMachine.StartCoroutine(
                                    _reelManager.StopReelsWithoutSpecialSymbol(
                                    reelstopldelay));

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
            if (reelstopcount == _reelManager.ReelSetting.Columns - 1)
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
            GameApiManager.Instance.SendSpinCompleteRequest();

            bool normalpayline = HasNormalPayLine();
            if (normalpayline)
            {
                _gamePlayStateMachine.StartCoroutine(ShowWinCoroutine(normalpayline));
            }
               
            else
            {
                if (_remainingFreeSpin > 0)
                {
                    yield return new WaitForSeconds(0.3f);
                    StartFreeSpin();
                }
                else
                {
                    GameController.DeactivateFreeGame();
                }
            }
        }
        private bool HasNormalPayLine()
        {
            Payline[] paylinesData = GameApiManager.Instance.ApiData.GetPaylineData();
            int paylineCount = paylinesData.Length;
            Debug.Log($"Payline Count: {paylineCount}");
            
            if (paylineCount > 0)
            {
                _paylineController.GeneratePayline(PaylineType.Normal, paylinesData, paylineCount);
                SetTotalAmount(GameApiManager.Instance.ApiData.GetFreeSpinCreditsWon());
                EventManager.InvokeCheckWinCelebration(_currntFreeSpinAmount);
                return true;
            }
            else
                return false;
        }


        private void SetTotalAmount(double amount) => _currntFreeSpinAmount = amount;

        private IEnumerator ShowWinCoroutine(bool normalPayline)
        {
            _paylineController.WinTint.SetActive(true);
            _paylineController.CurrentPayLineState = PaylineController.PayLineState.FirstIteration;

            if (normalPayline)
            {
                _paylineController.ShowTotalWinAmountVisuals(_currntFreeSpinAmount);
                yield return _gamePlayStateMachine.StartCoroutine(_paylineController.ShowNormalPayline());
                yield return _gamePlayStateMachine.StartCoroutine(CelebrationManager.Instance.ShowCelebrationPopupAndWait(_currntFreeSpinAmount));
            }

            _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;
            _paylineController.WinTint.SetActive(false);

            if (_remainingFreeSpin>0)
            {
                yield return new WaitForSeconds(0.3f);
                StartFreeSpin();
            }
            else
            {
                GameController.DeactivateFreeGame();
            }
        }

        private void ScatterGameEnd()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.NormalGameState);
        }

        public override void Exit()
        {
            EventManager.InvokeResetWinData();
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            EventManager.SpinResponseEvent -= OnSpinDataFetched;
            EventManager.FreeSpinGameEndEvent -= ScatterGameEnd;
            BottomUIPanel.SlamStopReelsEvent -= OnSlamStop;
        }

    }
}

