using System;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorType
{
    COMMON_ERROR,
    NO_INTERNET,
    LOW_BALANCE
}

[Serializable]
public class ErrorPopupConfig
{
    [SerializeField] public GameObject popupPrefab;
    [SerializeField] public ErrorType type;
}

public class ErrorHandler : MonoBehaviour
{
    public static ErrorHandler Instance;
    [SerializeField] private List<ErrorPopupConfig> errorPopupConfigs;

    private Dictionary<ErrorType, GameObject> errorPopupsDict = new();

    private void Start()
    {
        Instance = this;

        foreach (ErrorPopupConfig errorPopup in errorPopupConfigs)
        {
            errorPopupsDict.Add(errorPopup.type, errorPopup.popupPrefab);
        }
    }

    public void ShowErrorPopup(ErrorType errorType)
    {
        Instantiate(errorPopupsDict[errorType], this.transform);
    }

    public void ShowCommonErrorPopup(string heading, string message)
    {
        ErrorPopup errorPopup = Instantiate(errorPopupsDict[ErrorType.COMMON_ERROR], this.transform)
                                .GetComponent<ErrorPopup>();
        errorPopup.SetErrorData(heading, message);
    }
}