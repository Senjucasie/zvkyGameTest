using FSM;
using FSM.GamePlay.State;
using UnityEngine;

public class GamePlayStateMachine : StateMachine

{
    [field: SerializeField] public ReelManager ReelManager { get; private set; }

    [field: Space(12)]
    [field: SerializeField] public PaylineController PaylineController { get; private set; }

    #region states
    public State NormalGameState { get; private set; }
    public State ScatterGameState { get; private set; }
    public State AutoPlayGameState { get; private set; }
    public State BonusGameState { get; private set; }
    #endregion

    void OnEnable()
    {
        EventManager.OnTurboClickedEvent += OnTurboMode;
    }

    public void SetReelManager(ReelManager reelManager) => this.ReelManager = reelManager;


    public void Init()
    {
        NormalGameState = new NormalState(this);
        ScatterGameState = new ScatterState(this);
        AutoPlayGameState = new AutoPlayState(this);
        BonusGameState = new BonusState(this);
    }

    private void Start()
    {
        SwitchState(NormalGameState);
    }

    private void OnTurboMode(bool enable)
    {
        SlotGameEngineStarter.TurboEnabled = enable;
        if (SlotGameEngineStarter.TurboEnabled)
            SetTurboModeData();
        else
            SetNormalSpinData();
    }

    private void SetTurboModeData() => ReelManager.SetConfig(ReelManager.ReelSetting.TurboSpinAnticipationDuration,
                                                             ReelManager.ReelSetting.TurboSpinStopDelay,
                                                             ReelManager.ReelSetting.TurboReelRotationOffset);

    private void SetNormalSpinData()
    {
        ReelManager.SetConfig(ReelManager.ReelSetting.SpinAnticipationDuration,
                              ReelManager.ReelSetting.SpinStopDelay,
                              ReelManager.ReelSetting.ReelRotationOffset);
        foreach (Reel reel in ReelManager.Reels)
        {
            reel.ConfigureReel(ReelManager.ReelSetting, ReelManager.SystemSetting, false);
        }
    }


    void OnDisable()
    {
        EventManager.OnTurboClickedEvent -= OnTurboMode;
    }


}