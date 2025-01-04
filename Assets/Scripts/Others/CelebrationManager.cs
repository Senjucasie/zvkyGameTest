using UnityEngine;
using System.Collections;
using System;
using Popups;

public enum CelebrationWinType
{
    NormalWin,
    SuperWin,
    MegaWin,
    BigWin
}

public class CelebrationManager : MonoBehaviour
{
    public static CelebrationWinType CurrentWinType = CelebrationWinType.NormalWin;

    private double superWinAmount;
    private double megaWinAmount;
    private double bigWinAmount;

    public static CelebrationManager Instance { get; private set; }

    public static bool isCelebrationPopUpActive;
    private void Awake()
    {
        Instance = this;
        isCelebrationPopUpActive = false;
    }
    private void OnEnable()
    {
        EventManager.WinCelebration += ShowWinCelebration;
        EventManager.CheckWinCelebrationEvent += CheckWinCelebration;
    }

    private void Start()
    {
        superWinAmount = BetManager.Instance.Bet * 50; //1200;// bet *50 // New: 1500+
        megaWinAmount = BetManager.Instance.Bet * 30; // 800;   // bet *30 // New: 900+
        bigWinAmount = BetManager.Instance.Bet * 15; //500;   // bet *15 // New: 450+
    }

    private void CheckWinCelebration(double winAmount)
    {
        if (winAmount > bigWinAmount)
            isCelebrationPopUpActive = true;

        CurrentWinType = GetCelebrationWinType(winAmount);
    }

    private CelebrationWinType GetCelebrationWinType(double winamount)
    {
        if (winamount > superWinAmount)
            return CelebrationWinType.SuperWin;

        else if (winamount > megaWinAmount)
            return CelebrationWinType.MegaWin;

        else if (winamount > bigWinAmount)
            return CelebrationWinType.BigWin;

        else
            return CelebrationWinType.NormalWin;
    }

    #region Celebration Manager
    private void ShowWinCelebration(double winAmount)
    {
        if (CurrentWinType == CelebrationWinType.NormalWin)
            return;

        PopupCategories selectedPopUp = PopupCategories.BigWin;

        if (CurrentWinType == CelebrationWinType.SuperWin)
        {
            selectedPopUp = PopupCategories.SuperWin;
        }
        else if (CurrentWinType == CelebrationWinType.MegaWin)
        {
            selectedPopUp = PopupCategories.MegaWin;
        }
        else if (CurrentWinType == CelebrationWinType.BigWin)
        {
            selectedPopUp = PopupCategories.BigWin; ;
        }

        CommandQueue.AddAction((callback) =>
        {
            ShowWin(selectedPopUp, winAmount, callback);
        }, 0f, 20, "SpecialWin");
        CoroutineStarter.Instance.StartCoroutine(CommandQueue.StartExecution());
    }


    #endregion Celebration Manager

    #region ShowWin

    private void ShowWin(PopupCategories selectedPopUp, double winAmount, Action callback)
    {
        //SlotMachine.isCelebrationPopUpActive = true;
        Audiomanager.Instance.PlaySfx(SFX.SuperWin);

        PopupManager.Instance.ShowPopup(selectedPopUp, winAmount);
        StartCoroutine(DeactivatePopUp(callback));
    }
    #endregion ShowWin

    public IEnumerator ShowCelebrationPopupAndWait(double currentspincreditwon)
    {
        if (CurrentWinType == CelebrationWinType.NormalWin) yield break;

        ShowWinCelebration(currentspincreditwon);
        yield return new WaitWhile(() => isCelebrationPopUpActive);
    }

    private IEnumerator DeactivatePopUp(Action callback)
    {
        yield return new WaitForSeconds(6.5f);

        Debug.Log("Command Queue Count at end: " + CommandQueue.queue.Count);
        callback();

        PopupManager.Instance.ClosePopup(PopupCategories.BigWin);
        PopupManager.Instance.ClosePopup(PopupCategories.MegaWin);
        PopupManager.Instance.ClosePopup(PopupCategories.SuperWin);

        CurrentWinType = CelebrationWinType.NormalWin;
        isCelebrationPopUpActive = false;
    }
    private void OnDisable()
    {
        EventManager.WinCelebration -= ShowWinCelebration;
        EventManager.CheckWinCelebrationEvent -= CheckWinCelebration;
    }
}