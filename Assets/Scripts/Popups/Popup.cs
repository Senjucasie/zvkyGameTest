using CusTween;
using Popups;
using System;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private PopupCategories _popupType;
    [SerializeField] private MultipleFontHandler _popupText;
    public void SetPopupData(PopupCategories popuptype, double value)
    {
        this._popupType = popuptype;
        if(_popupText != null) SetPopupText(value);
    }

    private void SetPopupText(double value)
    {
        _popupText.setText(0);
        {
            if ((value) % 1 == 0)
            {
                _popupText.setText((Int32)value);
            }
            else
            {
                float valuef = MathF.Round((float)value, 2);
                _popupText.setText(valuef);
            }
        }
    }

    public PopupCategories GetPopupCategory()
    {
        return this._popupType;
    }

    public void DestroyPopup()
    {
        Destroy(this.gameObject);
    }
}
