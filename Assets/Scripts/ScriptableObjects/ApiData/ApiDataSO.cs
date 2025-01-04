using SimpleJSON;
using Spin.Api;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApiData", menuName = "ScriptableObject/NewApiDataConfig")]

public class ApiDataSO : ScriptableObject
{
    [SerializeField] private SpinDataModel _spinParsedData;
    [SerializeField] private WheelBonusDataModel _wheelBonusData;

    public WheelBonusDataModel WheelBonusData { get => _wheelBonusData; set => _wheelBonusData = value; }
    public SpinDataModel SpinParsedData { get => _spinParsedData; set => _spinParsedData = value; }


    #region SPIN_DATA
    public void CreateSpinMatrix(JSONNode spinData)
    {
        SpinParsedData.data.matrix = new();

        for (int i = 0; i < 5; i++)
        {
            List<int> tempList = new List<int>();

            foreach (var item in spinData["data"]["matrix"][i])
            {
                int a = int.Parse(item.Value);
                tempList.Add(a);
            }
            SpinParsedData.data.matrix.Add(i, tempList);
        }
    }

    public void ClearSpinMatrix()
    {
        if(SpinParsedData.data.matrix != null)
            SpinParsedData.data.matrix.Clear();
    }

    public Dictionary<int, List<int>> GetMatrix()
    {
        return SpinParsedData.data.matrix;
    }

    public double GetSpinCredit()
    {
        return SpinParsedData.data.creditValue;
    }

    public Spin.Api.Payline[] GetPaylineData()
    {
        return SpinParsedData.data.payline;
    }

    public Spin.Api.Payline[] GetFeaturePaylineData()
    {
        return SpinParsedData.data.expandingWildFeature.featurePaylines;
    }

    public Spin.Api.Scatter GetScatterData()
    {
        return SpinParsedData.data.scatter;
    }

    public Spin.Api.Bonus GetBonusData()
    {
        return SpinParsedData.data.bonus;
    }

    public int GetFreeSpinCount(bool isResumeAvailable, bool isCompleted)
    {
        FreeGame freeGameData = SpinParsedData.data.freeGame;
        if (isResumeAvailable)
        {
            if (isCompleted)
                return freeGameData.totalFreeSpin - freeGameData.currentFreeSpin;
            else
                return freeGameData.totalFreeSpin - freeGameData.currentFreeSpin + 1;
        }
        else
            return freeGameData.totalFreeSpin - freeGameData.currentFreeSpin;
    }

    public int GetReSpinCount(bool isResumeAvailable, bool isCompleted)
    {
        int currentRespinData = UpdatedCurrentReSpinTestData(); //SpinParsedData.data.reSpin.currentRespin;
        int totalRespinData = SpinParsedData.data.reSpin.totalRespin;
        if (isResumeAvailable)
        {
            if (isCompleted)
                return totalRespinData - currentRespinData;
            else
                return totalRespinData - currentRespinData + 1;
        }
        else
            return totalRespinData - currentRespinData;
    }

    private int UpdatedCurrentReSpinTestData()
    {
        return ++SpinParsedData.data.reSpin.currentRespin;
    }

    public double GetFreeSpinCreditsWon()
    {
        return SpinParsedData.data.freeGame.freeSpinCreditsWon;
    }

    public double GetReSpinCreditsWon()
    {
        return SpinParsedData.data.reSpin.baseRespinCreditWon;
    }

    
    public double GetMainSpinCreditsWon()
    {
        return SpinParsedData.data.mainSpinCreditsWon;
    }

    public double GetFreeSpinAmoutWon()
    {
        return SpinParsedData.data.freeGame.freeSpinTotalWon;
    }
    public double GetReSpinAmoutWon()
    {
        return SpinParsedData.data.reSpin.baseRespinTotalWon;
    }
    #endregion

    #region BONUS
    public double GetWheelBonusWinAmount()
    {
        return _wheelBonusData.data.wheelSpin.amount;
    }

    public double GetWheelBonusFinalWinAmount()
    {
        return _wheelBonusData.data.wheelSpin.totalBonusWin;
    }

    public double GetWheelBonusWonBalance()
    {
        return _wheelBonusData.data.balance;
    }

    public string GetWheelBonusReward()
    {
        return _wheelBonusData.data.wheelSpin.reward;
    }

    public Spin.Api.PickBonus GetPickBonusData()
    {
        return SpinParsedData.data.pickBonus;
    }
    #endregion

    public int[] GetExpandingWild()
    {
        return SpinParsedData.data.expandingWildFeature.expandingWild;
    }

    public string[] GetExpandingWildPosition()
    {
        return SpinParsedData.data.expandingWildFeature.expandingPosition;
    }
}