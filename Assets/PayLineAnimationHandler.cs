using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class PayLineAnimationHandler : MonoBehaviour
{
    [SpineAnimation]
    [SerializeField] private List <string> _paylineAnimations;

    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _animationState;

    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _animationState = _skeletonAnimation.AnimationState;
    }

    public void PlayAnimation(int id)
    {
        gameObject.SetActive(true);
        if (id>=_paylineAnimations.Count)
        {
            id = _paylineAnimations.Count - 1;
        }
        _animationState.SetAnimation(0, _paylineAnimations[id], true);
    }

    public void HideAnimation()
    {
       gameObject.SetActive(false);
    }

}
