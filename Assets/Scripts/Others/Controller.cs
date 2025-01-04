using UnityEngine;

public class Controller : MonoBehaviour
{
    private int remainingAutoSpins;

    public int RemainingAutoSpins { get => remainingAutoSpins; }

    [SerializeField]private GamePlayStateMachine _gamePlayStateMachine;

    public static Controller Instance;

    private void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        EventManager.FreeSpinOnLoginEvent += OnFreeSpinOnLogin;
        EventManager.OnSpinClickedEvent += OnSpinClicked;
        EventManager.OnScatterPaylineStopped += OnScatterPaylineStopped;
  
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

    void OnDisable()
    {
        EventManager.FreeSpinOnLoginEvent -= OnFreeSpinOnLogin;
        EventManager.OnSpinClickedEvent -= OnSpinClicked;
        EventManager.OnScatterPaylineStopped -= OnScatterPaylineStopped;

    }
}
