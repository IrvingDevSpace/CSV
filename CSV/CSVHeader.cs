using CSV.HeaderStateMachine;
using CSV.Model.HeaderStateMachine;
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
        public static void AddHeader<T>(String filePath, T data)
        {
            CSVReadFileContent csvReadContent = GetCSVReadFileContent(filePath);
            AHeaderState<T> headerState = GetHeaderStatus<T>(csvReadContent);
            HeaderStateInfo<T> headerStateInfo = new HeaderStateInfo<T>
            {
                FilePath = filePath,
                Data = data,
                CSVReadContent = csvReadContent
            };
            CheckHeaderStatus<T> checkHeaderStatus = new CheckHeaderStatus<T>(headerState, headerStateInfo);
            while (!checkHeaderStatus.IsEndState)
                checkHeaderStatus.Check();


            // https://www.youtube.com/watch?v=IdOv257xuP8&t=310s
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

        private static AHeaderState<T> GetHeaderStatus<T>(CSVReadFileContent csvReadContent)
        {
            if (!csvReadContent.FileExist)
                return new FileNotExistState<T>();
            return new FileExistState<T>();
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
