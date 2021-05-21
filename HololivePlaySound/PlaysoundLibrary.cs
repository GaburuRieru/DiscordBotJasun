using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace HololivePlaySound
{
    public static class PlaysoundLibrary
    {
        private const string NoiseBank = "Playsounds.json";
        private const string PlaysoundFolder = "Playsound";

        private static Dictionary<string, string> _playsoundPaths = new Dictionary<string, string>();
        

        public static async Task<string> GetPlaysound(string noiseCommand)
        {
            if (string.IsNullOrWhiteSpace(noiseCommand)) return "";

            // if (_noises.TryGetValue(noiseCommand, out string noisePathOutput))
            // {
            //     if (string.IsNullOrEmpty(noisePathOutput)) return "";
            //
            //     return Path.Combine("Noises", noisePathOutput);
            // }

            //var result = await GetPlaysoundPathFromNoisebankAsync(noiseCommand);
            var result = await GetPlaysoundPathFromMemoryAsync(noiseCommand);

            if (result == string.Empty) return string.Empty;

            return Path.Combine(PlaysoundFolder, result);
        }

        private static async Task<string> GetPlaysoundPathFromMemoryAsync(string command)
        {
            if (_playsoundPaths.TryGetValue(command, out var path)) return path;
            Console.WriteLine($"Invalid Playsound - {command}");
            return string.Empty;

        }

        // private static async Task<string> GetPlaysoundPathFromNoisebankAsync(string command)
        // {
        //     var elements = await ReadFromJson();
        //
        //     foreach (var element in elements)
        //         if (string.Equals(element.Command, command))
        //             return element.Path;
        //
        //     return string.Empty;
        // }

        private static async Task<HoloJson[]> ReadFromJson()
        {
            await using var stream = File.OpenRead(NoiseBank);
            var elements = await JsonSerializer.DeserializeAsync<HoloJson[]>(stream);
            return elements;
        }
        
        public static async Task<string> GetAvailableCommands()
        {
            if (_playsoundPaths == null || _playsoundPaths.Count == 0) return "Playsounds unavailable at the moment.";
            
            // var elements = await ReadFromJson();
            // var stringCommand = new StringBuilder("Playsounds available: ");
            //
            // for (var i = 0; i < elements.Length; i++)
            // {
            //     stringCommand.Append(elements[i].Command);
            //
            //     if (i < elements.Length) stringCommand.Append(", ");
            // }

            
            var separator = ", ";
            var stringCommand = new StringBuilder("Playsounds available: ");

            for (int i = 0; i < _playsoundPaths.Count; i++)
            {
                stringCommand.Append(_playsoundPaths.ElementAt(i).Key);
                if (i < _playsoundPaths.Count) stringCommand.Append(separator);
            }

            return stringCommand.ToString();
        }
        

        public static async Task ReloadDatabase()
        {
            _playsoundPaths.Clear();

            var elements = await ReadFromJson();
            foreach (var element in elements)
            {
                if (!_playsoundPaths.TryAdd(element.Command, element.Path))
                {
                    Console.WriteLine($"Failed to add Command - {element.Command} for Path {element.Path}");
                }
            }

        }
    }

    internal class HoloJson
    {
        public string Command { get; set; }
        public string Path { get; set; }
    }
}