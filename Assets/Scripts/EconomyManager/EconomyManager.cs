using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static void OnUpdateCurrentBalance()
    {
        double SetInitialBalance = 100000;
        EventManager.InvokeUpdateBalanceAmount(SetInitialBalance);
    }

    public static bool HasSufficientBalance()
    {
        return true;
        //double currentBalance = GameApiManager.Instance.CurrentBalance;
        //BetManager betManager = BetManager.Instance;
        //return currentBalance >= GameConstants.creditValue[betManager.BetIndex] * betManager.Bet;
    }
}
