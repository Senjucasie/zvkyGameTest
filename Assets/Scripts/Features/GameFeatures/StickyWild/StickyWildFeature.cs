using UnityEngine;

public class StickyWildFeature : GameFeatureAbstract
{
    [SerializeField] private StickyWildSymbol _stickyWildSymbol;

    public void ShowStickyWild()
    {
        _stickyWildSymbol.Show();
    }

    public void HideStickyWild()
    {
        _stickyWildSymbol.Hide();
    }

    public void MakeSpaceForStickyWild(Symbol symbol)
    {
        symbol.HideWin();
    }
}
