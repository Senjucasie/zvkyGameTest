using TMPro;
using UnityEngine;

[System.Serializable]
public class PaytableFields
{
    [SerializeField] private TextMeshProUGUI paytable;
    [SerializeField] private int x5_creditMultiplier;
    [SerializeField] private int x4_creditMultiplier;
    [SerializeField] private int x3_creditMultiplier;

    public TextMeshProUGUI Paytable => paytable;
    public int X5_CreditMultiplier => x5_creditMultiplier;
    public int X4_CreditMultiplier => x4_creditMultiplier;
    public int X3_CreditMultiplier => x3_creditMultiplier;
}