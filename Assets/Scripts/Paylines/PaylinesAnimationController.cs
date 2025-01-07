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
            }
        }
  

    }
    private bool IsSpecialSymbol()
    {
        var systemSetting = ReelManager.Instance.SystemSetting;
        return _symbolId == systemSetting.ScatterId || _symbolId == systemSetting.BonusId;
    }

}
