using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace JsonRocket.Test
{
    public class ProfilingTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public ProfilingTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private byte[] LoadJson()
        {
            const string url = "https://chronicdata.cdc.gov/api/views/4juz-x2tp/rows.json?accessType=DOWNLOAD";
            using (var client = new HttpClient())
            {
                return client.GetByteArrayAsync(url).Result;
            }
        }

        [Fact]
        public void TokenizeLargeFile()
        {
            byte[] buffer = LoadJson();
            var tokenizer = new Tokenizer();

            var sw = new Stopwatch();
            var times = new List<TimeSpan>(10);
            for (int i = 0; i < 10; i++)
            {
                if (i > 3) sw.Restart();
                tokenizer.Reset(buffer);
                while (tokenizer.MoveNext())
                {
                }
                if (i > 3) times.Add(sw.Elapsed);
                tokenizer.Current.Should().NotBe(Token.Error);
            }

            foreach (var t in times)
            {
                _outputHelper.WriteLine($"{t} for {buffer.Length / 1024.0:N4} KB");
            }
        }

        [Fact]
        public void TokenizeWithJsonDotNet()
        {
            var buffer = LoadJson();

            var sw = new Stopwatch();
            var times = new List<TimeSpan>(10);
            for (int i = 0; i < 10; i++)
            {
                if (i > 3) sw.Restart();
                var reader = new StreamReader(new MemoryStream(buffer));
                var json = new Newtonsoft.Json.JsonTextReader(reader);
                while (json.Read())
                {
                }
                if (i > 3) times.Add(sw.Elapsed);
            }

            foreach (var t in times)
            {
                _outputHelper.WriteLine($"{t} for {buffer.Length / 1024.0:N4} KB");
            }
        }
    }
}
