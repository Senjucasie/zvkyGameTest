using UnityEngine;
using Spine.Unity;
using Spine;

public class RestartSpineOnEnable : MonoBehaviour
{
    [SerializeField] private GameObject spineGameObject;

    [Space(8)]
    [SpineAnimation][SerializeField] string startAnimation;
    [SerializeField] bool isLooping = false;
    private SkeletonAnimation skeletonAn;
    private SkeletonGraphic skeletonGra;
    private Spine.AnimationState animationState; 
    private TrackEntry animationTrack;
    public TrackEntry AnimationTrack {  get { return animationTrack; } }

    // Start is called before the first frame update
    void Awake()
    {
        skeletonAn = spineGameObject.GetComponent<SkeletonAnimation>();
        skeletonGra = spineGameObject.GetComponent<SkeletonGraphic>();
        if (skeletonAn)
        {

            animationState = skeletonAn.AnimationState;

        }
        else
        {
            animationState = skeletonGra.AnimationState;
        }
    }

    void OnEnable()
    {
        animationState.ClearTracks();
        /*if (skeletonAn)
        {
            skeletonAn.skeleton.SetToSetupPose();
        }
        else
        {
            skeletonGra.Skeleton.SetToSetupPose();
        }*/

        animationTrack = animationState.SetAnimation(0, startAnimation, isLooping);
/*
        if (UIPanel.IsTurboButtonClicked && Controller.Instance.CurrentGameState == Controller.GameStatesType.AutoSpin)
            StartCoroutine(PauseAnimationAfterStartInAutoTurbo());*/
    }

    /*IEnumerator PauseAnimationAfterStartInAutoTurbo()
    {
        yield return new WaitForSeconds(0.0f);

        // pause the animation abruptly
        if (animationTrack != null)
        {
            animationTrack.TimeScale = 0f; // Pause the animation
        }
    }*/
}
