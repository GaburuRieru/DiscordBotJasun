
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioLibrary
{
    public class AudioConfig
    {
        private const string ConfigPath = "config.json";

        public static async Task<int> GetBufferDuration()
        {
            // var stream = File.OpenRead(ConfigPath);
            // TextReader textReader = new StreamReader(stream);
            // JsonReader jsonReader = new JsonTextReader(textReader);
            // var jsonDeseriealizer = new JsonSerializer().Deserialize<Config>(jsonReader);

            await using var stream = File.OpenRead(ConfigPath);
            var element = await JsonSerializer.DeserializeAsync<Config>(stream);
            return element?.Parameter ?? 0;
        }

        private class Config
        {
            public string Key { get; set; }
            public int Parameter { get; set; }
        }
    }


}