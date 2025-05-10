using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSV
{
    public class CSVHelper
    {
        public static void Write<T>(String filePath, T data)
        {
            Write(filePath, new List<T>() { data });
            //FilePathWriteCheck(filePath);
            //CSVHeader.AddHeader(filePath, data);
            //PropertyInfo[] propertyInfos = data.GetType().GetProperties();
            //String content = "";
            //for (int i = 0; i < propertyInfos.Length; i++)
            //{
            //    if (i > 0)
            //        content += ",";
            //    content += propertyInfos[i].GetValue(data);
            //}

            //using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
            //    writer.WriteLine(content);
        }

        public static void Write<T>(String filePath, List<T> datas) where T : class, new()
        {
            FilePathWriteCheck(filePath);
            CSVHeader.AddHeader<T>(filePath);
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            var writer = new StreamWriter(filePath, true, Encoding.UTF8);

            // string pool => 字串池
            //StringBuilder sb = new StringBuilder();
            //foreach (var item in datas)
            //{
            //    String content = "";
            //    for (int i = 0; i < propertyInfos.Length; i++)
            //    {
            //        if (i > 0)
            //            content += ",";
            //        content += propertyInfos[i].GetValue(item);
            //    }
            //    sb.AppendLine(content);
            //}
            //using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
            //    writer.Write(sb.ToString());

            foreach (var item in datas)
            {
                String content = "";
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (i > 0)
                        content += ",";
                    content += propertyInfos[i].GetValue(item);
                }
                writer.WriteLine(content);
            }
            writer.Close();
        }

        public static List<T> Read<T>(String filePath) where T : class, new()
        {
            FilePathReadCheck(filePath);
            CSVHeader.CsvFileContent readContent = CSVHeader.CheckReadHeader<T>(filePath);
            List<T> list = new List<T>();
            using (var reader = new StreamReader(filePath))
            {
                int count = 0;
                while (!reader.EndOfStream)
                {
                    T t = new T();
                    String content = reader.ReadLine();
                    if (count > 0)
                    {
                        List<String> contents = content.Split(',').ToList();
                        PropertyInfo[] propertyInfos = t.GetType().GetProperties();
                        for (int i = 0; i < propertyInfos.Length; i++)
                        {
                            String displayName = propertyInfos[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                            if (displayName != null)
                            {
                                if (readContent.HeaderNameIndexDic.TryGetValue(displayName, out int index))
                                    propertyInfos[i].SetValue(t, contents[index]);
                            }
                            else
                            {
                                if (readContent.HeaderNameIndexDic.TryGetValue(propertyInfos[i].Name, out int index))
                                    propertyInfos[i].SetValue(t, contents[index]);
                            }
                        }
                        list.Add(t);
                    }
                    count++;
                }
            }
            return list;
        }

        public static List<T> Read<T>(CSVHeader.CsvFileContent csvContent, string filePath, int startIndex, int quantity) where T : class, new()
        {
            // 準備返回的結果列表
            var list = new List<T>();

            // 確保起始索引和數量的有效性
            if (startIndex < 0 || quantity <= 0)
                throw new ArgumentOutOfRangeException("startIndex 或 quantity 參數無效");

            using (var reader = new StreamReader(filePath))
            {
                int currentIndex = 0;
                string line;

                // 跳過第一行
                if (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    currentIndex++;
                }

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (currentIndex >= startIndex && currentIndex < startIndex + quantity)
                    {
                        var item = new T();
                        var properties = item.GetType().GetProperties();
                        var values = line.Split(',');

                        foreach (var property in properties)
                        {
                            var displayName = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                            var headerName = displayName ?? property.Name;

                            if (csvContent.HeaderNameIndexDic.TryGetValue(headerName, out int columnIndex) && columnIndex < values.Length)
                                property.SetValue(item, values[columnIndex]);
                        }

                        list.Add(item);

                        // 若達到指定數量則結束
                        if (list.Count >= quantity)
                            break;
                    }

                    currentIndex++;
                }
            }

            return list;
        }

        //public static void ProcessLargeCSV<T>(string inputFilePath, string outputFilePath, int totalRecords, int batchSize) where T : class, new()
        //{
        //    int currentIndex = 0;
        //    bool first = true;
        //    while (currentIndex < totalRecords)
        //    {
        //        int startIndex = currentIndex + 1; // 計算當前批次的起始索引
        //        int quantity = Math.Min(batchSize, totalRecords - currentIndex); // 確保最後一批數量正確

        //        // 讀取當前批次的資料
        //        List<T> records = Read<T>(first, inputFilePath, startIndex, quantity);

        //        // 寫入當前批次的資料
        //        Write(outputFilePath, records);

        //        currentIndex += batchSize;
        //        first = false;
        //    }
        //}

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

        public static void FilePathReadCheck(String filePath)
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
