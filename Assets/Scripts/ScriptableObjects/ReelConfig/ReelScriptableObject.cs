using UnityEngine;

[CreateAssetMenu(fileName = "ReelConfig", menuName = "ScriptableObject/NewReelConfig")]

public class ReelScriptableObject : ScriptableObject
{
    //Reels Parameters
    public int Rows;
    public int Columns;
    public float RotationSpeed;
    public bool AnticipateRotation;

    //Normal Reel Parameters
    public float ReelRotationOffset;
    public float SpinAnticipationDuration;
    public float SpinStopDelay;

    //Turbo Reel Parameters
    public float TurboReelRotationOffset;
    public float TurboSpinDelay;
    public float TurboSpinAnticipationDuration;
    public float TurboSpinStopDelay;
}
