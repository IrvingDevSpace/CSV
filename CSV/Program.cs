using CSV.Model;
using System;
using System.Collections.Generic;

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
                Amount = DateTime.Now.ToString("hhmmssff"),
                Type = "交通費",
                Purpose = "出差",
                Who = "自己",
                Payment = "LinePay",
                ImagePath1 = "C:\\Users\\IRVING\\Program Course\\Code\\Accounting\\FileServer\\86cee891-7003-4f8e-8204-a8a4ccf72c45_imgPath1.png",
                ImagePath2 = "C:\\Users\\IRVING\\Program Course\\Code\\Accounting\\FileServer\\2aa44ba5-0150-4aaa-8d88-db035c55a652_imgPath2.png"
            };

            CSVHelper.Write($@"C:\Users\IRVING\Desktop\Filesss\Dazataa.csv", addAccountingInfo);
            //List<AddAccountingInfo> list = CSVHelper.Read<AddAccountingInfo>($"{fileServerPath}Data.csv");
            List<AddAccountingInfo> list = CSVHelper.Read<AddAccountingInfo>(@"C:\Users\IRVING\Desktop\Filesss\Dazataa.csv");
        }
    }
}
