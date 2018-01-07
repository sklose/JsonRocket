using System.Resources;

namespace JsonRocket.Benchmark
{
    public static class Resources
    {
        static Resources()
        {
            ResumeJson = LoadResource("resume.json");
        }

        public static readonly byte[] ResumeJson;

        private static byte[] LoadResource(string name)
        {
            var assmebly = typeof(Resources).Assembly;
            foreach (string resourceName in assmebly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(name))
                {
                    var stream = assmebly.GetManifestResourceStream(resourceName);
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }

            throw new MissingManifestResourceException($"unable to find {name}");
        }
    }
}