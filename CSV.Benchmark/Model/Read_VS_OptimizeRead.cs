using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CSV.Benchmark.Model
{
    [MemoryDiagnoser]
    public class Read_VS_OptimizeRead
    {
        //static List<Action<object, object>> actionList = new List<Action<object, object>>();

        //static Read_VS_OptimizeRead()
        //{
        //    PropertyInfo[] props = typeof(Info).GetProperties();
        //    for (int i = 0; i < props.Length; i++)
        //    {
        //        actionList.Add(props[i].SetValue);
        //    }
        //}

        delegate void SetterDelegate(object target, object value);

        static SetterDelegate[] setters = typeof(Info).GetProperties().Select(x => CreateSetterDelegate(x)).ToArray();

        private static SetterDelegate CreateSetterDelegate(PropertyInfo propertyInfo)
        {
            Type targetType = typeof(object);
            Type valueType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");
            var valueParam = Expression.Parameter(valueType, "value");

            var convertTargetParam = Expression.Convert(targetParam, propertyInfo.DeclaringType);
            var convertValueParam = Expression.Convert(valueParam, propertyInfo.PropertyType);

            var setter = Expression.Call(convertTargetParam, propertyInfo.GetSetMethod(), convertValueParam);

            var expression = Expression.Lambda<SetterDelegate>(setter, targetParam, valueParam);

            return expression.Compile();
        }

        [Benchmark]
        public void Read()
        {
            string content = "1,Alejoa,Meijer,ameijer0@xinhuanet.com,Male,195.65.209.61";
            string[] contents = content.Split(',');
            Info info = new Info();

            var props = info.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                props[i].SetValue(info, contents[i]);
            }

            var infos = new List<Info>();
            infos.Add(info);
        }

        [Benchmark]
        public void OptimizeRead()
        {
            Info info = new Info();

            string content = "1,Alejoa,Meijer,ameijer0@xinhuanet.com,Male,195.65.209.61";
            ReadOnlySpan<char> span = content.AsSpan();
            //string[] datas = new string[6];
            int start = 0;
            //int field = 0;

            //while (true)
            //{
            //    int commaIndex = span.Slice(start).IndexOf(',');
            //    if (commaIndex == -1)
            //    {
            //        datas[field++] = span.Slice(start).ToString();
            //        break;
            //    }
            //    else
            //    {
            //        datas[field++] = span.Slice(start, commaIndex).ToString();
            //        start += commaIndex + 1;
            //    }
            //}


            //for (int i = 0; i < setters.Length; i++)
            //{
            //    setters[i](info, datas[i]);


            for (int i = 0; i < setters.Length; i++)
            {
                int commaIndex = span.Slice(start).IndexOf(',');
                if (commaIndex == -1)
                {
                    setters[i](info, span.Slice(start).ToString());
                    break;
                }
                else
                {
                    setters[i](info, span.Slice(start, commaIndex).ToString());
                    start += commaIndex + 1;
                }
            }

            var infos = new List<Info>();
            infos.Add(info);
        }
    }
}