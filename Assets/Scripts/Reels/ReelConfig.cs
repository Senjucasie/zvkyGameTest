using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReelConfig
{
    [field: SerializeField] public int Rows { get; private set; }//r
    [field: SerializeField] public int Columns { get; private set; }//r
    [field: SerializeField] public float RotationSpeed { get; private set; }//r
    [field: SerializeField] public float ReelRotationOffset { get; private set; }//r
    [field: SerializeField] public bool AnticipateRotation { get; private set; }//r
    [field: SerializeField] public float SpinAnticipationDuration { get; private set; }//r
    [field: SerializeField] public float TurboSpinDelay { get; private set; }
    [field: SerializeField] public float SpinStopDelay { get; private set; }//r
    [field: SerializeField] public List<GameObject> SymbolPrefabs { get; private set; } = new();

    [field: Space(12)]
    [field: SerializeField] public float ShowPaylineDuration { get; private set; } = 2f;
    [field: SerializeField] public float ShowFlashPaylineDuration { get; private set; } = 0.5f;
    [field: SerializeField] public bool ShowPaylinesInLoop { get; private set; }

    [field: Space(12)]
    [field: SerializeField] public int ScatterId { get; private set; }
    [field: SerializeField] public bool AnticipateScatter { get; private set; }
    [field: SerializeField] public float ScatterAnticipationDuration { get; private set; } = 5f;


  

    public void SetConfig(float spinAnticipationDuration, float spinStopDelay,float reeloffset )
    {
        SpinAnticipationDuration = spinAnticipationDuration;
        SpinStopDelay = spinStopDelay;
        ReelRotationOffset = reeloffset;
    }
}