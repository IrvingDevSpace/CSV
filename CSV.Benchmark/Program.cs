using BenchmarkDotNet.Running;
using CSV.Benchmark.Model;

namespace CSV.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Write_VS_OptimizeWrite>();
        }
    }
}
