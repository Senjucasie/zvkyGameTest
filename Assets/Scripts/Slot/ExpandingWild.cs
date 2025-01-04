using System.Collections;
using Spine.Unity;
using UnityEngine;

public enum ExpandingPosition
{
    Top,
    Mid,
    Bottom,
    None
}

public class ExpandingWild : MonoBehaviour
{
    [SerializeField] private GameObject spriteNode;
    [SerializeField] private GameObject spineNode;
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [Header("Animations Name")]
    [Space(8)]
    [SpineAnimation]
    [SerializeField] private string expandDown;
    [SpineAnimation]
    [SerializeField] private string downExpandedLoop;
    [Space(2)]
    [SpineAnimation]
    [SerializeField] private string expandCenter;
    [SpineAnimation]
    [SerializeField] private string centerExpandedLoop;
    [Space(2)]
    [SpineAnimation]
    [SerializeField] private string expandUp;
    [SpineAnimation]
    [SerializeField] private string upExpandedLoop;


    private Spine.AnimationState animationState;
    private string selectedOpeningAnimation;
    private string selectedLoopAnimation;


    private void OnEnable()
    {
        animationState = skeletonAnimation.AnimationState;
        HideWin();
    }

    public void ShowWin(ExpandingPosition position = ExpandingPosition.None)
    {
        Debug.Log("Expanding Wild: ShowWin()");
        spriteNode.SetActive(false);
        spineNode.SetActive(true);

        switch (position)
        {
            case ExpandingPosition.Top:
                {
                    selectedOpeningAnimation = expandDown;
                    selectedLoopAnimation = downExpandedLoop;
                    break;
                }
            case ExpandingPosition.Mid:
                {
                    selectedOpeningAnimation = expandCenter;
                    selectedLoopAnimation = centerExpandedLoop;
                    break;
                }
            case ExpandingPosition.Bottom:
                {
                    selectedOpeningAnimation = expandUp;
                    selectedLoopAnimation = upExpandedLoop;
                    break;
                }
            default:
                {
                    {
                        selectedOpeningAnimation = expandCenter;
                        selectedLoopAnimation = centerExpandedLoop;
                        break;
                    }
                }
        }

        ResetAnimation();
        animationState.SetAnimation(0, selectedOpeningAnimation, false);
        animationState.AddAnimation(0, selectedLoopAnimation, true, 0);
    }

    private void ResetAnimation()
    {
        animationState.ClearTracks();
        skeletonAnimation.skeleton.SetToSetupPose();
    }

    public void HideWin()
    {
        spriteNode.SetActive(true);
        spineNode.SetActive(false);
    }

    public void IncreaseSpineSortingOrder(int increaseBy)
    {
        MeshRenderer mr = spineNode.GetComponent<MeshRenderer>();
        mr.sortingOrder += increaseBy;
    }
}
