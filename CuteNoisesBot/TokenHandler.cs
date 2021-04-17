using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlowWaterBot
{
    public static class TokenHandler
    {
        private const string TokenFile = "token.txt";
        private const string Pattern = "\"([^\"]*)\"";


        public static async Task<string> GetTokenAsync()
        {
            return await ReadTokenFileAsync();
        }

        private static async Task<string> ReadTokenFileAsync()
        {
            var path = $"{Directory.GetCurrentDirectory()}/{TokenFile}";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Token file not found.", TokenFile);
            }

            using (FileStream file = File.OpenRead(path))
            using (StreamReader reader = new StreamReader(file))
            {
                string readstring = default;
                while (!reader.EndOfStream)
                {
                    readstring = await reader.ReadLineAsync();
                    if (readstring != null && readstring.Contains("token=")) break;
                }

                if (string.IsNullOrEmpty(readstring)) return string.Empty;
                
                Regex reg = new Regex(Pattern);
                Match match = reg.Match(readstring);
                return match.Value.Trim('"');
                //eturn match.Value;
            }
            
        }
    }
}