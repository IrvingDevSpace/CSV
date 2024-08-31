using CSV.Model.HeaderStateMachine;

namespace CSV.HeaderStateMachine
{
    public class FileExistState<T> : AHeaderState<T>
    {
        public FileExistState()
        {
            NextState = null;
            IsEndState = false;
        }

        public override void Execute(HeaderStateInfo<T> headerStateInfo)
        {
            if (IsValidHeaderName(headerStateInfo))
                NextState = new FinishState<T>();
            else
                NextState = new HeaderInValidState<T>();
        }
    }
}
