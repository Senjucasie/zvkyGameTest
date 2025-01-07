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
        animationTrack = animationState.SetAnimation(0, startAnimation, isLooping);
    }

  
}
