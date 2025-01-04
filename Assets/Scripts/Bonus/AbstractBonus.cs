using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractBonus : MonoBehaviour
{
    //public abstract void StartBonusGame(Action callback);
    public abstract void StartBonusGame(GamePlayStateMachine gamePlayStateMachine);
    public abstract void EndBonusGame();
}
