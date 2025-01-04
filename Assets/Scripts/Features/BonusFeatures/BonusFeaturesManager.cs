using Spin.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BonusFeaturesManager : MonoBehaviour
{
    #region BONUS   
    [SerializeField] private List<BonusRegistry> _bonusList;
    private BonusRegistry _activeBonus;
    #endregion


    private void OnEnable()
    {
        //EventManager.BonusGameStartEvent += StartGameFeature;
        EventManager.BonusStartEvent += StartGameFeature;
        EventManager.BonusGameEndEvent += EndBonusFeature;
    }

    private void Start()
    {
        _activeBonus = _bonusList.FirstOrDefault(bonus => bonus.IsActive);  //LINQ
    }

    #region START_FEATURE
    /*private void StartGameFeature(Action callback)
    {
        _activeBonus.BonusFeature.StartBonusGame(callback);
    }*/

    private void StartGameFeature(GamePlayStateMachine gamePlayStateMachine)
    {
        _activeBonus.BonusFeature?.StartBonusGame(gamePlayStateMachine);
    }
    #endregion

    #region END_FEATURE
    private void EndBonusFeature()
    {
        _activeBonus.BonusFeature?.EndBonusGame();
    }
    #endregion


    private void OnDisable()
    {
        //EventManager.BonusGameStartEvent -= StartGameFeature;
        EventManager.BonusStartEvent -= StartGameFeature;
        EventManager.BonusGameEndEvent -= EndBonusFeature;
    }

}
