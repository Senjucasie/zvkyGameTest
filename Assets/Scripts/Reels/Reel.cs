using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Reel : MonoBehaviour
{
    [SerializeField] public BlurredStrip blurredStrip;
    [SerializeField] public GameObject outcomeStrip;
    [SerializeField] private GameObject anticipationGlowNode;
    [SerializeField] private int baseSpineSortingId = 12;
    [HideInInspector] public bool isSpinning = false;
    [HideInInspector] public bool anticipateReelForSpecialSymbol;

    #region Internals
    private ReelScriptableObject _reelSetting;
    private SystemScriptableObject _systemSetting;
    private Vector3 _outcomeStripPosn = new();
    private List<Symbol> _outcomeSymbols = new();
    private Coroutine _spinCoroutine;
    private float _reelTweenEndDuration;
    #endregion internals

    #region Properties
    public List<Symbol> OutcomeSymbols { get => _outcomeSymbols; }
    #endregion Properties

    void Start()
    {
        _outcomeStripPosn = outcomeStrip.transform.localPosition;
    }

    public void ConfigureReel(ReelScriptableObject reelsetting, SystemScriptableObject systemsetting, bool setRandomSymbol = true)
    {
        this._reelSetting = reelsetting;
        this._systemSetting = systemsetting;
        if (setRandomSymbol) SetRandomOutcomeSymbols();
    }

    public void SetRandomOutcomeSymbols()
    {
        for (int i = 0; i < _reelSetting.Rows + 2; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, SymbolPool.Instance.poolDictionary.Count);
            SymbolPool.Instance.GetObject(randomIndex, outcomeStrip.transform);
        }
    }

    public void ClearOutcomeStrip()
    {
        Symbol[] symbols = outcomeStrip.GetComponentsInChildren<Symbol>();
        foreach (Symbol child in symbols)
            SymbolPool.Instance.ReturnObject(child);

        foreach (Transform child in outcomeStrip.transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void SetOutcomeSymbol(List<int> symbolIds)
    {
        // TODO: If not HasExpandingWild then the functionalities related to basic game will be invoked.
        // Else the instatiation for expanding wild will happen via another method that is there in Expanding Wild class
        ClearOutcomeStrip();
        _outcomeSymbols.Clear();
        int fillerCount = 2; // Recommended = 2, to avoid showing blank space

        InstantiateRandomFiller(fillerCount);
        int sortingOffset = 1;
            foreach (int symbolId in symbolIds)
            {
                Symbol symbol = SymbolPool.Instance.GetObject(symbolId, outcomeStrip.transform).GetComponent<Symbol>();
                symbol.SetSpineSortingOrder(baseSpineSortingId + sortingOffset);
                _outcomeSymbols.Add(symbol);

                sortingOffset++;
            }
        
      

        InstantiateRandomFiller(fillerCount);
    }

    private void InstantiateRandomFiller(int fillerCount)
    {
        for (int i = 0; i < fillerCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, SymbolPool.Instance.poolDictionary.Count);
            SymbolPool.Instance.GetObject(randomIndex, outcomeStrip.transform);
        }
    }

    public void Spin()
    {
        anticipateReelForSpecialSymbol = false;
        _spinCoroutine = StartCoroutine(SpinRoutine());
    }

    public void StopSpinRoutine()
    {
        if (_spinCoroutine != null)
        {
            StopCoroutine(_spinCoroutine);
        }
    }

    private IEnumerator SpinRoutine()
    {
        // Anticipate reel rotation
        if (_reelSetting.AnticipateRotation)
        {
            float anticipationElapsedTime = 0;
            float anticipationSpeed = _reelSetting.RotationSpeed / 4;
            {
                while (anticipationElapsedTime < _reelSetting.SpinAnticipationDuration)
                {
                    float t = anticipationElapsedTime / _reelSetting.SpinAnticipationDuration;
                    float variedSpeed = Mathf.Lerp(0, anticipationSpeed, t);
                    outcomeStrip.transform.Translate(Vector3.up * (anticipationSpeed - variedSpeed) * Time.deltaTime);
                    anticipationElapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }



        float newPositionY = outcomeStrip.transform.localPosition.y - 1550;
        outcomeStrip.SetActive(false);
        outcomeStrip.transform.DOLocalMoveY(newPositionY, 0.05f);

        isSpinning = true;
        blurredStrip.gameObject.SetActive(true);
        blurredStrip.StartSpinIllusion(_reelSetting.RotationSpeed);
    }

    public void ShowAnticipationGlow() => anticipationGlowNode.SetActive(true);
    public void HideAnticipationGlow() => anticipationGlowNode.SetActive(false);

    public void SpinStop(Action callback)
    {
        outcomeStrip.SetActive(true);
        disableBlur();
        HideAnticipationGlow();
        if (SlotGameEngineStarter.IsSlamStop)
        {
            _reelTweenEndDuration = 0.1f;
            outcomeStrip.transform.localPosition = new(_outcomeStripPosn.x, _outcomeStripPosn.y, _outcomeStripPosn.z);
        }
        else
        {
            _reelTweenEndDuration = 0.3f;
            outcomeStrip.transform.localPosition = new(_outcomeStripPosn.x, _outcomeStripPosn.y + 400, _outcomeStripPosn.z); // value: 400 is for setting the offset of the outcome strip's local position. 
        }

        outcomeStrip.transform.DOLocalMoveY(_outcomeStripPosn.y, _reelTweenEndDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => { callback(); });
    }

    private void disableBlur()
    {
        blurredStrip.gameObject.SetActive(false);
        isSpinning = false;
    }
}
