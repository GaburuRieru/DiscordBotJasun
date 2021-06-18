using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
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

            if (nodes.Length <= 1) return nodes.FirstOrDefault();

            else
            {
                region = CapitalizeFirst(region);
                return nodes.FirstOrDefault(x => x.Name == region);
            }
        }

        private static string CapitalizeFirst(string region)
        {
            return $"{region.First().ToString().ToUpper()}{region[1..]}";
        }
        
        //Try to get node when autoreconnected
        // public static void ReconnectNode(LavalinkExtension lavalink, LavalinkNodeConnection node)
        // {
        //    node.Disconnected += async (sender, args) =>
        //    {
        //        
        //    } 
        // }
        
        //Auto reconnect to disconnected node if disconnection was unclean
        public static void AutoReconnectUnclean(LavalinkNodeConnection node, LavalinkConfiguration config)
        {
            node.Disconnected += async (sender, args) =>
            {
                if (args.IsCleanClose) return;

                Console.WriteLine($"Unclean disconnection of node {args.LavalinkNode.NodeEndpoint.Hostname} \n" +
                                  $"Attempting to reconnect to disconnected node.");
                do
                {
                    int countdown = 15;
                    while (countdown > 0)
                    {
                        Console.WriteLine($"Reconnecting in {countdown.ToString()} seconds..");
                        await Task.Delay(1000);
                        countdown--;
                    }

                    try
                    {
                        node = await node.Parent.ConnectAsync(config).ConfigureAwait(false);
                    }
                    catch (WebSocketException we)
                    {
                        Console.WriteLine(we);
                    }
                    finally
                    {
                        
                    }
                    
                    if (node.IsConnected) break;
                    
                 
                } while (!node.IsConnected);

                Console.WriteLine($"Reconnected to node.");
            };
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