using CSV.Model.HeaderStateMachine;

namespace CSV.HeaderStateMachine
{
    public class HeaderInValidState<T> : AHeaderState<T>
    {
        public HeaderInValidState()
        {
            NextState = null;
            IsEndState = false;
        }

        public override void Execute(HeaderStateInfo<T> headerStateInfo)
        {
            WriteHeaderAndContent(headerStateInfo);
            NextState = new FinishState<T>();
        }
    }
}
