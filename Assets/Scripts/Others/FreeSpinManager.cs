using System;
using System.Collections;
using CusTween;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreeSpinManager : MonoBehaviour
{

    private static GameObject freeSpinPopUp;

    [SerializeField] private GameObject freeSpinIntroPopUp;
    [SerializeField] private GameObject freeSpinOutrPopUp;
    [SerializeField] private GameObject SpinButton;
    [SerializeField] private GameObject BGImage;
    [SerializeField] private GameObject FreeSpinReelFrame;
    [SerializeField] private GameObject BaseReelFrame;
    [SerializeField] private GameObject FreeSpinTextNode;

    [SerializeField] private MultipleFontHandler totalFreeSpinText;
    [SerializeField] private GameObject currentFreeSpinNode;
    [SerializeField] private TextMeshProUGUI currentFreeSpinText;
    [SerializeField] private MultipleFontHandler totalFreeSpinWinAmonut;
    [SerializeField] private float introHoldTime = 5f;


    void Start()
    {
        freeSpinPopUp = this.gameObject;
        freeSpinPopUp.SetActive(false);
    }

    void OnEnable()
    {
        EventManager.OnFreeSpinPlayed += CurrentSpin;
    }

    public static void Activate(Action callback = null)
    {
        freeSpinPopUp.SetActive(true);
        Debug.Log("Activating Free Spin");

        FreeSpinManager temp_Instance = freeSpinPopUp.GetComponent<FreeSpinManager>();
        temp_Instance.StartCoroutine(temp_Instance.SetupFreeSpinView(callback));
    }

    private IEnumerator SetupFreeSpinView(Action callBack = null)
    {
        int freeSpinAwarded = GameApiManager.Instance.GetFreeSpinCount();
        Debug.Log("FreeSpinAwarded: " + freeSpinAwarded);

        PopupManager.Instance.ShowPopup(PopupCategories.FreeSpinIntro, freeSpinAwarded);
        Audiomanager.Instance.PlaySfx(SFX.FreeIntro);
        yield return new WaitForSeconds(introHoldTime);

        PopupManager.Instance.ClosePopup(PopupCategories.FreeSpinIntro);
        ShowTransition();
        yield return new WaitForSeconds(TransitionManager.SweetSpot);

        EventManager.InvokeFreeSpinIntroEnded();
        SetFreeSpinUi();
        currentFreeSpinNode.SetActive(true);
        currentFreeSpinText.text = freeSpinAwarded.ToString();

        callBack();
    }

    private void SetFreeSpinUi()
    {
        Audiomanager.Instance.StopMusic(Music.Bgm);
        Audiomanager.Instance.PlayMusic(Music.FreeBgm);
        SpinButton.GetComponent<Button>().interactable = false;
        BGImage.SetActive(true);
        FreeSpinReelFrame.SetActive(true);
        Debug.Log("transition FreespinUi");
        BaseReelFrame.GetComponent<Image>().enabled = false;
        FreeSpinTextNode.SetActive(true);
    }
    private void RemoveFreeSpinUi()
    {
        Audiomanager.Instance.StopMusic(Music.FreeBgm);
        Audiomanager.Instance.PlayMusic(Music.Bgm);
        SpinButton.GetComponent<Button>().interactable = true;
        BGImage.SetActive(false);
        FreeSpinReelFrame.SetActive(false);
        BaseReelFrame.GetComponent<Image>().enabled = true;
        FreeSpinTextNode.SetActive(false);
        EventManager.InvokeResetWinData();
    }

    public static void Deactivate(Action callback = null)
    {
        FreeSpinManager temp_Instance = freeSpinPopUp.GetComponent<FreeSpinManager>();
        temp_Instance.StartCoroutine(temp_Instance.FreeSpinDeactivate(callback));

        Debug.Log("FreeSpinExitDialogBox");
    }
    private void ShowTransition() => TransitionManager.Play();

    private IEnumerator FreeSpinDeactivate(Action callback)
    {
        double freeSpinWinAmount = GameApiManager.Instance.ApiData.GetFreeSpinAmoutWon();
        PopupManager.Instance.ShowPopup(PopupCategories.FreeSpinOutro, freeSpinWinAmount);

        Audiomanager.Instance.PlaySfx(SFX.FreeOutro);
        Audiomanager.Instance.PlaySfx(SFX.counterLoop);
        
        yield return new WaitForSeconds(introHoldTime);
        ShowTransition();
        yield return new WaitForSeconds(TransitionManager.SweetSpot);

        RemoveFreeSpinUi();
        PopupManager.Instance.ClosePopup(PopupCategories.FreeSpinOutro);
        currentFreeSpinNode.SetActive(false);
        Controller.Instance.CurrentGameState = Controller.GameStatesType.NormalSpin;
        EventManager.InvokeFreeSpinGameEnd();
        callback();
    }

    private void CurrentSpin(int currentSpin)
    {
        currentFreeSpinText.text = currentSpin.ToString();
        Debug.Log("Current freeSpin  Count  " + currentSpin);
    }

    void OnDisable()
    {
        EventManager.OnFreeSpinPlayed -= CurrentSpin;
    }



}
