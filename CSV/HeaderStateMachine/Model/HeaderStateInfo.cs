using System;

namespace CSV.Model.HeaderStateMachine
{
    public class HeaderStateInfo<T>
    {
        public String FilePath { get; set; }
        public T Data { get; set; }
        public CSVReadFileContent CSVReadContent { get; set; }
    }
}
