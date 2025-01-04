using UnityEngine;

public class Controller : MonoBehaviour
{
    private int remainingAutoSpins;

    public int RemainingAutoSpins { get => remainingAutoSpins; }

    [SerializeField]private GamePlayStateMachine _gamePlayStateMachine;

    private void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        EventManager.FreeSpinOnLoginEvent += OnFreeSpinOnLogin;
        EventManager.BonusOnLoginEvent += OnBonusOnLogin;
        EventManager.OnSpinClickedEvent += OnSpinClicked;
        EventManager.OnScatterPaylineStopped += OnScatterPaylineStopped;
        EventManager.OnBonusPaylineStopped += OnBonusPaylineStopped; 
        EventManager.OnAutoSpinPlayEvent += OnAutoPlayPressed;
        EventManager.OnAutoSpinStopEvent += OnAutoPlayStopPressed;
    }


    public void OnSpinClicked()
    {
        currentGameState = GameStatesType.NormalSpin;
        currentSpinState = SpinStatesTypes.Spinning;
        EventManager.InvokeSpinButton();
     
        EconomyManager.OnUpdateCurrentBalance();
    }

    private void OnFreeSpinOnLogin()
    {
        GameController.ActiveFreeGame();
    }

    public void OnScatterPaylineStopped()
    {
        GameController.ActiveFreeGame();
    }

    public void OnBonusOnLogin()
    {
        EventManager.InvokeBonusStateEvent();
        //GameController.ActiveBonusGame();
    }

    public void OnBonusPaylineStopped()
    {
        EventManager.InvokeBonusStateEvent();
        //GameController.ActiveBonusGame();
    }

  

    public void OnFreeSpinPlayPressed()
    {
        if (currentGameState != GameStatesType.AutoSpin)
        {
            if (CurrentSpinState == SpinStatesTypes.Idle)
            {
                Debug.Log("OnFreeSpinPlayPressed ");
                EventManager.InvokeSpinButton();
                EconomyManager.OnUpdateCurrentBalance();
            }
        }
    }
    public void OnAutoPlayPressed(int autoSpinCount)
    {
        //if (currentGameState != GameStatesType.FreeSpin)
        //{
        //    if (autoSpinCount > 0) remainingAutoSpins = autoSpinCount;

        //    if (CurrentSpinState == SpinStatesTypes.Idle)
        //    {
        //        if (remainingAutoSpins > 0)
        //        {
        //            Debug.Log("Individual Auto Spin Started!");
        //            currentGameState = GameStatesType.AutoSpin;
        //            Debug.Log("OnSpinAutoPlayPressed");
        //            EventManager.SpinButton();
        //            EconomyManager.OnUpdateCurrentBalance();
        //            EventManager.OnAutoSpinPlayed();
        //            this.remainingAutoSpins--;
        //            Debug.Log("Individual Auto Spin Stopped!");
        //        }
        //        else
        //        {
        //            CurrentGameState = GameStatesType.NormalSpin;
        //            EventManager.OnAutoSpinStop();
        //            EventManager.SetNormalSpinData();
        //        }
        //    }
        //}
    }

    public void OnAutoPlayStopPressed()
    {
        Debug.Log("Auto spin stopped");
        remainingAutoSpins = 0;
    }

    public enum GameStatesType
    {
        NormalSpin = 0,
        Idle,
        AutoSpin,
        FreeSpin,
        BonusGame
    }

    public enum SpinStatesTypes
    {
        Idle = 0,
        Spinning,
        AutoSpinning,
    }

    SpinStatesTypes currentSpinState;
    public SpinStatesTypes CurrentSpinState
    {
        get { return currentSpinState; }
        set
        {
            Debug.Log("Spin State set Value = >  " + value);
            currentSpinState = value;
        }
    }

    GameStatesType currentGameState;
    public GameStatesType CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            Debug.Log("Spin State set Value = >  " + value);
            currentGameState = value;
        }
    }


    public static Controller Instance;



    void OnDisable()
    {
        EventManager.FreeSpinOnLoginEvent -= OnFreeSpinOnLogin;
        EventManager.BonusOnLoginEvent -= OnBonusOnLogin;
        EventManager.OnSpinClickedEvent -= OnSpinClicked;
        EventManager.OnScatterPaylineStopped -= OnScatterPaylineStopped;
        EventManager.OnBonusPaylineStopped -= OnBonusPaylineStopped;
        EventManager.OnAutoSpinPlayEvent -= OnAutoPlayPressed;
        EventManager.OnAutoSpinStopEvent -= OnAutoPlayStopPressed;
    }
}
