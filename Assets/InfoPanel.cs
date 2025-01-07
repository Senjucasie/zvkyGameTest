using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField]private Toggle[] _toggles;
    [SerializeField] private GameObject[] _infoPanels;
    [SerializeField] private Button _nextBtn;
    [SerializeField]private Button _backBtn;
    [SerializeField] private Button _closeBtn;
    private int _numbersofPages = 0;
    private int _currentpage = 0;

    private void Awake()
    {
        _numbersofPages = _infoPanels.Length;
        _nextBtn.onClick.AddListener(Next);
        _backBtn.onClick.AddListener(Back);
        _closeBtn.onClick.AddListener(Close);
    }
   
    private void OnEnable()
    {
        ResetPanels();
    }
    private void ResetPanels()
    {
        _currentpage = 1;
        ShowPage(_currentpage);
     }

    private void Next()
    {
        _currentpage++;
        if (_currentpage > _numbersofPages)
        {
            _currentpage = 4;
            return;
        }
        ShowPage(_currentpage);
    }
    private void Back()
    {
        --_currentpage;
        if (_currentpage < 1)
        {
            _currentpage = 1;
            return;
        }

        ShowPage(_currentpage);
    }
    private void ShowPage(int currentapage)
    {
        currentapage = --currentapage;
        _toggles[currentapage].isOn = true;
        for (int i = 0; i < _infoPanels.Length; i++)
        {
            if (i == currentapage)
                _infoPanels[i].SetActive(true);
            else
                _infoPanels[i].SetActive(false);
        }
    }

    private void Close() => gameObject.SetActive(false);
}
