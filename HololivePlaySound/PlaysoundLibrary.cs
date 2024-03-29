﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace HololivePlaySound
{
    public static class PlaysoundLibrary
    {
        private const string NoiseBank = "Playsounds.json";
        //private const string PlaysoundFolder = "Playsound";

       // private static Dictionary<string, string> _playsoundPaths = new Dictionary<string, string>();
        


        // public string GetRandomNoisePath()
        // {
        //     var keys = _noises.Keys;
        //     var element = keys.ElementAt()    
        // }

        public static async Task<string> GetPlaysoundAsync(string noiseCommand)
        {
            if (string.IsNullOrWhiteSpace(noiseCommand)) return "";

            // if (_noises.TryGetValue(noiseCommand, out string noisePathOutput))
            // {
            //     if (string.IsNullOrEmpty(noisePathOutput)) return "";
            //
            //     return Path.Combine("Noises", noisePathOutput);
            // }

            var result = await GetPlaysoundPathFromNoisebankAsync(noiseCommand);
            //var result = await GetPlaysoundPathFromMemoryAsync(noiseCommand);

            return result == string.Empty ? string.Empty : result;

            //return Path.Combine(PlaysoundFolder, result);
        }

        // private static async Task<string> GetPlaysoundPathFromMemoryAsync(string command)
        // {
        //     if (_playsoundPaths.TryGetValue(command, out var path)) return path;
        //     Console.WriteLine($"Invalid Playsound - {command}");
        //     return string.Empty;
        //
        // }

        private static async Task<string> GetPlaysoundPathFromNoisebankAsync(string command)
        {
            var elements = await ReadFromJsonAsync();
        
            foreach (var element in elements)
                if (string.Equals(element.Command, command))
                    return element.Path;
        
            return string.Empty;
        }

        private static async Task<HoloJson[]> ReadFromJsonAsync()
        {
            await using var stream = File.OpenRead(NoiseBank);
            var elements = await JsonSerializer.DeserializeAsync<HoloJson[]>(stream);
            return elements;
        }
        
        public static async Task<string> GetAvailableCommandsAsync()
        {
           // if (_playsoundPaths == null || _playsoundPaths.Count == 0) return "Playsounds unavailable at the moment.";
            
            var elements = await ReadFromJsonAsync();
            var stringCommand = new StringBuilder("Playsounds available: ");
            
            for (var i = 0; i < elements.Length; i++)
            {
                stringCommand.Append(elements[i].Command);
            
                if (i < elements.Length) stringCommand.Append(", ");
            }

            
            // var separator = ", ";
            // var stringCommand = new StringBuilder("Playsounds available: ");
            //
            // for (int i = 0; i < _playsoundPaths.Count; i++)
            // {
            //     stringCommand.Append(_playsoundPaths.ElementAt(i).Key);
            //     if (i < _playsoundPaths.Count) stringCommand.Append(separator);
            //}

            return stringCommand.ToString();
        }
        

        // public static async Task ReloadDatabaseAsync()
        // {
        //     _playsoundPaths.Clear();
        //
        //     var elements = await ReadFromJsonAsync();
        //     foreach (var element in elements)
        //     {
        //         if (!_playsoundPaths.TryAdd(element.Command, element.Path))
        //         {
        //             Console.WriteLine($"Failed to add Command - {element.Command} for Path {element.Path}");
        //         }
        //     }
        //
        //     // if (_playsoundPaths.Count > 0) await channel.SendMessageAsync($"Playsound database reloaded.");
        //     // else await channel.SendMessageAsync($"Failed to reload playsound database; Check logs.");
        // }
    }

    internal class HoloJson
    {
        public string Command { get; set; }
        public string Path { get; set; }
    }
}