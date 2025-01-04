using System.Collections;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject transition;


    #region 
    public static float SweetSpot = 1.0f;
    [SerializeField] private float _exitTime = 2.95f;
    #endregion 

    #region internals

    private static GameObject thisGameObject;
    #endregion internals

    // Start is called before the first frame update
    void Start()
    {
        // transition.gameObject.SetActive(false);
        thisGameObject = this.gameObject;
    }

    public static void Play()
    {
        TransitionManager temp_Ins = thisGameObject.GetComponent<TransitionManager>();
        temp_Ins.ShowTransition();
        Audiomanager.Instance.PlaySfx(SFX.Transition);


    }
    private void ShowTransition()
    {
        transition.gameObject.SetActive(true);
        StartCoroutine(FinishTransition());

    }
    IEnumerator FinishTransition()
    {
        yield return new WaitForSeconds(_exitTime);
        transition.gameObject.SetActive(false);
    }

}
