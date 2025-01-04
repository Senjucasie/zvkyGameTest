using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AutoPlayToggle : MonoBehaviour
{
    private bool _toggle;
    private Button _autoPlayButton;
    [SerializeField]private BottomUIPanel _bottomUIPanel;

    void Awake()
    {
        _autoPlayButton = GetComponent<Button>();
        _autoPlayButton.onClick.AddListener(OnButtonClick);
        InitToggle();
    }
    private void OnEnable() 
    {
        EventManager.OnAutoSpinStopEvent += InitToggle;
        AutoPlayConfig.AutoPlayToggleReSetEvent += InitToggle;
       
    }
    private void InitToggle() => _toggle = true;
    private void OnButtonClick()
    {
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        _bottomUIPanel.OnAutoButtonClicked(_toggle);
        _toggle = !_toggle;
    }

    private void OnDisable()
    {
        EventManager.OnAutoSpinStopEvent -= InitToggle;
        AutoPlayConfig.AutoPlayToggleReSetEvent -= InitToggle;
    }
}
