using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SlotMachineEngine;
using Payline = Spin.Api.Payline;
using VisualizedPaylines = SlotMachineEngine.Payline;

public enum PaylineType
{
    Normal,
    Feature
}

public class PaylineController : MonoBehaviour
{
    [SerializeField] private PaylineVisuals _paylineVisuals;
    public GameObject WinTint;
    internal List<VisualizedPaylines> NormalPayline = new();
    internal List<VisualizedPaylines> FeaturePayline = new();
    public SpecialPayline _scatterPayline;  
    public SpecialPayline _bonusPayline;

    public enum PayLineState
    {
        NotShowing = 0,
        FirstIteration,
        Looping
    }
    private PayLineState _state;
    public PayLineState CurrentPayLineState
    {
        get { return _state; }
        set
        {
            //Debug.Log("payline state Value = >  " + value);
            _state = value;
        }
    }
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        CurrentPayLineState = PayLineState.NotShowing;
    }
    public void ResetPayLine()
    {
        _paylineVisuals.Reset();
    }

    public void GeneratePayline(PaylineType paylineType, Payline[] data, int paylineCount = 0)
    {
        List<VisualizedPaylines> paylinesList = paylineType == PaylineType.Normal ? NormalPayline : FeaturePayline;
        paylinesList.Clear();

        for (int i = 0; i < paylineCount; i++)
        {
            var payline = new VisualizedPaylines(data[i].paylineNo, data[i].positions, data[i].symbolCount, data[i].won);
            paylinesList.Add(payline);
            InstantiatePayLineAmount(paylineType, data[i].paylineNo, payline.symbolIds[0], payline.won);
        }
    }

    public IEnumerator ShowNormalPayline()
    {
        for (int currentpayline = 0; currentpayline < NormalPayline.Count; currentpayline++)
        {
            AnimatePaylineAndSymbols(PaylineType.Normal, currentpayline);
            yield return WaitTimeForPayLine(PaylineType.Normal, CurrentPayLineState, currentpayline);
            StopWinAnimation();
            HidePaylineVisual(NormalPayline[currentpayline].id);
        }
    }

    public IEnumerator ShowFeaturePayline()
    {
        for (int currentpayline = 0; currentpayline < FeaturePayline.Count; currentpayline++)
        {
            AnimatePaylineAndSymbols(PaylineType.Feature, currentpayline);
            yield return WaitTimeForPayLine(PaylineType.Feature, CurrentPayLineState, currentpayline);
            StopWinAnimation();
            HidePaylineVisual(FeaturePayline[currentpayline].id);
        }
    }

    public IEnumerator ShowScatterPayline()
    {
        EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.stateSwitchingData, false);

        float waittime = ReelManager.Instance.SystemSetting.ShowPaylineDuration;
        Symbol scattersymbol = null;

        for (int i = 0; i < _scatterPayline.positions.GetLength(0); i++)
        {
            int reelindex = _scatterPayline.positions[i, 0];
            int scattersymbolindex = _scatterPayline.positions[i, 1];
            scattersymbol = ReelManager.Instance.Reels[reelindex].OutcomeSymbols[scattersymbolindex];
            scattersymbol.ShowWin(CurrentPayLineState);
        }
        Audiomanager.Instance.PlaySfx(scattersymbol._audioClip);

        yield return new WaitForSeconds(waittime);
        StopWinAnimation();
        EventManager.InvokeScatterPaylineStopped();
    }

    private void AnimatePaylineAndSymbols(PaylineType paylineType, int currentpayline)
    {
        var payline = paylineType == PaylineType.Normal ? NormalPayline[currentpayline] : FeaturePayline[currentpayline];

        for (int i = 0; i < payline.symbolShowCount; i++)
        {
            ReelManager.Instance.Reels[i].OutcomeSymbols[payline.symbolIds[i]].ShowWin(CurrentPayLineState);
        }

        ShowPaylineVisual(paylineType, payline.id);
    }

    private WaitForSeconds WaitTimeForPayLine(PaylineType paylineType, PayLineState paylinestate, int currentpayline)
    {
        if (paylinestate == PayLineState.FirstIteration)
        {
            Audiomanager.Instance.PlaySfx(SFX.Payline);
            return new WaitForSeconds(ReelManager.Instance.SystemSetting.ShowFlashPaylineDuration);
        }
        else
        {
            List<VisualizedPaylines> payline = paylineType == PaylineType.Normal ? NormalPayline : FeaturePayline;
            Audiomanager.Instance.PlaySfx(ReelManager.Instance.
                                            Reels[0].OutcomeSymbols[
                                            payline[currentpayline].symbolIds[0]]._audioClip);
            return new WaitForSeconds(ReelManager.Instance.SystemSetting.ShowPaylineDuration);
        }
    }

    public void ShowTotalWinAmountVisuals(double currentspincreditwon)
    {
        if (CurrentPayLineState == PayLineState.FirstIteration)
        {
            ShowTotalWinPopup(currentspincreditwon, NormalPayline.Count);
            EventManager.InvokeUpdateBalance(currentspincreditwon);
            EventManager.InvokeWinAmount(currentspincreditwon);
        }
    }

    public void StopWinAnimation()
    {
        foreach (Symbol symbol in ReelManager.Instance.Reels.SelectMany(reel => reel.OutcomeSymbols))
        {
            symbol.HideWin();
        }
    }

    public void InstantiatePayLineAmount(PaylineType paylineType, int paylineid, int position, double winamount)
    {
        _paylineVisuals.InstantiateWinAmount(paylineType, paylineid, position, winamount);
    }

    public void ShowTotalWinPopup(double winamount, int paylineCount) => _paylineVisuals.ShowTotalWin(winamount, paylineCount);
    public void ShowPaylineVisual(PaylineType paylineType, int paylineid)
    {
        _paylineVisuals.ShowPayline(paylineid);
        _paylineVisuals.ShowPaylineAmount(paylineType, paylineid);
    }
    public void HidePaylineVisual(int paylineid)
    {
        _paylineVisuals.HidePayline(paylineid);
        _paylineVisuals.HidePaylineAmount();
    }

}
