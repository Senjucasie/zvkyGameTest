using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameFeaturesManager : MonoBehaviour
{
    // INSTANCE
    public static GameFeaturesManager Instance;

    // PRIVATE VALUES
    [SerializeField] private PreExpandedWildFeature _preExpandingWildFeature;
    [SerializeField] private bool _hasPreExpandedWild;

    [SerializeField] private ExpandingWildFeature _expandingWildFeature;
    [SerializeField] private bool _hasExpandingWild;

    [SerializeField] private StickyWildFeature _stickyWildFeature;
    [SerializeField] private bool _hasStickyWild;

    // PUBLIC VALUES
    public PreExpandedWildFeature PreExpandingWildFeature { get => _preExpandingWildFeature; }
    public bool HasPreExpandedWild { get => _hasPreExpandedWild; }
    public ExpandingWildFeature ExpandingWildFeature { get => _expandingWildFeature; }
    public bool HasExpandingWild { get => _hasExpandingWild; }
    public StickyWildFeature StickyWildFeature { get => _stickyWildFeature; }
    public bool HasStickyWild { get => _hasStickyWild; }

    private void Awake()
    {
        Instance = this;
    }

    //private void Start()
    //{
    //    _activeGameFeature = _gameFeaturesList.FirstOrDefault(feature => feature.IsActive)
    //        .GameFeature;
    //}
}
