using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemConfig", menuName = "ScriptableObject/NewSystemConfig")]

public class SystemScriptableObject : ScriptableObject
{
    public List<GameObject> SymbolPrefabs;
    public float ShowPaylineDuration; 
    public float ShowFlashPaylineDuration;
    public bool ShowPaylinesInLoop;
    public int ScatterId;
    public int BonusId;
    public bool AnticipateScatter;
    public float AnticipationDuration;
}