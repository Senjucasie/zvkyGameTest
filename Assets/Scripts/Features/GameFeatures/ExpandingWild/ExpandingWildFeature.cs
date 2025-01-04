using System.Collections.Generic;
using UnityEngine;

public class ExpandingWildFeature : GameFeatureAbstract
{
    [SerializeField] private ExpandingWildSymbol _symbolPrefab;

    public ExpandingWildSymbol SymbolPrefab { get => _symbolPrefab; }

    public void SetExpandingWildProperty(List<Reel> reels)
    {
        foreach (Reel reel in reels)
        {
            reel.HasExpandingWild = false;
        }

        //int[] preExpandingWild = { 0, 1, 0, 0, 0 };
        int[] expandingWild = GameApiManager.Instance.ApiData.GetExpandingWild();
        string[] expandingPosition = GameApiManager.Instance.ApiData.GetExpandingWildPosition();

        //if (_testExpandingWild) preExpandingWild = _expandingWildTestData;   // for testing

        if (expandingWild.Length > 0)
        {
            for (int i = 0; i < expandingWild.Length; i++)
            {
                if (expandingWild[i] == 1)
                {
                    reels[i].HasExpandingWild = true;
                    if (expandingPosition.Length > 0)
                        reels[i].ExpandingPosition = int.Parse(expandingPosition[i][^1].ToString());       // char to int
                }
            }
        }
    }

    public void PlayExpandingWild()
    {
        foreach (Reel reel in ReelManager.Instance.Reels)
        {
            if (reel.HasExpandingWild)
            {
                //Audiomanager.Instance.PlaySfx(SFX.WildLand);
                reel.ExpandingWildSymbol.ShowWin((ExpandingPosition)reel.ExpandingPosition);
            }
        }
    }
}
