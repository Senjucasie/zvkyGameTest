using System;
using UnityEngine;

public class ResumePopup : MonoBehaviour
{
    public static ResumePopup Instance;
    [SerializeField] private GameObject popupNode;
    private Action ContinueCallback;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        popupNode.SetActive(false);
    }

    public void OnContinueClick()
    {
        popupNode.SetActive(false);
        ContinueCallback?.Invoke();
        ContinueCallback = null;
    }

    public void ShowResumePopup(Action contineCallback)
    {
        popupNode.SetActive(true);
        this.ContinueCallback = contineCallback;
    }
}
