using CSV.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    internal class CSVHelper
    {
        public static void Write<T>(String filePath, T data)
        {
            FilePathCheck(filePath);
            PropertyInfo[] propertyInfos = data.GetType().GetProperties();
            String content = "";
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (i > 0)
                    content += ",";
                content += propertyInfos[i].GetValue(data);
            }
            using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                writer.WriteLine(content);
        }

        public static List<T> Read<T>(String filePath) where T : class, new()
        {
            FilePathCheck(filePath);
            List<T> list = new List<T>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    T t = new T();
                    String content = reader.ReadLine();
                    List<String> contents = content.Split(',').ToList();
                    PropertyInfo[] propertyInfos = t.GetType().GetProperties();
                    for (int i = 0; i < propertyInfos.Length; i++)
                        propertyInfos[i].SetValue(t, contents[i]);
                    list.Add(t);
                }
            }
            return list;
        }

        private static void FilePathCheck(String filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Extension.ToLower() != "csv")
                throw new Exception("File Extension Invaild!");

            DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);

            if (!Directory.Exists(directoryInfo.FullName))
                directoryInfo.Create();

            //路徑 資料夾 檔名
        }
    }
}
