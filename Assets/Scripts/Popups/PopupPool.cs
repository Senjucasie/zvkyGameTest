using Popups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPool : MonoBehaviour
{
    public Dictionary<PopupCategories, GameObject> popupPoolDictionary = new();

    public void CreatePool(PopupRegistryEntry[] popupentries)
    {
        foreach (var popupentry in popupentries)
        {
            GameObject popup = Instantiate(popupentry.popup.gameObject, this.transform);
            popup.SetActive(false);
            popupPoolDictionary.Add(popupentry.popupCategory, popup);
        }
    }
}
