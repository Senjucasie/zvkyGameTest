using Spine;
using UnityEngine;

[RequireComponent(typeof(RestartSpineOnEnable))]
public class PaylinesAnimationController : MonoBehaviour
{
    [SerializeField] private GameObject spineGameObject;
    private TrackEntry animationTrack;
    private int _symbolId;

    private void OnEnable()
    {
        _symbolId = this.GetComponentInParent<Symbol>().SymbolID;
    }
    public void ManagePaylineAnimForGameState(PaylineController.PayLineState paylinestate)
    {
        animationTrack = spineGameObject.GetComponent<RestartSpineOnEnable>().AnimationTrack;

        if (paylinestate == PaylineController.PayLineState.FirstIteration)
        {
            if (animationTrack != null)
            {
                animationTrack.TimeScale = IsSpecialSymbol() ? 1f : 0f;
                //Debug.Log("Spine if isAlreadyInAutoTurboMode 1st" + isAlreadyInAutoTurboMode + " " + SlotMachine.isInAutoTurboRound);
                /*animationTrack.TimeScale = 0f; // Pause the animation
                if (_symbolId == ReelManager.Instance.SystemSetting.ScatterId || _symbolId == ReelManager.Instance.SystemSetting.BonusId)
                {
                    animationTrack.TimeScale = 1f;
                }*/

            }
        }

        /* if (isAlreadyInAutoTurboMode)
         {
             if (animationTrack != null)
             {
                 Debug.Log("Spine if isAlreadyInAutoTurboMode 2nd" + isAlreadyInAutoTurboMode + " " + SlotMachine.isInAutoTurboRound);
                 animationTrack.TimeScale = 0f; // Pause the animation
             }
         }
         else
         {
             isAlreadyInAutoTurboMode = false;
             Debug.Log("Spine if isAlreadyInAutoTurboMode 3nd" + isAlreadyInAutoTurboMode + " " + SlotMachine.isInAutoTurboRound);
             //animationTrack.TimeScale = 1f;
         }*/   

    }
    private bool IsSpecialSymbol()
    {
        var systemSetting = ReelManager.Instance.SystemSetting;
        return _symbolId == systemSetting.ScatterId || _symbolId == systemSetting.BonusId;
    }

}
