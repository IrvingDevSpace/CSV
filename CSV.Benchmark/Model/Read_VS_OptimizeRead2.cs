using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CSV.Benchmark.Model
{
    [MemoryDiagnoser]
    public class Read_VS_OptimizeRead2
    {
        static PropertyInfo[] props = typeof(Info).GetProperties();
        static int propsCount = props.Length;
        delegate void SetterDelegate(object target, object value);
        static SetterDelegate[] setters = props.Select(x => CreateDelegate(x)).ToArray();

        private static SetterDelegate CreateDelegate(PropertyInfo property)
        {
            Type target = typeof(object);
            Type value = typeof(object);

            var targetParm = Expression.Parameter(target, "target");
            var valueParm = Expression.Parameter(value, "value");

            var castTargetPram = Expression.Convert(targetParm, property.DeclaringType);
            var castValueParm = Expression.Convert(valueParm, property.PropertyType);

            var setter = Expression.Call(castTargetPram, property.GetSetMethod(), castValueParm);

            var setterFunction = Expression.Lambda<SetterDelegate>(setter, targetParm, valueParm);

            return setterFunction.Compile();
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
            string[] datas = new string[6];
            int start = 0;
            int field = 0;
            while (true)
            {
                int commaIndex = span.Slice(start).IndexOf(',');
                if (commaIndex == -1)
                {
                    datas[field++] = span.Slice(start).ToString();
                    break;
                }
                else
                {
                    datas[field++] = span.Slice(start, commaIndex).ToString();
                    start += commaIndex + 1;
                }
            }


            for (int i = 0; i < propsCount; i++)
            {
                setters[i](info, datas[i]);
            }

            var infos = new List<Info>();
            infos.Add(info);
        }
    }
}