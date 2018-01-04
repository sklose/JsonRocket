using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace JsonRocket.Test
{
    public class ProfilingTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly byte[] _buffer;

        public ProfilingTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _buffer = LoadJson();
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
        public void Tokenize()
        {
            var tokenizer = new Tokenizer();

            var sw = new Stopwatch();
            var times = new List<TimeSpan>(10);
            for (int i = 0; i < 10; i++)
            {
                if (i > 3) sw.Restart();
                tokenizer.Reset(_buffer);
                while (tokenizer.MoveNext())
                {
                }
                if (i > 3) times.Add(sw.Elapsed);
                tokenizer.Current.Should().NotBe(Token.Error);
            }

            foreach (var t in times)
            {
                _outputHelper.WriteLine($"{t} for {_buffer.Length / 1024.0:N4} KB");
            }
        }

        [Fact]
        public void TokenizeWithJsonDotNet()
        {
            var sw = new Stopwatch();
            var times = new List<TimeSpan>(10);
            for (int i = 0; i < 10; i++)
            {
                if (i > 3) sw.Restart();
                var reader = new StreamReader(new MemoryStream(_buffer));
                var json = new Newtonsoft.Json.JsonTextReader(reader);
                while (json.Read())
                {
                }
                if (i > 3) times.Add(sw.Elapsed);
            }

            foreach (var t in times)
            {
                _outputHelper.WriteLine($"{t} for {_buffer.Length / 1024.0:N4} KB");
            }
        }

        [Fact]
        public void Extract()
        {
            var tokenizer = new Tokenizer();
            var trie = new Trie();
            trie.Add("meta.view.metadata.renderTypeConfig.visible.table");
            var extractor = new Extractor(trie);

            var sw = new Stopwatch();
            var times = new List<TimeSpan>(10);
            var result = new List<ExtractedValue>();
            for (int i = 0; i < 10; i++)
            {
                if (i > 3) sw.Restart();
                tokenizer.Reset(_buffer);
                extractor.ReadFrom(tokenizer, result);
                if (i > 3) times.Add(sw.Elapsed);
                result.Should().HaveCount(1);
                result.Clear();
            }

            foreach (var t in times)
            {
                _outputHelper.WriteLine($"{t} for {_buffer.Length / 1024.0:N4} KB");
            }
        }

        [Fact]
        public void ExtractWithJsonDotNet()
        {
            var sw = new Stopwatch();
            var times = new List<TimeSpan>(10);
            for (int i = 0; i < 10; i++)
            {
                if (i > 3) sw.Restart();

                var reader = new StreamReader(new MemoryStream(_buffer));
                var json = new Newtonsoft.Json.JsonTextReader(reader);
                var obj = JToken.ReadFrom(json);
                var token = obj.SelectToken("$.meta.view.metadata.renderTypeConfig.visible.table", true);

                if (i > 3) times.Add(sw.Elapsed);
                token.Value<bool>().Should().BeTrue();
            }

            foreach (var t in times)
            {
                _outputHelper.WriteLine($"{t} for {_buffer.Length / 1024.0:N4} KB");
            }
        }
    }
}