using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AudioLibrary;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using dnet = Discord;
using dnetAudio = Discord.Audio;
using dnetCommand = Discord.Commands;
using dnetSocket = Discord.WebSocket;

namespace CuteNoisesBot
{
    // public class NoiseModuleOld : dnetCommand.ModuleBase<dnetCommand.SocketCommandContext>
    // {
    //     //WARUNG BAKSO VOICE CHANNEL ID
    //     private const ulong _warungbakso = 719170777345294357;
    //     private const ulong _botOwnerId = 120843233864581120;
    //
    //     private bool _commandBeingExecuted = false;
    //
    //
    //
    //     //[Command("join", RunMode = RunMode.Async)]
    //     [dnetCommand.RequireOwner]
    //     public async Task ManualJoin()
    //     {
    //         //await JoinVoice((Context.User as IGuildUser)?.VoiceChannel);
    //
    //         //Get audio channel
    //         var voiceChannel = (Context.User as dnet.IGuildUser)?.VoiceChannel;
    //         if (voiceChannel == null)
    //         {
    //             await Context.Channel.SendMessageAsync(
    //                 $"{Context.User.Mention} : You must be in a voice channel first.");
    //             return;
    //         }
    //
    //         await JoinVoice(voiceChannel);
    //
    //         // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
    //         // if (voiceChannel != null)
    //         // {
    //         //     var audioClient = await voiceChannel.ConnectAsync();
    //         // }
    //     }
    //
    //     //[Command("leave")]
    //     [dnetCommand.RequireOwner]
    //     public async Task ManualLeave()
    //     {
    //         await LeaveVoice((Context.User as dnet.IGuildUser)?.VoiceChannel);
    //     }
    //
    //     private async Task<dnetAudio.IAudioClient> JoinVoice(dnet.IVoiceChannel channel)
    //     {
    //         //_connectedVoiceChannel = channel;
    //         //_audioClient = await channel.ConnectAsync(true);
    //         return await channel.ConnectAsync(true);
    //     }
    //
    //     private async Task LeaveVoice(dnet.IVoiceChannel channel)
    //     {
    //         await channel.DisconnectAsync();
    //     }
    //
    //     private async Task AnnounceNoise(dnetAudio.IAudioClient audioClient, string path)
    //     {
    //         
    //             Console.WriteLine($"Attempting to decode audio file {path}.");
    //             //var output = AudioStream.CreateStream(path).StandardOutput.BaseStream;
    //             using (var ffmpeg = AudioStream.CreateStream(path))
    //             await using (var output = ffmpeg.StandardOutput.BaseStream)
    //
    //             await using (var discord = audioClient.CreatePCMStream(dnetAudio.AudioApplication.Mixed))
    //             {
    //                 await output.CopyToAsync(discord);
    //                 await discord.FlushAsync().ConfigureAwait(false);
    //                 // try
    //                 // {
    //                 //     await output.CopyToAsync(discord);
    //                 // }
    //                 // finally
    //                 // {
    //                 //     await discord.FlushAsync();
    //                 // }
    //             }
    //             
    //             Console.WriteLine($"Output stream copied.");
    //             Console.WriteLine("Flushing stream.");
    //
    //         
    //
    //     }
    //     
    //
    //     //[Command("noise", RunMode = RunMode.Async)]
    //     public async Task PlayNoise(string noise)
    //     {
    //         if (_commandBeingExecuted) return;
    //         _commandBeingExecuted = true;
    //         
    //         await ExecutePlaysound(Context, noise);
    //
    //         _commandBeingExecuted = false;
    //     }
    //
    //     private async Task ExecutePlaysound(dnetCommand.SocketCommandContext context, string noise)
    //     {
    //         var ownerExecuted = context.User.Id == _botOwnerId;
    //         var userVoice = (context.User as dnet.IGuildUser)?.VoiceChannel;
    //         var botVoice = context.Guild.CurrentUser.VoiceChannel;
    //
    //         if (userVoice == null && ownerExecuted)
    //         {
    //             var bakso = context.Guild.GetVoiceChannel(_warungbakso);
    //             if (bakso == null) return;
    //             var userCount = bakso.Users.Count;
    //
    //             if (userCount == 0) return;
    //
    //             userVoice = bakso;
    //         }
    //
    //
    //         if (botVoice != null)
    //         {
    //             //Console.WriteLine($"Bot is already in a voice channel {botVoice}");
    //             return;
    //         }
    //
    //
    //         string path = await NoiseLibrary.GetNoise(noise);
    //
    //         //Couldnt find any path to the noises, ignore this command
    //         if (path == string.Empty) return;
    //
    //         //Stop execution for SFW mode if a NSFW playsound was request and send a message
    //         if (path.Contains("NSFW"))
    //         {
    //             await context.Channel.SendMessageAsync($"NSFW Playsound was requested.");
    //             return;
    //         }
    //         
    //         dnetAudio.IAudioClient voiceClient = await JoinVoice(userVoice);
    //
    //         // await Task.Delay(2000);
    //         await AnnounceNoise(voiceClient, path);
    //         await LeaveVoice(userVoice);
    //         return;
    //     }
    //
    //     //[Command("noise", RunMode = RunMode.Async)]
    //     public async Task PlayNoiseInvalid(params string[] args)
    //     {
    //         if (args.Length == 0)
    //         {
    //             //await Context.User.SendMessageAsync(await NoiseLibrary.GetAvailableCommands());
    //             return;
    //         }
    //
    //         if (args.Length >= 2)
    //         {
    //             return;
    //         }
    //     }
    //
    //     //[Command("noisesfw")]
    //     [RequirePermissions(Permissions.Administrator)]
    //     public async Task ToggleSFWAsync()
    //     {
    //         Globals.ToggleSFW();
    //         await Context.Channel.SendMessageAsync(Globals.SFWMessage());
    //         //await Context.Client.SetGameAsync(Globals.ActivityStatus(), null, ActivityType.Listening);
    //     }
    // }

    public class NoiseModule : BaseCommandModule
    {
        [Command("noise")]
        public async Task PlayNoise(CommandContext ctx, string sound)
        {
            
            await AudioLibrary.PlaysoundSystem.Play(ctx, await NoiseLibrary.GetNoise(sound));

            // if (sound == "korodisco")
            // {
            //     await PlayKoroDisco(ctx);
            //     return;
            // }

            // //Check if command invoker is in a voice channel
            // var channel = (ctx.Member.VoiceState?.Channel == null)
            //     ? null
            //     : ctx.Member.VoiceState
            //         .Channel;
            //
            // if (channel == null)
            // {
            //     return;
            // }
            //
            // var soundPath = await NoiseLibrary.GetNoise(sound);
            // if (string.IsNullOrEmpty(soundPath))
            // {
            //     Console.WriteLine($"Playsound -- {sound} not found");
            //     return;
            // }
            //
            // //get lavalink client
            // var lava = ctx.Client.GetLavalink();
            // if (lava == null)
            // {
            //     await ctx.RespondAsync("Lavalink not initalized.");
            //     return;
            // }
            //
            // var node = lava.GetIdealNodeConnection();
            // if (node == null)
            // {
            //     await ctx.RespondAsync("No node found.");
            //     return;
            // }
            //
            //
            // //load track to play
            // var result = await node.Rest.GetTracksAsync(new FileInfo(soundPath));
            // if (result.LoadResultType == LavalinkLoadResultType.LoadFailed
            //     || result.LoadResultType == LavalinkLoadResultType.NoMatches)
            // {
            //     await ctx.RespondAsync($"Playsound failed. Reason: {result.Exception.Message}.");
            //     return;
            // }
            //
            // var guildId = ctx.Guild.Id;
            // var track = result.Tracks.First();
            //
            // //Add track to queue
            // PlaysoundQueue.EnqueueTrack(guildId, track);
            //
            // //Check if bot is not already in a voice channel and playing
            // var connection = node.GetGuildConnection(ctx.Guild);
            // if (connection == null)
            // {
            //     //connect to channel
            //     connection = await node.ConnectAsync(channel);
            //     Console.WriteLine($"Connecting to voice channel {channel.Name} in guild {channel.Guild.Name}");
            // }
            //
            // //lavalink play
            // if (connection.CurrentState.CurrentTrack == null) await PlayQueue(connection);
        }

        // private async Task PlayQueue(LavalinkGuildConnection connection)
        // {
        //     var guildId = connection.Guild.Id;
        //
        //     var track = PlaysoundQueue.DequeueTrack(guildId);
        //     if (track == null) return;
        //     await connection.PlayAsync(track);
        //
        //     connection.PlaybackStarted += async (senderConn, args) =>
        //     {
        //         Console.WriteLine($"Playback of track {args.Track.Title} started by {args.Player.ToString()}");
        //     };
        //
        //     connection.PlaybackFinished += async (senderConn, args) =>
        //     {
        //         Console.WriteLine($"Number of queued tracks:{PlaysoundQueue.QueuedTracks(guildId).ToString()}");
        //
        //         if (PlaysoundQueue.QueuedTracks(guildId) > 0 && args.Reason is TrackEndReason.Finished)
        //         {
        //             var trackToPlay = PlaysoundQueue.DequeueTrack(guildId);
        //             await Task.Delay(200);
        //             await connection.PlayAsync(trackToPlay);
        //         }
        //
        //         else
        //         {
        //             Console.WriteLine(
        //                 $"Playback of track finished {args.Track.Title}: Disconnected due to {args.Reason.ToString()}");
        //             await senderConn.DisconnectAsync();
        //         }
        //
        //         // if (args.Reason is TrackEndReason.Replaced)
        //         // {
        //         //     Console.WriteLine($"Changed playing track");
        //         // }
        //     };
        // }


        // private async Task DisconnectAfterPlayback(LavalinkGuildConnection connection, TrackFinishEventArgs args)
        // {
        //     connection.PlaybackFinished -= DisconnectAfterPlayback;
        //     await connection.DisconnectAsync();
        // }

        [Command("noise")]
        public async Task PlayNoiseExtra(CommandContext ctx, params string[] t)
        {
            if (t.Length == 0 || (t.Length == 1 && string.IsNullOrWhiteSpace(t.First())))
            {
                await ctx.Member.SendMessageAsync(await NoiseLibrary.GetAvailableCommands());
            }
        }


        private async Task PlayKoroDisco(CommandContext ctx)
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

            var lavalink = ctx.Client.GetLavalink();
            var node = lavalink.GetIdealNodeConnection();


            //Search for korodisco
            var result = await node.Rest.GetTracksAsync(new Uri("https://youtu.be/mRTap9LP1jo"));
            if (result.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                Console.WriteLine($"No results found for Koro Disco on Youtube.");
                Console.WriteLine($"Reasons: {result.Exception.Message}");
                return;
            }

            var track = result.Tracks.First();

            var conn = await node.ConnectAsync(channel);

            conn.PlaybackFinished += async (lgc, args) => { await lgc.DisconnectAsync(); };


            await conn.PlayAsync(track);
        }

        [Command("noiseyeet")]
        [RequireUserPermissions(Permissions.Administrator)]
        // [RequirePermissions(Permissions.Administrator)]
        public async Task StopPlaying(CommandContext ctx)
        {
            // Console.WriteLine($"Yeeting the bot.");
            //
            // var lavalink = ctx.Client.GetLavalink();
            // var node = lavalink.GetIdealNodeConnection();
            //
            // var conn = lavalink.GetGuildConnection(ctx.Guild);
            // if (conn == null) //No voice connections in guild
            // {
            //     Console.WriteLine($"Bot is not connected to a voice channel in {ctx.Guild.Name}");
            //     return;
            // }
            //
            // if (conn.CurrentState.CurrentTrack == null) return;
            //
            // await conn.StopAsync();
            //
            // await Task.Delay(500);
            //
            // if (conn.IsConnected) await conn.DisconnectAsync();

            await AudioLibrary.PlaysoundSystem.LeaveVoice(ctx);
        }
    }
}