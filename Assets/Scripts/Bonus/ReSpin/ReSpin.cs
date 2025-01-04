using CusTween;
using Popups;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReSpin : AbstractBonus
{
    GamePlayStateMachine gamePlayStateMachine;

    [SerializeField] private ReSpinDriver reSpinDriver;

    [SerializeField] private GameObject reSpinIntroPopUp;
    [SerializeField] private GameObject reSpinOutroPopUp;
    [SerializeField] private GameObject SpinButton;
    [SerializeField] private GameObject BGImage;
    [SerializeField] private GameObject ReSpinReelFrame;
    [SerializeField] private GameObject BaseReelFrame;
    [SerializeField] private GameObject ReSpinTextNode;

    [SerializeField] private MultipleFontHandler totalReSpinText;
    [SerializeField] private GameObject currentReSpinNode;
    [SerializeField] private TextMeshProUGUI currentReSpinText;
    [SerializeField] private MultipleFontHandler totalReSpinWinAmonut;
    //[SerializeField] private float introHoldTime = 5f;

    private float holdIntroDelay = 5f;
    protected Action cB;


    private static GameObject BonusGamePopUp;
    private GameObject instantiatedPrefab;


    public override void StartBonusGame(GamePlayStateMachine gamePlayStateMachine)
    {
        this.gamePlayStateMachine = gamePlayStateMachine;
        Activate();
        //GameApiManager.Instance.SendBonusContinueRequest();
    }

    public override void EndBonusGame()
    {
        BonusGameComplete();
    }

    private void OnEnable()
    {
        EventManager.OnReSpinPlayed += CurrentReSpin;
    }

    void Start()
    {
        BonusGamePopUp = this.gameObject;
    }

    private void Activate()
    {
        BonusGamePopUp.SetActive(true);
        Debug.Log("Activate ReSpin Bonus");
        ReSpin temp_Instance = BonusGamePopUp.GetComponent<ReSpin>();
        temp_Instance.BonusGameStart();
    }

    private void BonusGameStart()
    {
        PopupManager.Instance.ShowPopup(PopupCategories.BonusIntro);
        Audiomanager.Instance.PlaySfx(SFX.BonusIntro);

        StartCoroutine(InstantiateBonus());
    }

    private IEnumerator InstantiateBonus()
    {
        int reSpinAwarded = GameApiManager.Instance.GetReSpinCount();
        yield return new WaitForSeconds(holdIntroDelay);
        TransitionManager.Play();
        yield return new WaitForSeconds(TransitionManager.SweetSpot);
        PopupManager.Instance.ClosePopup(PopupCategories.BonusIntro);
        
        reSpinDriver.Enter(gamePlayStateMachine); 

        yield return new WaitForSeconds(2f);

        SetReSpinUi();
        currentReSpinNode.SetActive(true);
        currentReSpinText.text = reSpinAwarded.ToString();
        Audiomanager.Instance.PlaySfx(SFX.WheelBuffalo);
    }

    public void BonusGameComplete()
    {
        GameApiManager.Instance.SendSpinCompleteRequest();
        Debug.Log("ReSpin Bonus Game Complete");
        TransitionManager.Play();
        StartCoroutine(DeactivateReSpin());
    }

    private IEnumerator DeactivateReSpin()
    {
        double reSpinWinAmount = GameApiManager.Instance.ApiData.GetReSpinAmoutWon();

        PopupManager.Instance.ShowPopup(PopupCategories.BonusOutro, reSpinWinAmount);
        Audiomanager.Instance.PlaySfx(SFX.BonusOutro);
        yield return new WaitForSeconds(holdIntroDelay);
        TransitionManager.Play();
        yield return new WaitForSeconds(TransitionManager.SweetSpot);
        RemoveReSpinUi();
        PopupManager.Instance.ClosePopup(PopupCategories.BonusOutro);

        currentReSpinNode.SetActive(false);
        Controller.Instance.CurrentGameState = Controller.GameStatesType.NormalSpin;

        EventManager.InvokeBonusSpinGameEnd();
    }
        
    private void SetReSpinUi()
    {
        Audiomanager.Instance.StopMusic(Music.Bgm);
        Audiomanager.Instance.PlayMusic(Music.FreeBgm);
        SpinButton.GetComponent<Button>().interactable = false;
        BGImage.SetActive(true);
        ReSpinReelFrame.SetActive(true);
        Debug.Log("transition ReSpinUi");
        BaseReelFrame.GetComponent<Image>().enabled = false;
        ReSpinTextNode.SetActive(true);
    }

    private void RemoveReSpinUi()
    {
        Audiomanager.Instance.StopMusic(Music.FreeBgm);
        Audiomanager.Instance.PlayMusic(Music.Bgm);
        SpinButton.GetComponent<Button>().interactable = true;
        BGImage.SetActive(false);
        ReSpinReelFrame.SetActive(false);
        BaseReelFrame.GetComponent<Image>().enabled = true;
        ReSpinTextNode.SetActive(false);
        EventManager.InvokeResetWinData();
    }


    private void CurrentReSpin(int currentReSpin)
    {
        currentReSpinText.text = currentReSpin.ToString();
        Debug.Log("Current reSpin count " + currentReSpin);
    }

    private void OnDisable()
    {
        EventManager.OnReSpinPlayed -= CurrentReSpin;
    }
}
