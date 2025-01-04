using UnityEngine;
using Spine.Unity;
using UnityEditor;

public class SkeletonGraphicSequencer : MonoBehaviour
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

    private SkeletonGraphic skeletonAnimation;
    private Spine.AnimationState animationState;

    void OnEnable()
    {
        skeletonAnimation = gameObject.GetComponent<SkeletonGraphic>();
        animationState = skeletonAnimation.AnimationState;

        if (resetOnEnable)
            ResetSpine();

        AnimationStartSequence();
    }

    private void AnimationStartSequence()
    {
        Debug.Log("set animations entry popup");
        animationState.SetAnimation(0, startAnimationState, false);

        Spine.Animation startClip = skeletonAnimation.Skeleton.Data.FindAnimation(startAnimationState);
        float addDelay = startClip.Duration;
        animationState.AddAnimation(0, midAnimationState, loopMidAnimationState, addDelay);

        if (playExitAnimation)
            animationState.AddAnimation(0, exitAnimationState, false, playExitAnimationAfter);
    }

    private void ResetSpine()
    {
        animationState.ClearTracks();
        skeletonAnimation.Skeleton.SetToSetupPose();
    }
}

#if UNITY_EDITOR
// CUSTOM INSPECTOR EDITOR FOR THIS CLASS
[CustomEditor(typeof(SkeletonGraphicSequencer))]
class ScriptEditors : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SkeletonGraphicSequencer script = (SkeletonGraphicSequencer)target;

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