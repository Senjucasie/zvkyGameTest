using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PickBonus : AbstractBonus
{
    public override void StartBonusGame(GamePlayStateMachine gamePlayStateMachine)
    {
        Activate();
    }

    public override void EndBonusGame()
    {
        PickBonusGameComplete();
    }

    [SerializeField] private GameObject bonusIntroPrefab;
    private float transitionSweetSpot = 2.5f;
    private Action cB;

    [SerializeField] private GameObject BonusGame;
    [SerializeField] private GameObject BonusPrefab;
    private static GameObject BonusGamePopUp;
    private GameObject bonusIntroGO;
    private static GameObject game;
    private GameObject bonus;
    private int totalAmount;

    [SerializeField] private List<PickBonusSymbol> prizes = new();

    void Start()
    {
        BonusGamePopUp = this.gameObject;
    }

    private void ShowTransition() => TransitionManager.Play();


    private void Activate()
    {
        BonusGamePopUp.SetActive(true);
        Debug.Log("Activate Bonus");
        PickBonus temp_Instance = BonusGamePopUp.GetComponent<PickBonus>();
        temp_Instance.BonusGameStart();
        temp_Instance.StartCoroutine(temp_Instance.InstantiateBonus());
    }

    private void BonusGameStart()
    {
        Debug.Log("START BONUS ");
        bonusIntroGO =  Instantiate(bonusIntroPrefab, this.transform.position, quaternion.identity, this.transform);
        Audiomanager.Instance.PlaySfx(SFX.BonusIntro);
        Invoke(nameof(ShowTransition), 2f);
    }

    private IEnumerator InstantiateBonus()
    {
        yield return new WaitForSeconds(transitionSweetSpot);
        Audiomanager.Instance.StopMusic(Music.Bgm);
        Audiomanager.Instance.PlayMusic(Music.FreeBgm);
        bonus = Instantiate(BonusPrefab, BonusGamePopUp.transform);
        bonus.GetComponent<PickBonusClass>().getBonusData();
        Destroy(bonusIntroGO);
        BonusGame.SetActive(true);

        yield return new WaitForSeconds(2f);
    }

    private void PickBonusGameComplete()
    {
        ShowTransition();
        Invoke(nameof(DestroyBonusGamePrefab), 1.1f);
    }

    private void DestroyBonusGamePrefab()
    {
        Audiomanager.Instance.StopMusic(Music.FreeBgm);
        Audiomanager.Instance.PlayMusic(Music.Bgm);
        Destroy(bonus);
        Controller.Instance.CurrentGameState = Controller.GameStatesType.NormalSpin;
        EventManager.InvokeBonusSpinGameEnd();
    }
}
