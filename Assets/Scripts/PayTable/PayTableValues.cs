using System.Collections.Generic;
using UnityEngine;

public class PayTableValues : MonoBehaviour
{
    [SerializeField] private List<PaytableFields> paytableFields = new();

    void OnEnable()
    {
        EventManager.UpdateCreditValueIndexEvent += UpdateCreditValue;
    }

    void OnDisable()
    {
        EventManager.UpdateCreditValueIndexEvent -= UpdateCreditValue;
    }

    public void UpdateCreditValue(int index)
    {
        foreach (var field in paytableFields)
        {
            if (field.Paytable != null)
            {
                float x5 = (float)System.Math.Round((field.X5_CreditMultiplier * GameConstants.creditValue[index]), 2);
                float x4 = (float)System.Math.Round((field.X4_CreditMultiplier * GameConstants.creditValue[index]), 2);
                float x3 = (float)System.Math.Round((field.X3_CreditMultiplier * GameConstants.creditValue[index]), 2);

                field.Paytable.text = $"{x5}\n" +
                                      $"{x4}\n" +
                                      $"{x3}\n";
            }
        }
    }
}
