using FSM;

public abstract class GamePlayBaseState : State
{
    public StateName Name;
    protected GamePlayStateMachine _gamePlayStateMachine;

    public GamePlayBaseState(GamePlayStateMachine gameplaystatemachine)
    {
        _gamePlayStateMachine= gameplaystatemachine;
    }
}
