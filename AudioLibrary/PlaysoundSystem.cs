using System;
using System.Buffers.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuteNoisesBot;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace AudioLibrary
{
    public static class PlaysoundSystem
    {
        public static async Task Play(CommandContext ctx, string playSoundPath, string domain)
        {
            //Check if command invoker is in a voice channel
            var channel = (ctx.Member.VoiceState?.Channel == null)
                ? null
                : ctx.Member.VoiceState
                    .Channel;

            if (channel == null)
            {
                return;
            }

            //var soundPath = await NoiseLibrary.GetNoise(sound);
            if (string.IsNullOrEmpty(playSoundPath))
            {
                // Console.WriteLine($"Playsound -- {playSoundPath} not found");
                return;
            }

            //get lavalink client
            var lava = ctx.Client.GetLavalink();
            if (lava == null)
            {
                await ctx.RespondAsync("Lavalink not initalized.");
                return;
            }
            
            //Only one node for now, so use it 
            //var node = lava.GetIdealNodeConnection();
            //var node = lava.ConnectedNodes.FirstOrDefault().Value;
            var nodes = lava.ConnectedNodes;
            LavalinkNodeConnection node = null;
            if (nodes.Count <= 1) node = nodes.Values.FirstOrDefault();

            if (node == null)
            {
                await ctx.RespondAsync("No node found.");
                return;
            }

            var guildId = ctx.Guild.Id;
            //await node.Rest.GetTracksAsync(new Uri(new UriBuilder()));
            //load track to play

            //For when player and lavalink is on same machine
            //var result = await node.Rest.GetTracksAsync(new FileInfo(playSoundPath));
            // if (result.LoadResultType is LavalinkLoadResultType.LoadFailed or LavalinkLoadResultType.NoMatches)
            // {
            //     string failedMessage = "";
            //     if (result.LoadResultType is LavalinkLoadResultType.NoMatches)
            //         failedMessage = "No matches for searched track";
            //     if (result.LoadResultType is LavalinkLoadResultType.LoadFailed) failedMessage = "Track failed to load";
            //
            //     await ctx.RespondAsync($"Playsound failed. Reason: {failedMessage}");
            //     return;
            // }
            //
            // var track = result.Tracks.First();


            //URI 
            var uri = ContentDelivery.GetUri(domain, playSoundPath);
            // Console.WriteLine($"URI: {uri}");
            // Console.WriteLine($"FullURL: {uri.AbsoluteUri}");
            // Console.WriteLine($"FullPath: {uri.AbsolutePath}");

            var result = await node.Rest.GetTracksAsync(uri);
            //var result = await node.Rest.GetTracksAsync(new Uri("https://hololive-playsounds.s3.ap-southeast-1.amazonaws.com/Playsound/yubi.ogg"));
           // var result = await node.Rest.GetTracksAsync("(2nd most liked) korone's \"yubi yubi!\" sound effect",
            //LavalinkSearchType.Youtube);
            
            
            // Console.WriteLine(result.Exception.Message);

            if (!result.Tracks.Any())
            {
                //Console.WriteLine("No tracks loaded");
                await ctx.Channel.SendMessageAsync("No tracks loaded");
                return;
            }

            var track = result.Tracks.First();




            //Check if bot is not already in a voice channel and playing
            var connection = node.GetGuildConnection(ctx.Guild);
            //Console.WriteLine($"{connection.Channel}");
            if (connection == null)
            {
                //connect to channel
                // Console.WriteLine($"Connecting to voice channel {channel.Name} in guild {channel.Guild.Name}");
                connection = await node.ConnectAsync(channel);
                //PlayQueueAsync(connection);
            }
            
            //Check when command invoker is in a voice channel,false);
                //Console.WriteLine($"Connected to channel {channel.Name}");
            //BUT the bot is already playing in another channel
            if (connection.Channel != channel) return;

            //Add track to queue
            PlaysoundQueue.EnqueueTrack(guildId, track);

            //lavalink play
            if (connection.CurrentState.CurrentTrack == null) PlayQueueAsync(connection, ctx.Channel);

            //await connection.PlayAsync(track);
        }

        private static async void PlayQueueAsync(LavalinkGuildConnection connection, DiscordChannel invokingChannel)
        {
           // Console.WriteLine($"Playing queue");
            
            var guildId = connection.Guild.Id;

            var track = PlaysoundQueue.DequeueTrack(guildId);
            if (track == null) return;
            
            
            
            // Console.WriteLine($"Track details: \n" +
            //                   $"Length: {track.Length.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)} \n" +
            //                   $"Start position: {track.Position.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)}");
            
            // var lavaBufferMs = await AudioConfig.GetBufferDuration();
            //const int lavaBufferMs = 400;
            //Console.WriteLine($"Delay buffer: {lavaBufferMs.ToString()}");

            // connection.PlaybackStarted += (senderConn, args) =>
            // {
            //     //Console.WriteLine($"Playback of track {args.Track.Title} started by {args.Player.Guild.Name}");
            //     return Task.CompletedTask;
            // };
            
            
            
            
            // connection.PlayerUpdated += async (sender, args) =>
            // {
            //     var position = args.Position;
            //     Console.WriteLine(
            //         $"Last updated position was: {position.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)}");
            //     Console.WriteLine($"Last updated time was: {args.Timestamp.Millisecond.ToString()}");
            // };

            connection.PlaybackFinished += async (senderConn, args) =>
            {
                //Wait for buffer to finish
                //await Task.Delay(1000);
                // var timer = lavaBufferMs;
                // do
                // {
                //     await Task.Delay(400);
                //    // Console.WriteLine($"Waited for 400ms");
                //     timer -= 400;
                // } while (timer > 0);

                //Console.WriteLine($"Number of queued tracks:{PlaysoundQueue.QueuedTracks(guildId).ToString()}");

                if (PlaysoundQueue.QueuedTracks(guildId) > 0 && args.Reason is TrackEndReason.Finished)
                {
                    var trackToPlay = PlaysoundQueue.DequeueTrack(guildId);
                    //await Task.Delay(200);
                    await connection.PlayAsync(trackToPlay);
                }

                else
                {
                   // Console.WriteLine(
                   //     $"Playback of track finished {args.Track.Title}: Disconnected due to {args.Reason.ToString()}");
                    await senderConn.DisconnectAsync();
                }

                // if (args.Reason is TrackEndReason.Replaced)
                // {
                //     Console.WriteLine($"Changed playing track");
                // }
            };

            connection.TrackException += async (sender, args) =>
            {
                var errorString = ($"Track playback encountered an error: \n" +
                                   $"Error: {args.Error}");

                                   Console.WriteLine(errorString);
                await invokingChannel.SendMessageAsync(errorString);
                await args.Player.StopAsync();
                await args.Player.DisconnectAsync();
            };
            
            connection.TrackStuck += async (sender, args) =>
            {
                string message =
                    $"Track - {args.Track} was stuck for {args.ThresholdMilliseconds}ms at {args.Player.Channel.Name} in {args.Player.Guild.Name}";
                Console.WriteLine(message);

                if (args.ThresholdMilliseconds > 1500)
                {
                    await args.Player.StopAsync();
                    await args.Player.DisconnectAsync();
                }
            };

            if (connection.CurrentState.CurrentTrack == null)
            {
                
                await connection.PlayAsync(track);
            }
        }

        public static async Task LeaveVoice(CommandContext ctx)
        {
            var lavalink = ctx.Client.GetLavalink();
            var node = lavalink.GetIdealNodeConnection();

            var conn = lavalink.GetGuildConnection(ctx.Guild);
            if (conn == null) //No voice connections in guild
            {
               // Console.WriteLine($"Bot is not connected to a voice channel in {ctx.Guild.Name}");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null) return;

            await conn.StopAsync();

            //await Task.Delay(250);

            if (conn.IsConnected) await conn.DisconnectAsync();
        }


        public static async Task SeeNode(CommandContext ctx)
        {
            var lava = ctx.Client.GetLavalink();

            if (lava.ConnectedNodes.Count == 0)
            {
                await ctx.Channel.SendMessageAsync($"No nodes found for this lava client.");
                return;
            }

            var node = lava.ConnectedNodes.First();



            var message = $"Node is from {node.Key.Hostname}";
            await ctx.Channel.SendMessageAsync(message);
        }
    }
}