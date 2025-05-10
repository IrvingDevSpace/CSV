using BenchmarkDotNet.Running;
using CSV.Benchmark.Model;

namespace CSV.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Read_VS_OptimizeRead>();

            // object obj = "123"
            // obj.GetType().Name
        }
    }
}
