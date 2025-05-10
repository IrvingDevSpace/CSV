using CSV.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSV
{
    public class CSVHeader
    {
        public class CsvFileContent
        {
            public bool FileExist { get; set; } = true;
            public List<String> ContentLines { get; set; } = new List<String>();
            public List<String> HeaderNames { get; set; } = new List<String>();
            public Dictionary<String, int> HeaderNameIndexDic { get; set; } = new Dictionary<String, int>();
        }

        public static void AddHeader<T>(String filePath) where T : new()
        {
            CsvFileContent csvContent = GetCsvFileContent(filePath);
            HeaderTag headerTag = CheckHeaderStatus<T>(csvContent);
            switch (headerTag)
            {
                case HeaderTag.FileNotExist:
                case HeaderTag.HeaderInValid:
                    WriteHeaderAndContent<T>(filePath, csvContent);
                    break;
                case HeaderTag.HeaderValid:
                    break;
            }
        }

        public static CsvFileContent CheckReadHeader<T>(String filePath) where T : new()
        {
            CsvFileContent csvContent = GetHeaderContent(filePath);
            HeaderTag headerTag = CheckHeaderStatus<T>(csvContent);
            if (headerTag != HeaderTag.HeaderValid)
                throw new Exception("Header有誤，無法讀取");
            return csvContent;
        }

        private static CsvFileContent GetCsvFileContent(String filePath)
        {
            CsvFileContent csvContent = new CsvFileContent();
            if (!File.Exists(filePath))
            {
                csvContent.FileExist = false;
                return csvContent;
            }
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.Trim('\r');
                    if (!string.IsNullOrWhiteSpace(line))
                        csvContent.ContentLines.Add(line);
                }
                //csvReadContent.ContentLines = fileContent.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim('\r')).ToList();//\r\n
                csvContent.HeaderNames = csvContent.ContentLines.FirstOrDefault()?.Split(',').ToList() ?? new List<String>();
                csvContent.HeaderNameIndexDic = new Dictionary<String, int>();
                for (int i = 0; i < csvContent.HeaderNames.Count; i++)
                    csvContent.HeaderNameIndexDic.Add(csvContent.HeaderNames[i], i);
            }
            return csvContent;
        }

        private static CsvFileContent GetHeaderContent(String filePath)
        {
            CsvFileContent csvReadContent = new CsvFileContent();
            if (!File.Exists(filePath))
            {
                csvReadContent.FileExist = false;
                return csvReadContent;
            }
            using (var reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine()?.Trim('\r');
                csvReadContent.HeaderNames = line.Split(',').ToList() ?? new List<String>();
                csvReadContent.HeaderNameIndexDic = new Dictionary<String, int>();
                for (int i = 0; i < csvReadContent.HeaderNames.Count; i++)
                    csvReadContent.HeaderNameIndexDic.Add(csvReadContent.HeaderNames[i], i);
            }
            return csvReadContent;
        }

        private static HeaderTag CheckHeaderStatus<T>(CsvFileContent csvContent) where T : new()
        {
            if (!csvContent.FileExist)
                return HeaderTag.FileNotExist;
            if (IsValidHeaderName<T>(csvContent))
                return HeaderTag.HeaderValid;
            return HeaderTag.HeaderInValid;
        }

        private static bool IsValidHeaderName<T>(CsvFileContent csvContent) where T : new()
        {
            T t = new T();
            PropertyInfo[] propertyInfos = t.GetType().GetProperties();
            if (propertyInfos.Length != csvContent.HeaderNames.Count)
                return false;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                String headerName = csvContent.HeaderNames[i];
                if (String.IsNullOrEmpty(headerName))
                    return false;

                String displayName = propertyInfos[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

                if (String.IsNullOrEmpty(displayName))
                {
                    if (headerName != propertyInfos[i].Name)
                        return false;
                }
                else
                {
                    if (headerName != displayName && headerName != propertyInfos[i].Name)
                        return false;
                }
            }
            return true;
        }

        private static void WriteHeaderAndContent<T>(String filePath, CsvFileContent csvContent) where T : new()
        {
            T t = new T();
            String writeContent = "";
            String headerName = "";
            PropertyInfo[] propertyInfos = t.GetType().GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (i > 0)
                    headerName += ",";
                String colName = propertyInfos[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyInfos[i].Name;
                headerName += colName;
            }
            writeContent += headerName + Environment.NewLine;

            for (int i = 0; i < csvContent.ContentLines.Count; i++)
                writeContent += csvContent.ContentLines[i] + Environment.NewLine;

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                writer.Write(writeContent);
        }
    }
}
