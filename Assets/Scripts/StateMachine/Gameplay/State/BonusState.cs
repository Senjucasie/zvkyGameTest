namespace FSM.GamePlay.State
{
    public class BonusState : GamePlayBaseState
    {
        public BonusState(GamePlayStateMachine gameplaystatemachine) : base(gameplaystatemachine) 
        { Name = StateName.Bonus; }

        public override void Enter()
        {
            SlotGameEngineStarter.CurrentState = Name;
            EventManager.InvokeSwitchBaseButtonState(BaseButtonInteractionRegistry.Instance.bonusData, false);
            EventManager.InvokeBonusGame(_gamePlayStateMachine);
            _SubscribeEvents();
        }

        private void _SubscribeEvents()
        {
            EventManager.BonusSpinGameEndEvent += SwitchToNormalState;
        }

        private void SwitchToNormalState()
        {
            _gamePlayStateMachine.SwitchState(_gamePlayStateMachine.NormalGameState);
        }

        public override void Exit()
        {
            EventManager.InvokeResetWinData();
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            EventManager.BonusSpinGameEndEvent -= SwitchToNormalState;
        }
    }
}
