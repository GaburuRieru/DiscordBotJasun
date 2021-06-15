using System.IO;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace AudioLibrary
{
    public static class Conversion
    {
        public static async Task<string> ToBase64String(string filePath)
        {
            Console.WriteLine($"Absolute path is : {Directory.GetCurrentDirectory()}/{filePath}");
            Console.WriteLine($"Reading Stream from {filePath}");
            await using var stream = File.OpenRead(filePath);
            await using var memorystream = new MemoryStream();
            
            var bytes = new byte[stream.Length];
            Console.WriteLine($"Reading...");
            await stream.CopyToAsync(memorystream);
            Console.WriteLine($"Done Copying..");
            //var count = await stream.ReadAsync(bytes.AsMemory(0, filePath.Length));
            bytes = memorystream.ToArray();
            Console.WriteLine($"Converting to base64");
            //Console.WriteLine($"Finished reading stream: {count.ToString()}");
            var base64 = Convert.ToBase64String(bytes);
            Console.WriteLine($"Finished Converting to base64");
            return base64;
        }
    }
}