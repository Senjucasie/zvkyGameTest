using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    [field: SerializeField] public ReelScriptableObject ReelSetting { get; private set; }
    [field: SerializeField] public SystemScriptableObject SystemSetting { get; private set; }

    internal SpinState _currentSpinState;

    internal float SpinAnticipationDuration;
    internal float SpinStopDelay;
    internal float ReelRotationOffset;
    internal bool IsWaitingForIndependentSpecialSymbol;

    [SerializeField] private List<Reel> reels = new();

    public static ReelManager Instance;

    public List<Reel> Reels { get => reels; }

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        this._currentSpinState = SpinState.Idle;
    }

    internal void SetInitialConfig()
    {
        SetConfig(ReelSetting.SpinAnticipationDuration, ReelSetting.SpinStopDelay, ReelSetting.ReelRotationOffset);
    }

    public void SetReels()
    {
        foreach (var reel in reels)
            reel.ConfigureReel(ReelSetting, SystemSetting);
    }

    public IEnumerator InitiateSpinCoroutine(StateName currentState)
    {
        if (_currentSpinState == SpinState.Spinning)
        {
            Debug.LogError($"Already spinning");
            yield break;
        }
        GameApiManager.Instance.SendSpinRequest(currentState);

        Audiomanager.Instance.PlaySfx(SFX.ReelSpin);
        _currentSpinState = SpinState.Spinning;
        WaitForSeconds waittime = new WaitForSeconds(ReelRotationOffset);

        foreach (Reel reel in reels)
        {
            reel.Spin();
            yield return waittime;
        }
    }

    public void StopSpinCoroutine(StateName currentState) => StopCoroutine(InitiateSpinCoroutine(currentState));

    public void SetReelAnticipationProperty(int column)
    {
        reels[column].anticipateReelForSpecialSymbol = true;
    }

    public void SetOutcomeSymbols()
    {
        for (int i = 0; i < reels.Count; i++)
        {
            reels[i].SetOutcomeSymbol(GameApiManager.Instance.ApiData.GetMatrix()[i]);
        }
    }

    public void SetReelsForAnticipation()
    {
        Dictionary<int, List<int>> matrix = GameApiManager.Instance.ApiData.GetMatrix();
        int column = 1;
        int scattercount = 0;
        int bonuscount = 0;

        foreach (List<int> rowData in matrix.Values)
        {
            foreach (int symbolId in rowData)
            {
                if (symbolId == SystemSetting.ScatterId)
                    scattercount++;
                else if (symbolId == SystemSetting.BonusId)
                    bonuscount++;
                if ((scattercount >= 2 || bonuscount >= 2) && column < ReelSetting.Columns)
                    SetReelAnticipationProperty(column);
            }
            column++;
        }
    }

    public IEnumerator StopReelsWithSpecialSymbol(Reel reel, float anticipationdelay)
    {
        IsWaitingForIndependentSpecialSymbol = true;
        #region Stop Forcefully
        if (SlotGameEngineStarter.TurboEnabled || SlotGameEngineStarter.IsSlamStop)
        {
            reel.HideAnticipationGlow();
            Audiomanager.Instance.StopSfx(SFX.anticipation);
            yield return new WaitForEndOfFrame();
        }
        #endregion

        #region Stop Normally
        else
        {
            reel.ShowAnticipationGlow();
            Audiomanager.Instance.PlaySfx(SFX.anticipation);
            Coroutine test = StartCoroutine(DelayAnticipation(anticipationdelay));
            yield return new WaitUntil(() => !IsWaitingForIndependentSpecialSymbol);
            StopCoroutine(test);
        }
        #endregion
    }

    public IEnumerator StopReelsWithoutSpecialSymbol(float reelstopldelay)
    {
        #region Stop Forcefully
        if (SlotGameEngineStarter.TurboEnabled || SlotGameEngineStarter.IsSlamStop)
        {
            yield return new WaitForEndOfFrame();
            Audiomanager.Instance.StopSfx(SFX.anticipation);
        }
        #endregion

        #region Stop Normally
        else
        {
            yield return new WaitForSeconds(reelstopldelay);
            Audiomanager.Instance.PlaySfx(SFX.ReelStop);
        }
        #endregion
    }

    public void StopAnimatingMidSymbol()
    {
        int midRow = (ReelSetting.Rows - 1) / 2;
        int midColumn = (ReelSetting.Columns - 1) / 2;
        Reels[midRow].OutcomeSymbols[midColumn].HideWin();
    }

    public void OnAllReelStop(Func<IEnumerator> ReelStopCallback)
    {
        if (SlotGameEngineStarter.TurboEnabled)
            Audiomanager.Instance.PlaySfx(SFX.ReelStop);
        /*if (SlotGameEngineStarter.IsSlamStop)
            Audiomanager.Instance.PlaySfx(SFX.ReelStop);*/
        _currentSpinState = SpinState.Idle;
        StartCoroutine(ReelStopCallback());
    }

    private IEnumerator DelayAnticipation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Audiomanager.Instance.PlaySfx(SFX.ReelStop);
        IsWaitingForIndependentSpecialSymbol = false;
    }

    public void SetConfig(float spinAnticipationDuration, float spinStopDelay, float reeloffset)
    {
        SpinAnticipationDuration = spinAnticipationDuration;
        SpinStopDelay = spinStopDelay;
        ReelRotationOffset = reeloffset;
    }
}
