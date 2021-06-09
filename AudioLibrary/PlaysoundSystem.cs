using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace AudioLibrary
{
    public static class PlaysoundSystem
    {
        public static async Task Play(CommandContext ctx, string playSoundPath)
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
                Console.WriteLine($"Playsound -- {playSoundPath} not found");
                return;
            }

            //get lavalink client
            var lava = ctx.Client.GetLavalink();
            if (lava == null)
            {
                await ctx.RespondAsync("Lavalink not initalized.");
                return;
            }

            var node = lava.GetIdealNodeConnection();
            if (node == null)
            {
                await ctx.RespondAsync("No node found.");
                return;
            }


            //load track to play
            var result = await node.Rest.GetTracksAsync(new FileInfo(playSoundPath));
            if (result.LoadResultType == LavalinkLoadResultType.LoadFailed
                || result.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Playsound failed. Reason: {result.Exception.Message}.");
                return;
            }

            var guildId = ctx.Guild.Id;
            var track = result.Tracks.First();

            //Add track to queue
            PlaysoundQueue.EnqueueTrack(guildId, track);

            //Check if bot is not already in a voice channel and playing
            var connection = node.GetGuildConnection(ctx.Guild);
            if (connection == null)
            {
                //connect to channel
                connection = await node.ConnectAsync(channel);
                Console.WriteLine($"Connecting to voice channel {channel.Name} in guild {channel.Guild.Name}");
            }

            //lavalink play
            if (connection.CurrentState.CurrentTrack == null) await PlayQueue(connection);

            //await connection.PlayAsync(track);
        }
        
        private static async Task PlayQueue(LavalinkGuildConnection connection)
        {
            var guildId = connection.Guild.Id;

            var track = PlaysoundQueue.DequeueTrack(guildId);
            if (track == null) return;

            var lavaBufferMs = await AudioConfig.GetBufferDuration();
            

            connection.PlaybackStarted += async (senderConn, args) =>
            {
                Console.WriteLine($"Playback of track {args.Track.Title} started by {args.Player.ToString()}");
            };

            connection.PlaybackFinished += async (senderConn, args) =>
            {
                //Wait for buffer to finish
                await Task.Delay(lavaBufferMs);
                
                Console.WriteLine($"Number of queued tracks:{PlaysoundQueue.QueuedTracks(guildId).ToString()}");

                if (PlaysoundQueue.QueuedTracks(guildId) > 0 && args.Reason is TrackEndReason.Finished)
                {
                    var trackToPlay = PlaysoundQueue.DequeueTrack(guildId);
                    //await Task.Delay(200);
                    await connection.PlayAsync(trackToPlay);
                }

                else
                {
                    Console.WriteLine(
                        $"Playback of track finished {args.Track.Title}: Disconnected due to {args.Reason.ToString()}");
                    await senderConn.DisconnectAsync();
                }

                // if (args.Reason is TrackEndReason.Replaced)
                // {
                //     Console.WriteLine($"Changed playing track");
                // }
            };

            connection.TrackStuck += async (sender, args) =>
            {
                string message =
                    $"Track - {args.Track} was stuck for {args.ThresholdMilliseconds}ms at {args.Player.Channel.Name} in {args.Player.Guild.Name}";
                Console.WriteLine(message);
            };
            
            await connection.PlayAsync(track);

        }

        public static async Task LeaveVoice(CommandContext ctx)
        {
            var lavalink = ctx.Client.GetLavalink();
            var node = lavalink.GetIdealNodeConnection();

            var conn = lavalink.GetGuildConnection(ctx.Guild);
            if (conn == null) //No voice connections in guild
            {
                Console.WriteLine($"Bot is not connected to a voice channel in {ctx.Guild.Name}");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null) return;

            await conn.StopAsync();

            //await Task.Delay(250);

            if (conn.IsConnected) await conn.DisconnectAsync();
        }
    }
}