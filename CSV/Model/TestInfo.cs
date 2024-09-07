using System;
using System.ComponentModel;

namespace CSV.Model
{
    public class TestInfo
    {
        [DisplayName("時間")]
        public String Time { get; set; }

        [DisplayName("金額")]
        public String Value { get; set; }

        [DisplayName("目的")]
        public String Purpose { get; set; }

        [DisplayName("付款方式")]
        public String Payment { get; set; }

        public String ImagePath1 { get; set; }
    }
}
