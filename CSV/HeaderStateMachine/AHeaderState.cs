using CSV.Model.HeaderStateMachine;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace CSV.HeaderStateMachine
{
    public abstract class AHeaderState<T>
    {
        public AHeaderState<T> NextState { get; protected set; }
        public bool IsEndState { get; protected set; }
        public abstract void Execute(HeaderStateInfo<T> headerStateInfo);

        protected bool IsValidHeaderName(HeaderStateInfo<T> headerStateInfo)
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

        protected void WriteHeaderAndContent(HeaderStateInfo<T> headerStateInfo)
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
