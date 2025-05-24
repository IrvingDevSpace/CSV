using BenchmarkDotNet.Attributes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CSV.Benchmark.Model
{
    [MemoryDiagnoser]
    public class Write_VS_OptimizeWrite
    {
        static PropertyInfo[] props = typeof(Info).GetProperties();
        static int propsCount = props.Length;
        delegate object GetterDelegate(object target);
        static GetterDelegate[] getters = props.Select(x => CreateDelegate(x)).ToArray();
        static StringBuilder stringBuilder = new StringBuilder();
        static char[] buffer = new char[90];

        static GetterDelegate CreateDelegate(PropertyInfo property)
        {
            var targetParam = Expression.Parameter(typeof(object), "target");

            // 將 object 轉換為實際的 declaring type
            var castTarget = Expression.Convert(targetParam, property.DeclaringType);

            var getter = Expression.Call(castTarget, property.GetGetMethod());

            var getterFunction = Expression.Lambda<GetterDelegate>(getter, targetParam);

            return getterFunction.Compile();
        }

        [Benchmark]
        public void Write()
        {
            for (int j = 0; j < 3000000; j++)
            {
                Info info = new Info()
                {
                    Id = "1",
                    FirstName = "Alejoa",
                    ILastNamed = "Meijer",
                    Email = "ameijer0@xinhuanet.com",
                    Gender = "Male",
                    IpAddress = "195.65.209.61"
                };

                string data = "";
                PropertyInfo[] props = typeof(Info).GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    data += prop.GetValue(info) + ",";
                }

                data = data.TrimEnd(',');
            }
        }

        [Benchmark]
        public void OptimizeWrite()
        {
            for (int j = 0; j < 3000000; j++)
            {
                Info info = new Info()
                {
                    Id = "1",
                    FirstName = "Alejoa",
                    ILastNamed = "Meijer",
                    Email = "ameijer0@xinhuanet.com",
                    Gender = "Male",
                    IpAddress = "195.65.209.61"
                };

                for (int i = 0; i < propsCount; i++)
                {
                    object a = getters[i](info);

                    stringBuilder.Append(a);
                    if (i < propsCount - 1)
                        stringBuilder.Append(',');
                }

                //for (int i = 0; i < stringBuilder.Length; i++)
                //{
                //    buffer[i] = stringBuilder[i];
                //}

                stringBuilder.CopyTo(0, buffer, 0, stringBuilder.Length - 1);
                //string data = stringBuilder.ToString();
                stringBuilder.Clear();
            }
        }
    }
}
