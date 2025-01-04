using System;
using System.Collections;
using UnityEngine;
using Popups;

public class WheelBonus : AbstractBonus
{
    public override void StartBonusGame(GamePlayStateMachine gamePlayStateMachine)
    {
        Activate();
        GameApiManager.Instance.SendBonusContinueRequest();
    }

    public override void EndBonusGame()
    {
        BonusGameComplete();
    }

    [SerializeField] private GameObject bonusGamePrefab;
    [SerializeField] private GameObject bonusSpinIntroPopUp;

    private float holdIntroDelay = 5f;
    private static GameObject BonusGamePopUp;
    private GameObject instantiatedPrefab;

    protected Action cB;

    void Start()
    {
        BonusGamePopUp = this.gameObject;
    }

    private void ShowTransition() => TransitionManager.Play();

    private void Activate()
    {
        BonusGamePopUp.SetActive(true);
        Debug.Log("Activate Bonus");
        WheelBonus temp_Instance = BonusGamePopUp.GetComponent<WheelBonus>();
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
        yield return new WaitForSeconds(holdIntroDelay);
        TransitionManager.Play();

        yield return new WaitForSeconds(TransitionManager.SweetSpot);
        instantiatedPrefab = Instantiate(bonusGamePrefab, this.gameObject.transform);
        PopupManager.Instance.ClosePopup(PopupCategories.BonusIntro);

        yield return new WaitForSeconds(2f);
        
        Audiomanager.Instance.PlaySfx(SFX.WheelBuffalo);
    }

    private void BonusGameComplete()
    {
        //EventManager.InvokeUpdateBalance(GameApiManager.Instance.ApiData.GetWheelBonusWinAmount());
        EventManager.InvokeUpdateWheelBonusBalance();
        GameApiManager.Instance.SendSpinCompleteRequest();
        ShowTransition();
        Invoke(nameof(DestroyBonusGamePrefab), TransitionManager.SweetSpot);
    }

    private void DestroyBonusGamePrefab()
    {
        PopupManager.Instance.ClosePopup(PopupCategories.BonusOutro);
        DestroyImmediate(instantiatedPrefab, true);
        EventManager.InvokeBonusSpinGameEnd();
        Controller.Instance.CurrentGameState = Controller.GameStatesType.NormalSpin;
    }
}