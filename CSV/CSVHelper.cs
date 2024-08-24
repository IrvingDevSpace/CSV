using CSV.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    internal class CSVHelper
    {
        public static void Write<T>(String filePath, T data, bool shouldAddHeader)
        {
            FilePathWriteCheck(filePath);
            if (shouldAddHeader)
                CSVHeader.AddHeader(filePath, data);
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
            FilePathReadCheck(filePath);
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

        private static void FilePathWriteCheck(String filePath)
        {
            // 檢查是否為絕對路徑
            if (String.IsNullOrEmpty(filePath) || !Path.IsPathRooted(filePath))
                throw new Exception("Invalid file path!");

            // 檢查檔案副檔名是否為 CSV
            String fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension != ".csv")
                throw new Exception("File Extension Invalid!");

            // 檢查資料夾是否存在，如果不存在則建立
            String directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        private static void FilePathReadCheck(String filePath)
        {
            // 檢查是否為絕對路徑
            if (String.IsNullOrEmpty(filePath) || !Path.IsPathRooted(filePath))
                throw new Exception("Invalid file path!");

            // 檢查檔案是否存在
            if (!File.Exists(filePath))
                throw new Exception("File not found!");

            // 檢查檔案副檔名是否為 CSV
            String fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension != ".csv")
                throw new Exception("File Extension Invalid!");
        }
    }
}
