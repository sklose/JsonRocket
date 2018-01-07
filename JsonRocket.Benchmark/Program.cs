using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace JsonRocket.Benchmark
{
    public static class Program
    {
        private static void Main()
        {
            var config = ManualConfig.Create(DefaultConfig.Instance)
                .With(Job.Default.With(CsProjCoreToolchain.NetCoreApp20))
                .With(Job.Default.With(CsProjClassicNetToolchain.Net462))
                .With(MemoryDiagnoser.Default)
                .With(DisassemblyDiagnoser.Create(new DisassemblyDiagnoserConfig()))
                .With(HardwareCounter.BranchMispredictions, HardwareCounter.BranchInstructions);

            BenchmarkRunner.Run<TokenizeBenchmark>(config);
        }
    }
}