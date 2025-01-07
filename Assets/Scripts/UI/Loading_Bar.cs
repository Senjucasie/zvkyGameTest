using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loading_Bar : MonoBehaviour
{


    [SerializeField] private Slider loadingSlider;


    private bool isLoadingComplete;


    private void Awake()
    {
        loadingSlider.value = 0;
        isLoadingComplete = false;
    }

    private void Start()
    {
        StartCoroutine(MoveSliderRandomly());
    }

    public IEnumerator MoveSliderRandomly()
    {
        loadingSlider.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        while (!isLoadingComplete)
        {
            if (loadingSlider.value > 99)
            {
                isLoadingComplete = true;
                gameObject.SetActive(false);
                break;
            }
            else
            {
                loadingSlider.value += Random.Range(10, 21);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

  
}
