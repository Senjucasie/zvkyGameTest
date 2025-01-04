using UnityEngine;
using Spine.Unity;
using UnityEditor;

public class SpineAnimationSequencer : MonoBehaviour
{
    [SpineAnimation]
    [SerializeField] private string startAnimationState;
    [SpineAnimation]
    [SerializeField] private string midAnimationState;
    [SpineAnimation]
    [SerializeField] private string exitAnimationState;

    [Space(8)]
    [SerializeField] private bool resetOnEnable;
    [SerializeField] private bool loopMidAnimationState;

    [Space(8)]
    [SerializeField] public bool playExitAnimation;
    [Tooltip("Time after with exit animation show be played (if mid is not looping)")]
    [HideInInspector] public float playExitAnimationAfter;

    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;

    void OnEnable()
    {
        skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;

        if (resetOnEnable)
            ResetAnimation();

        AnimationStartSequence();
    }

    private void AnimationStartSequence()
    {
        animationState.SetAnimation(0, startAnimationState, false);

        Spine.Animation startClip = skeletonAnimation.Skeleton.Data.FindAnimation(startAnimationState);
        float addDelay = startClip.Duration;
        animationState.AddAnimation(0, midAnimationState, loopMidAnimationState, addDelay);

        if (playExitAnimation)
            animationState.AddAnimation(0, exitAnimationState, false, playExitAnimationAfter);
    }

    private void ResetAnimation()
    {
        //skeletonAnimation.skeleton.SetToSetupPose();
        animationState.SetEmptyAnimation(0, 0);
        animationState.ClearTracks();
    }
}

#if UNITY_EDITOR
// CUSTOM INSPECTOR EDITOR FOR THIS CLASS
[CustomEditor(typeof(SpineAnimationSequencer))]
class ScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SpineAnimationSequencer script = (SpineAnimationSequencer)target;

        if (script.playExitAnimation)
        {
            // Ensure the label and the value are on the same line
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PlayExitAnimationAfter");

            // Show and save the value of playExitAnimationAfter
            script.playExitAnimationAfter = EditorGUILayout.FloatField(script.playExitAnimationAfter);

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif