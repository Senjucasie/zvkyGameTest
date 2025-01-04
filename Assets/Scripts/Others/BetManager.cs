using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance;
    internal double CurrentBet = 0;
    internal int BetIndex = 4;
    internal double Bet = 30;   //DiamondPanther: 40, Buffalo: 30

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        EventManager.SetPlayerDataEvent += UpdateCurrentBet;
        EventManager.UpdateCreditValueIndexEvent += UpdateBetIndex;
    }

    public void UpdateCurrentBet(double bet)
    {
        CurrentBet = bet;
    }

    private void UpdateBetIndex(int betIndex)
    {
        this.BetIndex = betIndex;
    }

    private void OnDisable()
    {
        EventManager.SetPlayerDataEvent -= UpdateCurrentBet;
        EventManager.UpdateCreditValueIndexEvent -= UpdateBetIndex;
    }
}
