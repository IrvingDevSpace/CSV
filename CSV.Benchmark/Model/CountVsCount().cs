using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace CSV.Benchmark.Model
{
    [MemoryDiagnoser]
    public class CountVsCount__
    {
        IEnumerable<int> counts = new List<int>() { 1, 2, 3, 4, 5 };
        //[Benchmark]
        public void Count()
        {
            var count = new List<int>().Count;
        }

        //[Benchmark]
        public void CountFun()
        {
            var count = new List<int>().ToList().Count;
        }

        [Benchmark]
        public void List()
        {
            counts.ToList();
        }

        //[Benchmark]
        //public void List2()
        //{
        //    counts.ToList().ToList();
        //}

        //[Benchmark]
        //public void List3()
        //{
        //    counts.ToList().ToList().ToList();
        //}

        [Benchmark]
        public void List4()
        {
            var a = (List<int>)counts;
        }
    }
}
