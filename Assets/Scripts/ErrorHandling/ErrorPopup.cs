using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headingText;
    [SerializeField] private TextMeshProUGUI messageText;

    // ON CLOSE BUTTON CLICK
    public void OnCloseButtonClick()
    {
        Destroy(this.gameObject);
    }

    public void SetErrorData(string heading, string message)
    {
        headingText.text = heading;
        messageText.text = message;
    }
}