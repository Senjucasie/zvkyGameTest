using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using DG.Tweening;
using CusTween;
using Popups;

public class AfterWheelSpin : MonoBehaviour
{
    [SerializeField] private Button WheelSpinSymbol;
    [SerializeField] private PickerWheel pickerWheel;
    [SerializeField] private GameObject wheel;
    [SerializeField] private MultipleFontHandler awardAmountText;
    [SerializeField] private GameObject freeSpinOutroPopUp;


    private void Start()
    {

        WheelSpinSymbol.onClick.AddListener(() =>
        {

            WheelSpinSymbol.interactable = false;
            pickerWheel.OnSpinEnd(wheelPiece =>
          {
              Debug.Log(
               @" <b>Index:</b> " + wheelPiece.Index
            );
              WheelSpinSymbol.interactable = true;
              pickerWheel.pieces[wheelPiece.Index].DOScale(1.2f, 0.5f).SetLoops(4, LoopType.Yoyo).OnComplete(OnWheelSymbolAnimateComplete);
              Audiomanager.Instance.PlaySfx(SFX.WheelOutcome);

          });

            pickerWheel.Spin();

        });

    }
    private void OnWheelSymbolAnimateComplete()
    {
        Audiomanager.Instance.PlaySfx(SFX.BonusOutro);
        //freeSpinOutroPopUp.SetActive(true);

        //double amount = GameApiManager.Instance.ApiData.GetWheelBonusWinAmount();
        double amount = GameApiManager.Instance.ApiData.GetWheelBonusFinalWinAmount();

        PopupManager.Instance.ShowPopup(PopupCategories.BonusOutro, amount);
        Audiomanager.Instance.PlaySfx(SFX.counterLoop);

        Invoke(nameof(BonusGameEndPopUp), 4f);

    }

    private void BonusGameEndPopUp()
    {
        EventManager.InvokeBonusGameEnd();
    }

}
