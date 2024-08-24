using CSV.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    public class CSVHeader
    {
        private class CSVReadFileContent
        {
            public bool FileExist { get; set; } = true;
            public List<String> ContentLines { get; set; } = new List<String>();
            public List<String> HeaderNames { get; set; } = new List<String>();
        }

        public static void AddHeader<T>(String filePath, T data)
        {
            CSVReadFileContent csvReadContent = GetCSVReadFileContent(filePath);
            HeaderTag headerTag = CheckHeaderStatus(data, csvReadContent);
            switch (headerTag)
            {
                case HeaderTag.FileNotExist:
                    break;
                case HeaderTag.HeaderInValid:
                    WriteHeaderAndContent(filePath, data, csvReadContent);
                    break;
                case HeaderTag.HeaderValid:
                    break;
            }
        }

        private static CSVReadFileContent GetCSVReadFileContent(String filePath)
        {
            CSVReadFileContent csvReadContent = new CSVReadFileContent();
            if (!File.Exists(filePath))
            {
                csvReadContent.FileExist = false;
                return csvReadContent;
            }
            using (var reader = new StreamReader(filePath))
            {
                String fileContent = reader.ReadToEnd();
                csvReadContent.ContentLines = fileContent.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim('\r')).ToList();//\r\n
                csvReadContent.HeaderNames = csvReadContent.ContentLines.FirstOrDefault()?.Split(',').ToList() ?? new List<String>();
            }
            return csvReadContent;
        }

        private static HeaderTag CheckHeaderStatus<T>(T data, CSVReadFileContent csvReadContent)
        {
            if (!csvReadContent.FileExist)
                return HeaderTag.FileNotExist;
            if (IsValidHeaderName(data, csvReadContent))
                return HeaderTag.HeaderValid;
            return HeaderTag.HeaderInValid;
        }

        private static bool IsValidHeaderName<T>(T data, CSVReadFileContent csvReadContent)
        {
            PropertyInfo[] propertyInfos = data.GetType().GetProperties();
            if (propertyInfos.Length != csvReadContent.HeaderNames.Count)
                return false;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                String headerName = csvReadContent.HeaderNames[i];
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

        private static void WriteHeaderAndContent<T>(String filePath, T data, CSVReadFileContent csvReadContent)
        {
            String writeContent = "";
            String headerName = "";
            PropertyInfo[] propertyInfos = data.GetType().GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (i > 0)
                    headerName += ",";
                String colName = propertyInfos[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyInfos[i].Name;
                headerName += colName;
            }
            writeContent += headerName + Environment.NewLine;

            for (int i = 0; i < csvReadContent.ContentLines.Count; i++)
                writeContent += csvReadContent.ContentLines[i] + Environment.NewLine;

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                writer.Write(writeContent);
        }
    }
}
