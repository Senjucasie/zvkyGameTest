using Spin.Api;
using System.Collections;
using UnityEngine;


public class ReSpinDriver : MonoBehaviour
{
    GamePlayStateMachine _gamePlayStateMachine;
    private int _remainingReSpin;
    private double _currentReSpinAmount;

    private ReelManager _reelManager;
    private PaylineController _paylineController;
    private Coroutine _showReelRoutine;
    private Coroutine _spinCoroutine;
    public void Enter(GamePlayStateMachine gamePlayStateMachine)
    {
        _gamePlayStateMachine = gamePlayStateMachine;
        _reelManager = _gamePlayStateMachine.ReelManager;
        _paylineController = _gamePlayStateMachine.PaylineController;
        EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.reSpinBonusData);
        SubscribeEvents();
        InitializeGamestate();
        StartCoroutine(StartReSpinWithDelay(3f));
    }
    private void SubscribeEvents()
    {
        EventManager.SpinResponseEvent += OnSpinDataFetched;
        BottomUIPanel.SlamStopReelsEvent += OnSlamStop;

    }
    private void OnSlamStop()
    {
        _gamePlayStateMachine.StopCoroutine(_spinCoroutine);
        SlotGameEngineStarter.IsSlamStop = true;
        if (_showReelRoutine != null)
        {
            StopCoroutine(_showReelRoutine);
            StartCoroutine(ShowReels());
        }

        StopReelsImmediately();
    }
    private void InitializeGamestate()
    {
        _remainingReSpin = GameApiManager.Instance.GetReSpinCount();
    }

    private IEnumerator StartReSpinWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartReSpin();
    }

    private void StartReSpin()
    {
        SlotGameEngineStarter.IsSlamStop = false;
        StopAllCoroutines();
        _reelManager._currentSpinState = SpinState.Idle;

        _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;
        _paylineController.WinTint.SetActive(false);
        _paylineController.ResetPayLine();
        _paylineController.StopWinAnimation();
        Audiomanager.Instance._sfxAudioSource.Stop();
        EventManager.InvokeOnClickResetData();
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        _spinCoroutine = _gamePlayStateMachine
                            .StartCoroutine(_reelManager
                                .InitiateSpinCoroutine(SlotGameEngineStarter.CurrentState));
        yield return _spinCoroutine;

        _remainingReSpin = UpdateReSpinCount();
        EventManager.InvokeReSpinPlayed(_remainingReSpin);
    }

    private int UpdateReSpinCount() => --_remainingReSpin;

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
        _showReelRoutine = StartCoroutine(SlotGameEngineStarter.TurboEnabled ? ShowReels(turbodelay) : ShowReels(normaldelay));
    }
    private IEnumerator ShowReels(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        _showReelRoutine = null;
        if (GameFeaturesManager.Instance.HasPreExpandedWild)
            _reelManager.SetPreExpandedWild();
        _reelManager.SetOutcomeSymbols();
        StartCoroutine(StopReels(_reelManager.ReelRotationOffset));
    }

    private IEnumerator StopReels(float reelstopldelay)
    {
        Audiomanager.Instance.StopSfx(SFX.ReelSpin);
        int reelstopcount = 0;

        foreach (Reel reel in _reelManager.Reels)
        {
            yield return StartCoroutine(
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
        if (reelstopcount == _reelManager.ReelSetting.Columns)
        {
            _reelManager.OnAllReelStop(CheckPaylines);
        }
    }

    private IEnumerator CheckPaylines()
    {
        GameApiManager.Instance.SendSpinCompleteRequest();

        if (HasNormalPayLine())
            StartCoroutine(ShowWinCoroutine());
        else
        {
            if (_remainingReSpin > 0)
            {
                yield return new WaitForSeconds(0.3f);
                StartReSpin();
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
            SetTotalAmount(GameApiManager.Instance.ApiData.GetReSpinCreditsWon());
            EventManager.InvokeCheckWinCelebration(_currentReSpinAmount);
            return true;
        }
        else
            return false;
    }
    private void SetTotalAmount(double amount) => _currentReSpinAmount = amount;

    private IEnumerator ShowWinCoroutine()
    {
        if (GameFeaturesManager.Instance.HasPreExpandedWild)
            _paylineController.PlayPreExpandedWild();

        _paylineController.CurrentPayLineState = PaylineController.PayLineState.FirstIteration;
        _paylineController.ShowTotalWinAmountVisuals(_currentReSpinAmount);
        _paylineController.WinTint.SetActive(true);
        yield return StartCoroutine(_paylineController.ShowNormalPayline());
        yield return StartCoroutine(CelebrationManager.Instance.ShowCelebrationPopupAndWait(_currentReSpinAmount));
        _paylineController.CurrentPayLineState = PaylineController.PayLineState.NotShowing;

        if (GameFeaturesManager.Instance.HasPreExpandedWild)
            _paylineController.StopPreExpandedWild();

        _paylineController.WinTint.SetActive(false);

        if (_remainingReSpin > 0)
        {
            yield return new WaitForSeconds(0.3f);
            StartReSpin();
        }
        else
        {
            Exit();
        }
    }

    public void Exit()
    {
        UnSubscribeEvents();
        EventManager.InvokeBonusGameEnd();
    }

    private void UnSubscribeEvents()
    {
        EventManager.SpinResponseEvent -= OnSpinDataFetched;
        BottomUIPanel.SlamStopReelsEvent -= OnSlamStop;
    }

}
