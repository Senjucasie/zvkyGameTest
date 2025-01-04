using System;
using UnityEngine;

[Serializable]
public class BonusRegistry
{
    [SerializeField] private AbstractBonus _bonusFeature;
    [SerializeField] private bool _isActive;

    public bool IsActive { get => _isActive; }
    public AbstractBonus BonusFeature { get => _bonusFeature; }
}
