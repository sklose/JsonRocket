using System;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace JsonRocket.Benchmark
{
    public class TokenizerBenchmark
    {
        private byte[] _data;
        private Tokenizer _tokenizer;

        [GlobalSetup]
        public void LoadData()
        {
            _tokenizer = new Tokenizer();
            _data = File.ReadAllBytes("baby_names.json");
        }

        [Benchmark]
        public void Tokenize()
        {
            _tokenizer.Reset(_data);
            while (_tokenizer.MoveNext())
            {
            }

            if (_tokenizer.Current == Token.Error)
                throw new Exception("unable to parse JSON");
        }
    }
}
