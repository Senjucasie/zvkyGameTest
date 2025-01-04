using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine;


public enum prizeStates
{
    idle,
    open,
    picked,
    None
}


public class PickBonusSymbol : MonoBehaviour
{
    [SerializeField] private GameObject spineNode;
    public GameObject spriteNode;
    [SerializeField] private TextMeshPro wonAmount;
    [Space(8)]
    [SpineAnimation]
    [SerializeField] private string idle;
    [SpineAnimation]
    [SerializeField] private string open;
    [SpineAnimation]
    [SerializeField] private string loop;
    [SpineAnimation]
    [SerializeField] private string none;
    //[SerializeField] private float pickAnimationtime = 5f;

    private string selectedAnimation;
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;
   

    private void Start()
    {
        skeletonAnimation = spineNode.GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
    }
    public void OnPicked(PickBonusSymbol symbol)
    {
        EventManager.InvokePickBonusPrizePicked(symbol);
    }
    public void setWinAmount(string amount)
    {
        wonAmount.text = amount;
        
    }
    public void ChangeState(prizeStates state = prizeStates.None)
    {
        selectedAnimation = state switch
        {
            prizeStates.idle => idle,
            prizeStates.open => open,
            prizeStates.picked => loop,
            _ => none
        };

        animationState.SetAnimation(0, selectedAnimation, false);
        animationState.AddAnimation(0, loop, true, 1);


    }
    public void ResetAnimation()
    {
        animationState.ClearTracks();
        skeletonAnimation.skeleton.SetToSetupPose();
        // disableTint.SetActive(true);
    }
    public void IncreaseSpineSortingOrder(int increaseBy)
    {
        MeshRenderer mr = spineNode.GetComponent<MeshRenderer>();
        mr.sortingOrder += increaseBy;
    }
    public void ResetSymbol()
    {
        //  disableTint.SetActive(false);
        animationState.AddAnimation(0, idle, true, 1);
        wonAmount.text = "";
    }

    
}