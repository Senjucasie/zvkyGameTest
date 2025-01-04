using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

//Button Interactivity Classes
[System.Serializable]
public class BaseButtonBehaviour
{
    public Button button;
    public bool interactivity;
}

public class BaseButtonInteractionRegistry : MonoBehaviour
{
    public static BaseButtonInteractionRegistry Instance;

    [SerializeField] internal List<BaseButtonBehaviour> normalData;
    [SerializeField] internal List<BaseButtonBehaviour> scatterData;
    [SerializeField] internal List<BaseButtonBehaviour> autoData;
    [SerializeField] internal List<BaseButtonBehaviour> bonusData;
    [SerializeField] internal List<BaseButtonBehaviour> reSpinBonusData;
    [SerializeField] internal List<BaseButtonBehaviour> stateSwitchingData;

    private void Awake()
    {
        Instance = this;
    }
}
