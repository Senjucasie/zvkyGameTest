using System;
using System.Collections.Generic;
 
namespace Spin.Api
{
    [Serializable]
    public class SpinRequestData
    {
        public double credits;
        public double bet;
        public bool isBuyFeature;
    }

    public class ContinueRequestData
    {
        public string state;
    }

    [Serializable]
    public class State
    {
        public string current;
        public string next;
    }

    [Serializable]
    public class Payline
    {
        public int paylineNo;
        public int symbol;
        public int symbolCount;
        public int[] positions;
        public int multiplier;
        public double won;
    }

    [Serializable]
    public class ExpandingWildFeature
    {
        public Payline[] featurePaylines;
        public int[] expandingWild;
        public string[] expandingPosition;
    }

    [Serializable]
    public class Scatter
    {
        public int count;
        public string[] position;
        public bool freegametrigger;
        public bool freeGameRetrigger;
        public double creditWon;
        public int freeSpinWon;
    }

    [Serializable]
    public class WheelSpin
    {
        public string award;
        public double amount;
    }

    [Serializable]
    public class FreeGame
    {
        public int currentFreeSpin;
        public int totalFreeSpin;
        public int remainingFreeSpin;
        public double freeSpinCreditsWon;
        public double freeSpinTotalWon;
    }

    [Serializable]
    public class Bonus
    {
        public WheelSpin wheelSpin;
        public bool bonusTrigger;
        public int count;
        public string[] position;
        public double TotalBonusWin;
    }

    // NOTE: FOR TEST PURPOSE
    [Serializable]
    public class PickBonus
    {
        public int count = 3;
        public string[] position = {
            "0,0",
            "1,0",
            "2,0"
        };
        public bool bonusGametrigger = true;
        public int[] revealAmount = {
            1600,
            2400,
            800,
            400,
            1200,
            0
        };
        public int creditWon = 6400;
    }

    [Serializable]
    public class ReSpin
    {
        public int currentRespin;
        public int totalRespin;
        public double baseRespinCreditWon;
        public double baseRespinTotalWon;
    }

    [Serializable]
    public class FinalWinnings
    {
        public double TotalWon;
        public int net;
    }

    [Serializable]
    public class SpinDataModel
    {
        public bool success;
        public string message;
        public Data data;
    }

    [Serializable]
    public class Data
    {
        public double balance;
        public int? baseBet;
        public double creditValue;
        public int betMultiplier;
        public int reelMode;
        public State state;
        public int[] reelStops;
        public Dictionary<int, List<int>> matrix;
        public Payline[] payline;
        public ExpandingWildFeature expandingWildFeature;
        public Scatter scatter;
        public FreeGame freeGame;
        public Bonus bonus;
        public PickBonus pickBonus;
        public double mainSpinCreditsWon;
        public double creditsWonOnBaseSpin;
        public double totalCreditsWon;
        public int creditsWagered;
        public FinalWinnings finalWinnings;
        public string next;
        public ReSpin reSpin;
    }
}


 