using UnityEngine;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour
{
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 initialPosition;
    [SerializeField] Vector3 pageSteps;
    [SerializeField] RectTransform levelPagesRect;
    [SerializeField] float tweenTime;
    [SerializeField] GameObject infoPanel;
    [SerializeField] Button previousBtn, nextBtn;



    private void Awake()
    {
        initialPosition = levelPagesRect.localPosition;
    }
    private void OnEnable()
    {
        currentPage = 1;
        levelPagesRect.localPosition = initialPosition;
    
    }
    public void Next()
    {
        if (currentPage < maxPage)
        {
            Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);

        }
    }
    public void Previous()
    {
        if (currentPage > 1)
        {
            Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
            currentPage--;
            movePage();
        }

    }

    public void Back()
    {
        infoPanel.SetActive(false);
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
    }


    void movePage()
    {
        UpdateArrowButton();
    }

    void UpdateArrowButton()
    {
        nextBtn.interactable = true;
        previousBtn.interactable = true;
        if (currentPage == 1) previousBtn.interactable = false;
        else if (currentPage == maxPage) nextBtn.interactable = false;
    }
}
