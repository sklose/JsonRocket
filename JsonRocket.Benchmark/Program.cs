using BenchmarkDotNet.Running;

namespace JsonRocket.Benchmark
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<TokenizerBenchmark>();
        }
    }
}