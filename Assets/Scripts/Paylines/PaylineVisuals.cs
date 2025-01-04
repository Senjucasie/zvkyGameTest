using System;
using System.Collections.Generic;
using CusTween;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PaylineVisuals : MonoBehaviour
{
    [SerializeField] private GameObject[] paylines;
    [SerializeField] private GameObject _paylineAmountPrefab;

    [Header("Total Win Panel")]
    [SerializeField] private GameObject TotalWinPanel;
    [SerializeField] private MultipleFontHandler totalWinText;
    [SerializeField] float _normalAnimationDuration = 0.5f;
    [SerializeField] float _fastAnimationDuration = 0.2f;

    private const int _maxPayLine = 2;
    private const int _defaultKeyForTextPosition = 1;// it is linked with the _textTranform list and if it is changed this sould be updated too
    [SerializeField] private List<Transform> _textTransform;// dont change the order of the tranform it is from top to bottom to show the win text
    private Dictionary<int, Transform> _paylineTextTransforms;

    private Dictionary<int, GameObject> normalPaylineAmounts;
    private Dictionary<int, GameObject> featurePaylineAmounts;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _paylineTextTransforms = new();
        normalPaylineAmounts = new();
        featurePaylineAmounts = new();
        PopulatePaylineTransform();
    }
    private void PopulatePaylineTransform()
    {
        for (int i = 0; i <= _maxPayLine; i++)
        {
            _paylineTextTransforms.Add(i, _textTransform[i]);
        }
    }

    void OnValidate()
    {
        HidePayline();
    }

    public void Reset()
    {
        CancelInvoke(nameof(HideTotalWin));
        HidePayline();
        HideTotalWin();

        foreach (var paylineAmount in normalPaylineAmounts)
            Destroy(paylineAmount.Value);
        normalPaylineAmounts.Clear();
        foreach (var paylineAmount in featurePaylineAmounts)
            Destroy(paylineAmount.Value);
        featurePaylineAmounts.Clear();
    }

    public void InstantiateWinAmount(PaylineType paylineType, int paylineid, int position, double winamount)
    {
        GameObject text = Instantiate(_paylineAmountPrefab, GetWinAmountTransform(position));
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;

        TextMeshProUGUI tmProGui = text.GetComponent<TextMeshProUGUI>();

        #region FormatText
        if (winamount % 1 == 0)
        {
            tmProGui.SetText(winamount.ToString("F0"));
        }
        else
        {
            tmProGui.SetText(winamount.ToString("F2"));
        }
        #endregion

        if (paylineType == PaylineType.Normal)
            normalPaylineAmounts.Add(paylineid, text);
        else if (paylineType == PaylineType.Feature)
            featurePaylineAmounts.Add(paylineid, text);

        text.SetActive(false);
    }

    private Transform GetWinAmountTransform(int position)
    {
        if (_paylineTextTransforms.ContainsKey(position))
            return _paylineTextTransforms[position];
        else
            return _paylineTextTransforms[_defaultKeyForTextPosition];
    }

    public void ShowPayline(int paylineId)
    {
        paylines[paylineId].SetActive(true);
        Audiomanager.Instance.PlaySfx(SFX.Payline);
    }

    public void HidePayline()
    {
        foreach (GameObject payline in paylines)
            payline.SetActive(false);
    }

    public void HidePayline(int id)
    {
        paylines[id].SetActive(false);
    }

    public void ShowPaylineAmount(PaylineType paylieType, int paylineId)
    {
        if (paylieType == PaylineType.Normal)
            normalPaylineAmounts[paylineId].SetActive(true);
        else if (paylieType == PaylineType.Feature)
            featurePaylineAmounts[paylineId].SetActive(true);
    }

    public void HidePaylineAmount()
    {
        foreach (KeyValuePair<int, GameObject> normalPaylineAmount in normalPaylineAmounts)
            normalPaylineAmount.Value.SetActive(false);
        foreach (KeyValuePair<int, GameObject> featurePaylineAmount in featurePaylineAmounts)
            featurePaylineAmount.Value.SetActive(false);
    }

    public void ShowTotalWin(double amt, int paylineCount)
    {
        RectTransform rt = TotalWinPanel.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        TotalWinPanel.SetActive(true);

        if (paylineCount > 3)
        {
            if (amt % 1 == 0)
            {
                totalWinText.setText((Int32)amt);
            }
            else
            {
                totalWinText.setText((float)amt);
            }

            rt.DOScale(1f, _normalAnimationDuration)
                        .SetEase(Ease.OutBack);
        }
        else
        {
            totalWinText.setStringText(amt.ToString());

            rt.DOScale(1f, _fastAnimationDuration)
                        .SetEase(Ease.OutBack);
        }

        Invoke(nameof(HideTotalWin), 2f);
    }

    private void HideTotalWin()
    {
        RectTransform rt = TotalWinPanel.GetComponent<RectTransform>();
        rt.DOScale(0f, _normalAnimationDuration / 2).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    totalWinText.setText(0);
                    TotalWinPanel.SetActive(false);
                });
    }
}
