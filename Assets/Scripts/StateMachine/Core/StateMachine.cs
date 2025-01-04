using FSM;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State _lastState;

    public void SwitchState(State currentstate)
    {
        _lastState?.Exit();

        currentstate?.Enter(); 
        _lastState = currentstate;
    }
}
