using System;
using System.Collections.Generic;

namespace CSV
{
    public class CSVReadFileContent
    {
        public bool FileExist { get; set; } = true;
        public List<String> ContentLines { get; set; } = new List<String>();
        public List<String> HeaderNames { get; set; } = new List<String>();
    }
}
