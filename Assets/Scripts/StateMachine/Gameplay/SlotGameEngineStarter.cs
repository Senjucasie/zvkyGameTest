using UnityEngine;

public class SlotGameEngineStarter : MonoBehaviour
{
    public SymbolPool SymbolPool;
    public ReelManager ReelManager;
    public GamePlayStateMachine GamePlayStateMachine;
    public static bool TurboEnabled { get; set; }
    public static bool IsTurboCached { get; set; }
    public static bool IsSlamStop { get; set; }
    public static StateName CurrentState { get; set; }

    private void Awake()
    {
        SymbolPool.CreateSymbolPool();
        ReelManager.Init();
        ReelManager.SetReels();
        GamePlayStateMachine.SetReelManager(this.ReelManager);
        GamePlayStateMachine.Init();
    }

    private void Start()
    {
        ReelManager.SetInitialConfig();
    }

}
