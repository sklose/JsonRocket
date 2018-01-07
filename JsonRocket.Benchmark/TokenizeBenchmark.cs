using BenchmarkDotNet.Attributes;
using System;
using System.IO;
using Newtonsoft.Json;

namespace JsonRocket.Benchmark
{
    public class TokenizeBenchmark
    {
        [Benchmark(Description = "JsonRocket")]
        public void JsonRocket()
        {
            var tokenizer = new Tokenizer();
            tokenizer.Reset(Resources.ResumeJson);
            while (tokenizer.MoveNext())
            {
            }

            if (tokenizer.Current == Token.Error)
            {
                throw new Exception("unable to parse JSON");
            }
        }

        [Benchmark(Description = "Json.NET")]
        public void JsonDotNet()
        {
            using (var stream = new MemoryStream(Resources.ResumeJson))
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                while (json.Read())
                {
                }
            }
        }
    }
}