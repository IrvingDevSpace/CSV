using CSV.Model.HeaderStateMachine;

namespace CSV.HeaderStateMachine
{
    public class FinishState<T> : AHeaderState<T>
    {
        public FinishState()
        {
            NextState = null;
            IsEndState = true;
        }

        public override void Execute(HeaderStateInfo<T> headerStateInfo)
        {
            return;
        }
    }
}
