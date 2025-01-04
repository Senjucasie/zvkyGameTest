using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using CusTween;
using UnityEngine.UI;
using Spin.Api;

public class PickBonusClass : MonoBehaviour
{
    // [SerializeField] private GameObject BonusGame;
    [SerializeField] private GameObject disableTint;

    [SerializeField] private GameObject bonusSpinOutroPopUp;

    public int[] revealAmount;
    private int totalAmount;
    private int counter = 0;
    [SerializeField] private DoubleFontHandler wonAmount;

    [SerializeField] private List<PickBonusSymbol> prizes = new();
    private PickBonusSymbol prevSymbol = null;
    private PickBonusSymbol currentsymbol = null;
    void Start()
    {
        //EventManager.PickBonusPrizePickedEvent += OnPrizePicked;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        EventManager.PickBonusPrizePickedEvent += OnPrizePicked;
    }
    public void getBonusData()
    {
        Spin.Api.PickBonus bonusData = GameApiManager.Instance.ApiData.GetPickBonusData();
        revealAmount = bonusData.revealAmount;

        counter = 0;
        totalAmount = bonusData.creditWon;

    }

    private void OnPrizePicked(PickBonusSymbol symbol)
    {
        symbol.spriteNode.GetComponent<Button>().interactable = false;
        currentsymbol = symbol;
        // if (prevSymbol != null)
        //     prevSymbol.GetComponent<Button>().interactable = false;
        if (counter >= revealAmount.Length)
        {
            // BonusGameComplete();
        }
        else
        {
            OpenPrize(symbol);
        }
    }
    private void OpenPrize(PickBonusSymbol symbol)
    {
        //  int sortingOffset = 1;
        //   symbol.IncreaseSpineSortingOrder(baseSpineSortingId + sortingOffset);
        disableTint.SetActive(true);
        Audiomanager.Instance.PlaySfx(SFX.jackpotOpen);
        Invoke(nameof(SetActiveObjects), 0.2f);

        symbol.ChangeState(prizeStates.open);
        Invoke(nameof(SetCurrentWinAmount), 0.2f);
    }

    private void SetCurrentWinAmount()
    {

        if (revealAmount[counter] == 0)
        {
            currentsymbol.setWinAmount("Exit ");
        }
        else
        {
            int winAmount = revealAmount[counter];
            currentsymbol.setWinAmount(winAmount.ToString());
        }

        prevSymbol = currentsymbol;
        counter++;
        if (counter >= revealAmount.Length)
        {
            wonAmount.setText(totalAmount);
            Invoke(nameof(ShowOutro), 2f);
            EventManager.InvokeUpdateBalance(totalAmount);

        }
    }
    private void ShowOutro()
    {
        bonusSpinOutroPopUp.SetActive(true);
        Audiomanager.Instance.PlaySfx(SFX.BonusOutro);
        Invoke(nameof(BonusEnd), 3.5f);

    }
    private void BonusEnd()
    {
        EventManager.InvokeBonusGameEnd();
    }

    private void SetActiveObjects()
    {
        disableTint.SetActive(false);
    }

    private void UnsubscribeEvents()
    {
        EventManager.PickBonusPrizePickedEvent -= OnPrizePicked;
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }
}
