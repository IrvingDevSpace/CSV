using CSV.Model.HeaderStateMachine;

namespace CSV.HeaderStateMachine
{
    public class FileNotExistState<T> : AHeaderState<T>
    {
        public FileNotExistState()
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
