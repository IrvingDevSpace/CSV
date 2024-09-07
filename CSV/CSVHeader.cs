using CSV.Enums;
using CSV.HeaderStateMachine;
using CSV.Model.HeaderStateMachine;
using Stateless;
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
            HeaderStateInfo<T> headerStateInfo = new HeaderStateInfo<T>
            {
                FilePath = filePath,
                Data = data,
                CSVReadContent = csvReadContent
            };
            var headerStateMachine = CreateStateMachine<T>(headerStateInfo);

            // 依次觸發狀態機事件，直到達到最終狀態
            while (!headerStateMachine.IsInState(HeaderState.Finished))
            {
                headerStateMachine.Fire(HeaderTrigger.Check);
            }
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

        private static StateMachine<HeaderState, HeaderTrigger> CreateStateMachine<T>(HeaderStateInfo<T> headerStateInfo)
        {
            var headerStateMachine = new StateMachine<HeaderState, HeaderTrigger>(HeaderState.Start);

            // 職責鏈
            headerStateMachine.Configure(HeaderState.Start)
                .PermitDynamic(HeaderTrigger.Check, () =>
                    headerStateInfo.CSVReadContent.FileExist ? HeaderState.FileExist : HeaderState.FileNotExist);

            headerStateMachine.Configure(HeaderState.FileExist)
                .PermitDynamic(HeaderTrigger.Check, () =>
                    IsValidHeaderName(headerStateInfo) ? HeaderState.Finished : HeaderState.HeaderInValid);

            headerStateMachine.Configure(HeaderState.FileNotExist)
                .OnEntry(() => WriteHeaderAndContent(headerStateInfo))
                .Permit(HeaderTrigger.Check, HeaderState.Finished);

            headerStateMachine.Configure(HeaderState.HeaderInValid)
                .OnEntry(() => WriteHeaderAndContent(headerStateInfo))
                .Permit(HeaderTrigger.Check, HeaderState.Finished);

            return headerStateMachine;
        }

        private static bool IsValidHeaderName<T>(HeaderStateInfo<T> headerStateInfo)
        {
            PropertyInfo[] propertyInfos = headerStateInfo.Data.GetType().GetProperties();
            if (propertyInfos.Length != headerStateInfo.CSVReadContent.HeaderNames.Count)
                return false;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                String headerName = headerStateInfo.CSVReadContent.HeaderNames[i];
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

        private static void WriteHeaderAndContent<T>(HeaderStateInfo<T> headerStateInfo)
        {
            String writeContent = "";
            String headerName = "";
            PropertyInfo[] propertyInfos = headerStateInfo.Data.GetType().GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (i > 0)
                    headerName += ",";
                String colName = propertyInfos[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? propertyInfos[i].Name;
                headerName += colName;
            }
            writeContent += headerName + Environment.NewLine;

            for (int i = 0; i < headerStateInfo.CSVReadContent.ContentLines.Count; i++)
                writeContent += headerStateInfo.CSVReadContent.ContentLines[i] + Environment.NewLine;

            using (StreamWriter writer = new StreamWriter(headerStateInfo.FilePath, false, Encoding.UTF8))
                writer.Write(writeContent);
        }
    }
}
