using System.Collections.Generic;
using UnityEngine;

public class PreExpandedWildFeature : GameFeatureAbstract
{
    [SerializeField] private PreExpandedWildSymbol _symbolPrefab;

    [Space (18)]
    [SerializeField] private bool _testExpandingWild;
    [SerializeField] private int[] _expandingWildTestData;
    public PreExpandedWildSymbol SymbolPrefab { get => _symbolPrefab; }

    public void SetPreExpandingWildProperty(List<Reel> reels)
    {
        foreach (Reel reel in reels)
        {
            reel.HasPreExpandingWild = false;
        }

        //int[] preExpandingWild = { 0, 1, 0, 0, 0 };
        int[] preExpandingWild = GameApiManager.Instance.ApiData.GetExpandingWild();

        //if (_testExpandingWild) preExpandingWild = _expandingWildTestData;   // for testing

        if (preExpandingWild.Length > 0)
            for (int i = 0; i < preExpandingWild.Length; i++)
                if (preExpandingWild[i] == 1) reels[i].HasPreExpandingWild = true;
    }

    public void PlayPreExpandedWild(PaylineController.PayLineState currentPaylineState)
    {
        foreach (Reel reel in ReelManager.Instance.Reels)
        {
            if (reel.HasPreExpandingWild)
            {
                reel.PreExpandingWildSymbol.ShowWin(currentPaylineState);
            }
        }
    }

    public void StopPreExpandedWild()
    {
        foreach (Reel reel in ReelManager.Instance.Reels)
        {
            if (reel.HasPreExpandingWild)
            {
                reel.PreExpandingWildSymbol.HideWin();
            }
        }
    }

}
