using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;

namespace AudioLibrary
{
    public static class LavalinkConnection
    {
        private const string NodeConfigs = "LavalinkNodes.json";

        // public static async Task<LavalinkConfiguration> GetLavaConfig(string region = null)
        // {
        //     var node = await GetNode(region);
        //     if (node == null) return null;
        //
        //     var endpoint = new ConnectionEndpoint()
        //     {
        //         Hostname = node.Hostname,
        //         Port = node.Port
        //     };
        //     
        //     return new LavalinkConfiguration()
        //     {
        //         SocketEndpoint = endpoint,
        //         RestEndpoint = endpoint,
        //         
        //     }
        // }
        
        public static async Task<NodeHosts> GetNode(string region = null)
        {
            await using var stream = File.OpenRead(NodeConfigs);
            var nodes = await JsonSerializer.DeserializeAsync<NodeHosts[]>(stream);

            if (nodes?.Length <= 1) return nodes.FirstOrDefault();

            else
            {
                region = CapitalizeFirst(region);
                return nodes?.FirstOrDefault(node => node.Name == region);
            }
        }

        private static string CapitalizeFirst(string region)
        {
            return $"{region.First().ToString().ToUpper()}{region[1..]}";
        }
    }
    
    
    public class NodeHosts
    {
        public string Name { get; set; }
        public string Cloud { get; set; }
        public string RegionId { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
} 