using CSV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String fileServerPath = "C:\\Users\\IRVING\\Program Course\\Code\\Accounting\\FileServer\\";
            AddAccountingInfo addAccountingInfo = new AddAccountingInfo
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd"),
                Value = "135153",
                Type = "飲食",
                Purpose = "午餐",
                Who = "自己",
                Payment = "現金",
                ImagePath1 = "C:\\Users\\IRVING\\Program Course\\Code\\Accounting\\FileServer\\86cee891-7003-4f8e-8204-a8a4ccf72c45_imgPath1.png",
                ImagePath2 = "C:\\Users\\IRVING\\Program Course\\Code\\Accounting\\FileServer\\2aa44ba5-0150-4aaa-8d88-db035c55a652_imgPath2.png"
            };

            CSVHelper.Write($@"C:\Users\IRVING\Desktop\Files\{fileServerPath}Data.csv", addAccountingInfo);
          //  List<AddAccountingInfo> list = CSVHelper.Read<AddAccountingInfo>($"{fileServerPath}Data.csv");
        }
    }
}
