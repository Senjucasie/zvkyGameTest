using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreExpandedWildSymbol : MonoBehaviour
{
    [SerializeField] public GameObject spriteNode;
    [SerializeField] public GameObject spineNode;

    [SerializeField] private PaylinesAnimationController paylineAnimationController;

    private void Start()
    {
        HideWin();
    }

    public void ShowWin(PaylineController.PayLineState paylinestate)
    {
        spriteNode.SetActive(false);
        spineNode.gameObject.SetActive(true);

        if (paylineAnimationController != null)
        {
            paylineAnimationController.ManagePaylineAnimForGameState(paylinestate);
        }
    }



    public void HideWin()
    {
        spriteNode.SetActive(true);
        spineNode.gameObject.SetActive(false);
    }

    public void SetSpineSortingOrder(int sortingOrder)
    {
        MeshRenderer mr = spineNode.GetComponent<MeshRenderer>();
        mr.sortingOrder = sortingOrder;
    }
}
