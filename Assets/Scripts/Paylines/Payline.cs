using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace SlotMachineEngine
{
    [Serializable]
    public class Payline
    {
        public int id;
        public int[] symbolIds;
        public int symbolShowCount;
        public double won;

        private Payline() { }

        public Payline(int id, int[] symbolIds, int symbolShowCount, double won)
        {
            this.id = id;
            this.symbolIds = new int[symbolIds.Length];
            this.symbolIds = symbolIds;
            this.symbolShowCount = symbolShowCount;
            this.won = won * GameApiManager.Instance.ApiData.GetSpinCredit();
        }
    }

    [Serializable]
    public class WaysPayline
    {
        public int id;
        public List<KeyValuePair<int, int>> positions;
        public int symbolShowCount;
        public double won;

        private WaysPayline() { }

        public WaysPayline(int id, List<KeyValuePair<int, int>> positions, int symbolShowCount, int won)
        {
            this.id = id;
            this.positions = positions;
            this.symbolShowCount = symbolShowCount;
            this.won = won * GameApiManager.Instance.ApiData.GetSpinCredit();
        }
    }

    [Serializable]
    public class SpecialPayline
    {
        public int[,] positions;
        private SpecialPayline() { }
        public SpecialPayline(string[] positions)
        {
            this.positions = new int[positions.Length, 2];

            for (int i = 0; i < positions.Length; i++)
            {
                string[] temp = positions[i].Split(',');
                this.positions[i, 0] = int.Parse(temp[0]);
                this.positions[i, 1] = int.Parse(temp[1]);
            }
        }
    }
}