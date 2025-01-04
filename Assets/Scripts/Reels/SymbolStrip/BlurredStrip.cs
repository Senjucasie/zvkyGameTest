using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlurredStrip : MonoBehaviour
{
    RectTransform rt;
    VerticalLayoutGroup vlg;
    [SerializeField] private float symbolHeight = 260f;
    private float stripHeight;
    private Coroutine _illusionCoroutine;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        vlg = GetComponentInParent<VerticalLayoutGroup>();
        this.gameObject.SetActive(false);
    }

    public void StartSpinIllusion(float rotationSpeed)
    {
        _illusionCoroutine = StartCoroutine(BlurredStripSpinCoroutine(rotationSpeed));
    }

    public void StopSpinIllusion()
    {
        if(_illusionCoroutine != null)
        StopCoroutine(_illusionCoroutine);
        _illusionCoroutine = null;
    }

    private IEnumerator BlurredStripSpinCoroutine(float rotationSpeed)
    {
        yield return new WaitForSeconds(0f);

        #region Actual Spinning

        stripHeight = rt.sizeDelta.y;
        float verticalSpacing = Math.Abs(vlg.spacing);
        float requiredOffsetY = (stripHeight / 2) - (symbolHeight + verticalSpacing);

        while (true)
        {
            rt.transform.Translate(Vector3.down * rotationSpeed * Time.deltaTime);

            if (rt.transform.localPosition.y <= -1 * requiredOffsetY)
            {
                rt.transform.localPosition = new Vector3(0, requiredOffsetY, 0);
            }
            yield return null;
        }

        #endregion Actual Spinning
    }
}
