using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Fact]
        public void TokenizeLargeFile()
        {
            const string url = "https://chronicdata.cdc.gov/api/views/4juz-x2tp/rows.json?accessType=DOWNLOAD";

            byte[] buffer;
            using (var client = new HttpClient())
            {
                buffer = client.GetByteArrayAsync(url).Result;
            }

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
    }
}
