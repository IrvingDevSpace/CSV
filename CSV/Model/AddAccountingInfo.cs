using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV.Model
{
    public class AddAccountingInfo
    {
        [DisplayName("時間")]
        public String Time { get; set; }

        [DisplayName("金額")]
        public String Value { get; set; }

        [DisplayName("類型")]
        public String Type { get; set; }

        [DisplayName("目的")]
        public String Purpose { get; set; }

        [DisplayName("對象")]
        public String Who { get; set; }

        [DisplayName("付款方式")]
        public String Payment { get; set; }

        //[DisplayName("")]
        public String ImagePath1 { get; set; }

        [DisplayName("發票圖檔2")]
        public String ImagePath2 { get; set; }
    }
}
