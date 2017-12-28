using BenchmarkDotNet.Running;

namespace JsonRocket.Benchmark
{
    public static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TokenizerBenchmark>();
        }
    }
}
