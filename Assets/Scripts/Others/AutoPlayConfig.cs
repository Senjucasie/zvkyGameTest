using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlayConfig : MonoBehaviour
{
    [SerializeField] private Toggle turboModeToggle;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle defaultCountToggle;

    private int count = 10;

    public static event Action<int> StartAutoSpinEvent;
    public static event Action<bool> ToggleTurboStateEvent;
    public static event Action AutoPlayToggleReSetEvent;

    private void OnEnable()
    {
        turboModeToggle.isOn = BottomUIPanel.IsTurboButtonClicked;
        StartCoroutine(ResetToggleRoutine());
    }
    public void OnCountToggle(int count)
    {
        this.count = count;
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
    }

    private IEnumerator ResetToggleRoutine()
    {
        yield return new WaitForSeconds(0.01f);
        defaultCountToggle.isOn = true;
        count = 10;
    }

    public void OnTurboModeToggle()
    {
       ToggleTurboStateEvent?.Invoke(turboModeToggle.isOn);
       Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
    }

    public void OnStartClick()
    {
        if (EconomyManager.HasSufficientBalance())
        {
            StartAutoSpinEvent?.Invoke(count);
            gameObject.SetActive(false);
            Audiomanager.Instance.PlayUiSfx(SFX.SpinBtn);
        }
        else
        {
            ErrorHandler.Instance.ShowErrorPopup(ErrorType.LOW_BALANCE);
        }
    }

    public void OnCloseClick()
    {
        AutoPlayToggleReSetEvent?.Invoke();
        Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
        this.gameObject.SetActive(false);
        
    }
}
