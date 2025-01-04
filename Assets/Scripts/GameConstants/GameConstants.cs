using System.Collections.Generic;

public sealed class GameConstants
{
    public static List<double> creditValue = new List<double> { 0.1, 0.2, 0.5, 0.7, 1, 2, 3, 5, 8, 10, 15, 20 };

    public sealed class ApiCurrntState
    {
        public const string Base = "BASE";
        public const string Free = "FREE";
        public const string WheelBonus = "WHEEL_BONUS";
        public const string PickBonus = "PICK_BONUS";
        public const string ReSpin = "RESPIN";
    }

    public sealed class SpecialSymbolIDs
    {
        public const int PreExpandedWild = 999;
    }
}