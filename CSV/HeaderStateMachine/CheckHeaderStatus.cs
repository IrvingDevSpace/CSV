using CSV.Model.HeaderStateMachine;

namespace CSV.HeaderStateMachine
{
    public class CheckHeaderStatus<T>
    {
        private AHeaderState<T> currentState;
        private HeaderStateInfo<T> headerStateInfo;

        public CheckHeaderStatus(AHeaderState<T> state, HeaderStateInfo<T> headerStateInfo)
        {
            currentState = state;
            this.headerStateInfo = headerStateInfo;
        }

        public bool IsEndState => currentState.IsEndState;

        public void Check()
        {
            currentState.Execute(headerStateInfo);
            currentState = currentState.NextState;
        }
    }
}
