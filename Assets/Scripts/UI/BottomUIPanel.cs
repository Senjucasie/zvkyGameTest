using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomUIPanel : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private TextMeshProUGUI current_Bet;
    [SerializeField] private TextMeshProUGUI availableBalance;
    [SerializeField] private TextMeshProUGUI winAmount;
    [SerializeField] private TextMeshProUGUI autoPlayCountTxt;

    [SerializeField] private List<GameObject> uiIButtonPopups;
    [SerializeField] private GameObject uiIButtonShow;
    [SerializeField] private GameObject[] autoPlayInfoPopups;
    [SerializeField] private GameObject[] turboPlayInfoPopups;
    [SerializeField] private GameObject autoPlayPopup;
    [SerializeField] private GameObject autoPlayStopBtnVisual;


    [SerializeField] private Button plusBtn;
    [SerializeField] private Button minusBtn;
    [SerializeField] private Button MaxBetBtn;
    [SerializeField] private Button infoBtn;
    [SerializeField] private Button spinButton;
    [SerializeField] private Button slamStopButton;
    [SerializeField] private float slamAnticipationDelay = 0;

    #endregion

    #region Internals
    private int IButtonValue = 0;
    private int _currentBetIndex;
    private static bool _isTurboButtonClicked = false;

    public static event Action SlamStopReelsEvent;
    public GamePlayStateMachine _gamePlayState;
    #endregion


    public static Action AutoPlayStopButtonEvent;

    public static bool IsTurboButtonClicked { get => _isTurboButtonClicked; }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void Awake()
    {
        _currentBetIndex = BetManager.Instance.BetIndex;
    }

    private void SubscribeEvents()
    {
        EventManager.WinAmountEvent += WinAmount;
        EventManager.BalanceAmountDeductionEvent += BalanceDeduction;
        EventManager.OnSpinCompleteEvent += OnSpinComplete;
        EventManager.OnClickResetDataEvent += ResetWinAndDisableSlamStop;
        EventManager.OnClickResetWinDataEvent += ResetWinAmount;

        EventManager.SetInitialBalanceEvent += SetInitialBalance;
        EventManager.OnUpdateCurrentBalanceEvent += UpdateBalanceAmount;
        EventManager.SpinResponseEvent += ShowSlamStopButton;
        EventManager.UpdateCreditValueEvent += UpdateCreditValue;
        EventManager.NormalStateStartedEvent += SetButtonForNormalState;
        EventManager.ScatterStateStartedEvent += SetButtonForScatterState;
      
        EventManager.SwitchGameButtonStateEvent += SetBaseButtonInteractivity;
        EventManager.SetButtonForResumeEvent += SetButtonForResumeSpin;
        EventManager.DisableSlamStopEvent += DisableSlamStopButton;
        EventManager.SetUpdatedResumeBet += ResumeUpdateCreditValue;
    }

    private void SetBaseButtonInteractivity(List<BaseButtonBehaviour> baseButtonBehaviours, bool setbetbuttoninteractivity = true)
    {
        foreach (BaseButtonBehaviour baseButtonBehaviour in baseButtonBehaviours)
        {
            baseButtonBehaviour.button.interactable = baseButtonBehaviour.interactivity;
        }
        if (setbetbuttoninteractivity)
            SetBetButtonInteractivity();
    }

    public void OnSpinButtonClicked()
    {
        if (EconomyManager.HasSufficientBalance())
        {
            ResetWinAndDisableSlamStop();
            EventManager.InvokeOnSpinClicked();
            SetButton(false);
            Audiomanager.Instance.PlayUiSfx(SFX.SpinBtn);
        }
        else
        {
            ErrorHandler.Instance.ShowErrorPopup(ErrorType.LOW_BALANCE);
        }
    }

    private void SetButton(bool enable)
    {
        slamStopButton.interactable = enable;
        spinButton.gameObject.SetActive(enable);
        infoBtn.interactable = enable;
        plusBtn.interactable = enable;
        minusBtn.interactable = enable;
        MaxBetBtn.interactable = enable;
    }

    private void SetButtonForNormalState()
    {
        SetButton(true);
        SetBetButtonInteractivity();
    }

    private void SetButtonStateForAutoState()
    {
        SetButton(false);
    }

    private void SetButtonForScatterState()
    {
        SetButton(false);
        //bottomPanelDisableTint.SetActive(false);
    }

    private void SetButtonForResumeSpin()
    {
        SetButton(false);
    }

    public void OnSlamStopButtonClicked()
    {
        Audiomanager.Instance.PlayUiSfx(SFX.slamStop);
        SlamStopReelsEvent?.Invoke();
        slamStopButton.interactable = false;
    }

    public void ShowSlamStopButton()
    {
        StartCoroutine(ShowButtonDelay(slamAnticipationDelay, slamStopButton));
    }

    private IEnumerator ShowButtonDelay(float delay, Button button)
    {
        yield return new WaitForSeconds(delay);
        if (!SlotGameEngineStarter.TurboEnabled)
            button.interactable = true;
    }
 
    private void OnSpinComplete()   // Gets called only by NormalState
    {
        SetButton(true);
        SetBetButtonInteractivity();
    }


    private void BalanceDeduction(double currentBalance)
    {
        if (Controller.Instance.CurrentGameState == Controller.GameStatesType.FreeSpin) return;
        EventManager.InvokeSetPlayerData(GameConstants.creditValue[_currentBetIndex] * BetManager.Instance.Bet);
        double balance = double.Parse(availableBalance.text) - double.Parse(current_Bet.text);
        availableBalance.text = balance.ToString(balance % 1 == 0 ? "F0" : "F2");
    }
    private void UpdateBalanceAmount(double won_Amount)
    {
        //double updatedBalance = (double.Parse(availableBalance.text) + won_Amount);
        double updatedBalance = GameApiManager.Instance.CurrentBalance;
        availableBalance.text = updatedBalance.ToString(updatedBalance % 1 == 0 ? "F0" : "F2");
    }

    private void UpdateWheelBalanceAmount()
    {
        double updatedBalance = GameApiManager.Instance.ApiData.GetWheelBonusWonBalance();
        availableBalance.text = updatedBalance.ToString(updatedBalance % 1 == 0 ? "F0" : "F2");
    }

    public void WinAmount(double amount)
    {
        winAmount.text = amount % 1 == 0 ? amount.ToString("F0") : amount.ToString("F2");

    }
    public void UpdateCreditValue()
    {
        current_Bet.text = (GameConstants.creditValue[_currentBetIndex] * BetManager.Instance.Bet).ToString();
        EventManager.InvokeUpdateCreditValueIndex(_currentBetIndex);
        SetBetButtonInteractivity();
    }


    public void MaxBetCreditValue()
    {
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        _currentBetIndex = GameConstants.creditValue.Count - 1;
        UpdateCreditValue();
        MaxBetBtn.interactable = false;
    }

    public void ResetWinAndDisableSlamStop() // Contains Button Edge Cases
    {
        winAmount.text = "0";
        slamStopButton.interactable = false;
    }

    public void DisableSlamStopButton()
    {
        slamStopButton.interactable = false;
    }

    public void ResetWinAmount()
    {
        winAmount.text = "0";
    }

  
    public void SetInitialBalance()
    {
        double balance = GameApiManager.Instance.PlayerData.data.balance;
        availableBalance.text = balance.ToString(balance % 1 == 0 ? "F0" : "F2");
        EventManager.InvokeSetPlayerData(GameConstants.creditValue[_currentBetIndex] * BetManager.Instance.Bet);
    }

    public void OnClickPlus()
    {
        if (_currentBetIndex >= GameConstants.creditValue.Count - 1) return;

        _currentBetIndex++;
        UpdateCreditValue();
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);

    }
    public void OnClickMinus()
    {
        if (_currentBetIndex > 0)
        {
            _currentBetIndex--;
            UpdateCreditValue();
            Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        }

    }
    private void SetBetButtonInteractivity()
    {
        if (_currentBetIndex == GameConstants.creditValue.Count - 1)
        {
            plusBtn.interactable = false;
            MaxBetBtn.interactable = false;
        }
        else
        {
            plusBtn.interactable = true;
            MaxBetBtn.interactable = true;
        }
        if (_currentBetIndex == 0)
        {
            minusBtn.interactable = false;
        }
        else
        {
            minusBtn.interactable = true;
        }
    }

    // Game Info-Panel

    public void InfoPanel()
    {
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        switch (uiIButtonShow.activeInHierarchy)
        {
            case false:
                uiIButtonShow.SetActive(true);
                break;
            case true:
                uiIButtonShow.SetActive(false);
                break;
        }
        UpdateCreditValue();
    }
    public void OnClickPreviousInfo()
    {
        if (uiIButtonPopups.Count - 1 == IButtonValue) return;
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        uiIButtonPopups[IButtonValue].gameObject.SetActive(false);
        IButtonValue++;
        uiIButtonPopups[IButtonValue].gameObject.SetActive(true);
        EventManager.InvokeUpdateCreditValueIndex(_currentBetIndex);
    }
    public void OnClickNextInfo()
    {
        if (IButtonValue == 0) return;
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        uiIButtonPopups[IButtonValue].gameObject.SetActive(false);
        IButtonValue--;
        uiIButtonPopups[IButtonValue].gameObject.SetActive(true);
        EventManager.InvokeUpdateCreditValueIndex(_currentBetIndex);
    }

    public void ResumeUpdateCreditValue(int totalBet)
    {
        int betIndex = GameConstants.creditValue.IndexOf(totalBet / BetManager.Instance.Bet);
        current_Bet.text = (GameConstants.creditValue[betIndex] * BetManager.Instance.Bet).ToString();
        EventManager.InvokeUpdateCreditValueIndex(betIndex);
        _currentBetIndex = betIndex;
    }

    private void UnsubscribeEvents()
    {
        EventManager.OnClickResetDataEvent -= ResetWinAndDisableSlamStop;
        EventManager.OnClickResetWinDataEvent -= ResetWinAmount;
        EventManager.WinAmountEvent -= WinAmount;
        EventManager.BalanceAmountDeductionEvent -= BalanceDeduction;
        EventManager.OnSpinCompleteEvent -= OnSpinComplete;
        EventManager.SetInitialBalanceEvent -= SetInitialBalance;
        EventManager.OnUpdateCurrentBalanceEvent -= UpdateBalanceAmount;

        EventManager.SpinResponseEvent -= ShowSlamStopButton;
        EventManager.ScatterStateStartedEvent -= SetButtonForScatterState;
        EventManager.NormalStateStartedEvent -= SetButtonForNormalState;
        EventManager.UpdateCreditValueEvent -= UpdateCreditValue;
        EventManager.SwitchGameButtonStateEvent -= SetBaseButtonInteractivity;
        EventManager.SetButtonForResumeEvent -= SetButtonForResumeSpin;
        EventManager.DisableSlamStopEvent -= DisableSlamStopButton;

        EventManager.SetUpdatedResumeBet -= ResumeUpdateCreditValue;
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }
}