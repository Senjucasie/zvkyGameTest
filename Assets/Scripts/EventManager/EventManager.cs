using System;
using System.Collections.Generic;

public class EventManager
{

    #region ACTIONS
    //GENERAL ACTIONS
    public static Action OnClickResetDataEvent;
    public static Action OnClickResetWinDataEvent;
    public static Action<bool> OnTurboClickedEvent;
    public static Action<double> SetPlayerDataEvent;
    public static Action<List<BaseButtonBehaviour>, bool> SwitchGameButtonStateEvent;
    public static Action SetButtonForResumeEvent;
    public static Action DisableSlamStopEvent;

    // Game Economy Actions
    public static Action<int> UpdateCreditValueIndexEvent;
    public static Action<double> WinAmountEvent;
    public static Action<double> BalanceAmountDeductionEvent;
    public static Action SetInitialBalanceEvent;
    public static Action<double> OnUpdateCurrentBalanceEvent;
    public static Action OnUpdateWheelBonusBalanceEvent;
    public static Action UpdateCreditValueEvent;
    public static Action<int> SetUpdatedResumeBet;

    // Celebration Actions
    public static Action<double> WinCelebration;
    public static Action<double> CheckWinCelebrationEvent;

    // Spin Actions
    public static Action SpinButtonClickedEvent; // TODO: Similar methods and events, needs to be updated.
    public static Action OnSpinClickedEvent; // TODO: Similar methods and events, needs to be updated.
    public static Action RequestSpinDataEvent;
    public static Action ResumeSpinDataEvent;
    public static Action SpinResponseEvent;
    public static Action OnSpinCompleteEvent;

    // Bonus Actions
    public static Action OnBonusPaylineStopped;
    public static Action<Action> BonusGameStartEvent;
    public static Action<GamePlayStateMachine> BonusStartEvent;
    public static Action<Action> BonusViaBonusStateEvent;
    public static Action BonusGameEndEvent;
    public static Action BonusSpinGameEndEvent;
    public static Action BonusStateEnterEvent;

    // Pick Bonus Actions
    public static Action<PickBonusSymbol> PickBonusPrizePickedEvent;

    // Free Spin Actions
    public static Action OnScatterPaylineStopped;
    public static Action OnFreeSpinIntroEnded;
    public static Action FreeSpinGameStartEvent;
    public static Action FreeSpinGameEndEvent;
    public static Action FreeSpinOnLoginEvent;
    public static Action BonusOnLoginEvent;
    public static Action<int> OnFreeSpinPlayed;

    // Re Spin Actions
    public static Action<int> OnReSpinPlayed;


    // Auto Spin Actions
    public static Action<int> OnAutoSpinPlayEvent;
    public static Action<int> OnAutoSpinPlayedEvent;
    public static Action OnAutoSpinStopEvent;

    // State Switching
    public static Action NormalStateStartedEvent;
    public static Action ScatterStateStartedEvent;
    public static Action AutoStateStartedEvent;
    #endregion

    public static void InvokeSwitchBaseButtonState(List<BaseButtonBehaviour> baseButtonBehaviour, bool setbasebuttoninteractivity = true)
    {
        SwitchGameButtonStateEvent?.Invoke(baseButtonBehaviour, setbasebuttoninteractivity);
    }

    public static void InvokeSetButtonForResume()
    {
        SetButtonForResumeEvent?.Invoke();
    }

    public static void InvokeNormalStateStartedEvent() => NormalStateStartedEvent?.Invoke();
    public static void InvokeScatterStateStartedEvent() => ScatterStateStartedEvent?.Invoke();
    public static void InvokeAutoStateStartedEvent() => AutoStateStartedEvent?.Invoke();

    public static void InvokeUpdateCreditValue()
    {
        UpdateCreditValueEvent();
    }

    public static void InvokeUpdateCreditValue(int totalBet)
    {
        SetUpdatedResumeBet(totalBet);
    }

    public static void InvokeUpdateBalance(double amount)
    {
        OnUpdateCurrentBalanceEvent(amount);
    }

    public static void InvokeUpdateWheelBonusBalance()
    {
        OnUpdateWheelBonusBalanceEvent?.Invoke();
    }

    public static void InvokeSetPlayerData(double current_Bet)
    {
        SetPlayerDataEvent(current_Bet);
    }

    public static void InvokeSetInitialBalance()
    {
        SetInitialBalanceEvent();
    }

    public static void InvokeRequestSpinData()
    {
        RequestSpinDataEvent();
    }

    public static void InvokeOnAutoSpinPlay(int count)
    {
        OnAutoSpinPlayEvent(count);
    }


    public static void InvokeOnAutoSpinPlayed(int count)
    {
        OnAutoSpinPlayedEvent?.Invoke(count);
    }

    public static void InvokeOnAutoSpinStop()
    {
        OnAutoSpinStopEvent();
    }

    public static void InvokeDisableSlamStop()
    {
        DisableSlamStopEvent?.Invoke();
    }

    public static void InvokeUpdateCreditValueIndex(int index)
    {
        UpdateCreditValueIndexEvent(index);
    }

    public static void InvokeWinAmount(double win)
    {
        WinAmountEvent(win);
    }

    public static void InvokeUpdateBalanceAmount(double currentBalance)
    {
        BalanceAmountDeductionEvent(currentBalance);
    }

    public static void InvokeCelebration(double args)
    {
        WinCelebration(args);
    }

    public static void InvokeCheckWinCelebration(double winAmount)
    {
        CheckWinCelebrationEvent(winAmount);
    }

    public static void InvokeSpinButton()
    {
        SpinButtonClickedEvent?.Invoke();
    }

    public static void InvokeScatterPaylineStopped()
    {
        OnScatterPaylineStopped();
    }

    public static void InvokeBonusPaylineStopped()
    {
        OnBonusPaylineStopped();
    }
    public static void InvokeOnClickResetData()
    {
        OnClickResetDataEvent();
    }
    public static void InvokeResetWinData()
    {
        OnClickResetWinDataEvent();
    }
    public static void InvokeFreeSpinIntroEnded()
    {
        OnFreeSpinIntroEnded?.Invoke();
    }

    public static void InvokeBonusGameStart(Action callBack)
    {
        BonusGameStartEvent(callBack);
    }

    public static void InvokeBonusGame(GamePlayStateMachine gamePlayStateMachine)
    {
        BonusStartEvent(gamePlayStateMachine);
    }

    public static void InvokeBonusViaBonusState(Action callback)
    {
        BonusViaBonusStateEvent(callback);
    }
    public static void InvokeSpinResponse()
    {
        SpinResponseEvent();
    }

    public static void InvokeFreeSpinOnLogin()
    {
        FreeSpinOnLoginEvent();
    }

    public static void InvokeBonusOnLogin()
    {
        BonusOnLoginEvent();
    }
    public static void InvokeOnSpinClicked()
    {
        OnSpinClickedEvent();
    }

    public static void InvokeOnTurboClicked(bool isturboenabled)
    {
        OnTurboClickedEvent?.Invoke(isturboenabled);
    }

    public static void InvokeOnSpinComplete()
    {
        OnSpinCompleteEvent();
    }
    public static void InvokeBonusGameEnd()
    {
        BonusGameEndEvent();
    }

    public static void InvokeFreeSpinGameEnd()
    {
        FreeSpinGameEndEvent();
    }
    public static void InvokeBonusSpinGameEnd()
    {
        BonusSpinGameEndEvent();
    }
    public static void InvokeFreeSpinGameStart()
    {
        FreeSpinGameStartEvent();
    }
    public static void InvokePickBonusPrizePicked(PickBonusSymbol symbol)
    {
        PickBonusPrizePickedEvent(symbol);
    }

    public static void InvokeFreeSpinPlayed(int currentFreeSpinCount)
    {
        OnFreeSpinPlayed(currentFreeSpinCount);
    }

    public static void InvokeReSpinPlayed(int currentReSpinCount)
    {
        OnReSpinPlayed(currentReSpinCount);
    }

    public static void InvokeBonusStateEvent() => BonusStateEnterEvent?.Invoke();


}


