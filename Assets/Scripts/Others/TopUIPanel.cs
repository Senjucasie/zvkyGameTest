using UnityEngine;
using UnityEngine.UI;
public class TopUIPanel : MonoBehaviour
{
    [SerializeField] private Button muteButton;
    [SerializeField] private Button unmuteButton;
    [SerializeField] private Button homeButton;
    private void OnEnable()
    {
        AddButtonListners();
    }
    private void AddButtonListners()
    {
        muteButton.onClick.AddListener(OnMuteButtonClick);
        unmuteButton.onClick.AddListener(OnUnmuteButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }
    private void OnMuteButtonClick()
    {
        SetAudioVolume(0f);
        ToggleMuteButtons();
    }
    private void OnUnmuteButtonClick()
    {
        SetAudioVolume(1f);
        ToggleMuteButtons();
    }
    private void SetAudioVolume(float volume) => AudioListener.volume = volume;
    private void ToggleMuteButtons()
    {
        muteButton.gameObject.SetActive(!muteButton.gameObject.activeSelf);
        unmuteButton.gameObject.SetActive(!unmuteButton.gameObject.activeSelf);
    }
    private void OnHomeButtonClick() => Application.Quit();
}

