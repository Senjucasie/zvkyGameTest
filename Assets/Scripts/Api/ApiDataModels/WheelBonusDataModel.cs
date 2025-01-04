using System.Collections.Generic;

[System.Serializable]
public class WheelBonusDataModel
{
    public bool status;
    public string message;
    public Data data;

    [System.Serializable]
    public class Data
    {
        public WheelSpin wheelSpin;
        public State state;
        public FreeGame freeGame;
        public string next;
        public double balance;
    }

    [System.Serializable]
    public class WheelSpin
    {
        public string reward;
        public double amount;
        public double totalBonusWin;
    }

    [System.Serializable]
    public class State
    {
        public string current;
    }

    [System.Serializable]
    public class FreeGame
    {

    }
}

