using UnityEngine;

namespace Popups
{
    public enum PopupCategories
    {
        FreeSpinIntro, FreeSpinOutro,
        BonusIntro, BonusOutro,
        BigWin, MegaWin, SuperWin,
        ResumeInfo
    }

    [System.Serializable]
    public class PopupRegistryEntry
    {
        public PopupCategories popupCategory;
        public Popup popup;
    }


    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private PopupPool _popupPool;

        [Space(10)]
        [SerializeField] private PopupRegistryEntry[] _popupRegistry;

        public static PopupManager Instance;
        private Popup _currentPopup;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _popupPool.CreatePool(_popupRegistry);
        }


        public void ShowPopup(PopupCategories popupcategory, double popuptextvalue = 0)
        {
            ClosePopup();
            _currentPopup = _popupPool.popupPoolDictionary[popupcategory].GetComponent<Popup>();
            _currentPopup.gameObject.SetActive(true);
            _currentPopup.SetPopupData(popupcategory, popuptextvalue);
        }

        public void ClosePopup(PopupCategories popupcategory)
        {
            if (_currentPopup != null && _currentPopup.GetPopupCategory() == popupcategory)
            {
                _currentPopup.gameObject.SetActive(false);
                _currentPopup = null;
            }
        }

        private void ClosePopup()
        {
            if (_currentPopup != null)
            {
                _currentPopup.gameObject.SetActive(false);
                _currentPopup = null;
            }
        }
    }
}
