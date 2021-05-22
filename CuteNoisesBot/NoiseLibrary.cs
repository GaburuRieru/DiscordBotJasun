using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CuteNoisesBot
{
    public static class NoiseLibrary
    {
        private const string NoiseBank = "NoiseBank.json";

        // private static Dictionary<string, string> _noises = new Dictionary<string, string>()
        // {
        //     {"horayo", "horayo.mp3"},
        //     {"iamcrazy", "I am crazy.mp3"},
        //     {"kokodayokrooz", "kokodayo.ogg"},
        //     {"kokodayokorone", "kokodayo_korone.mp3"},
        //     {"pekopeko", "PEKOPEKOPEKOPEKO.mp3"},
        //     {"yubi", "yubi.ogg"},
        // };

        // private static Dictionary<string, string> _noises = new Dictionary<string, string>();


        // public string GetRandomNoisePath()
        // {
        //     var keys = _noises.Keys;
        //     var element = keys.ElementAt()    
        // }

        public static async Task<string> GetNoise(string noiseCommand)
        {
            if (string.IsNullOrWhiteSpace(noiseCommand)) return "";

            // if (_noises.TryGetValue(noiseCommand, out string noisePathOutput))
            // {
            //     if (string.IsNullOrEmpty(noisePathOutput)) return "";
            //
            //     return Path.Combine("Noises", noisePathOutput);
            // }

            var result = await GetNoisePathFromNoiseBankAsync(noiseCommand);

            if (result == string.Empty) return string.Empty;

            return Path.Combine("Noises", result);
        }

        private static async Task<string> GetNoisePathFromNoiseBankAsync(string command)
        {
            var elements = await ReadFromJson();

            foreach (var element in elements)
                if (string.Equals(element.Command, command))
                {
                    if (Globals.SFWMode && !element.SFW) return "NSFW";
                    return element.Path;
                }

            return string.Empty;
        }

        private static async Task<NoiseJson[]> ReadFromJson()
        {
            await using var stream = File.OpenRead(NoiseBank);
            var elements = await JsonSerializer.DeserializeAsync<NoiseJson[]>(stream);
            return elements;
        }

        public static async Task<string> GetAvailableCommands()
        {
            var elements = await ReadFromJson();
            var stringCommand = new StringBuilder("Noises available: ");

            for (var i = 0; i < elements.Length; i++)
            {
                stringCommand.Append(elements[i].Command);

                if (i < elements.Length) stringCommand.Append(", ");
            }

            return stringCommand.ToString();
        }
    }

    internal class NoiseJson
    {
        public string Command { get; set; }
        public string Path { get; set; }
        public bool SFW { get; set; }
    }
}