using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioLibrary
{
    public static class LavalinkHandler
    {
        private const string LavaFile = "LavaRegion.txt";
       // private const string Pattern = "\"([^\"]*)\"";


        public static async Task<string> GetLavalinkRegion()
        {
            return await ReadFile();
        }

        private static async Task<string> ReadFile()
        {
            var path = $"{Directory.GetCurrentDirectory()}/{LavaFile}";
            if (!File.Exists(path))
            {
                //throw new FileNotFoundException("Token file not found.", TokenFile);
                var newStream = File.Create(path);
                await using StreamWriter writer = new StreamWriter(newStream);
                await writer.WriteAsync("Localhost");
                return "Localhost";
            }

            await using FileStream file = File.OpenRead(path);
            using StreamReader reader = new StreamReader(file);
            string readstring = default;
            while (!reader.EndOfStream)
            {
                readstring = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(readstring)) return string.Empty;
            
                return readstring;
            }
            
            // if (string.IsNullOrEmpty(readstring)) return string.Empty;
            //     
            // Regex reg = new Regex(Pattern);
            // Match match = reg.Match(readstring);
            // return match.Value.Trim('"');
            return string.Empty;
        }
    }
}