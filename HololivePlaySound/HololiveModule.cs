using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AudioLibrary;
using Discord;
using Discord.Audio;
//using Discord.Commands;
using Discord.WebSocket;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using ChannelType = DSharpPlus.ChannelType;
using CommandContext = DSharpPlus.CommandsNext.CommandContext;

namespace HololivePlaySound
{
    // public class PlaySoundModule : ModuleBase<SocketCommandContext>
    // {
    //     private const string CommandPhrase = "hololive";
    //     private const string ReloadCommand = "reload";
    //
    //     //private Dictionary<ulong, ulong> _activeVoiceGuilds = new Dictionary<ulong, ulong>();
    //     private List<ulong> _voiceActiveGuilds = new List<ulong>();
    //     private bool _databaseReloading = false;
    //
    //     private async Task<IAudioClient> JoinVoice(IVoiceChannel channel)
    //     {
    //         //_connectedVoiceChannel = channel;
    //         //_audioClient = await channel.ConnectAsync(true);
    //         return await channel.ConnectAsync(true);
    //     }
    //
    //     private async Task LeaveVoice(IVoiceChannel channel)
    //     {
    //         await channel.DisconnectAsync();
    //         // _audioClient = null;
    //     }
    //
    //     private async Task AnnounceNoise(IAudioClient audioClient, string path)
    //     {
    //         // using var ffmpeg = Audio.CreateStream(path);
    //         // await using var output = ffmpeg.StandardOutput.BaseStream;
    //         // await using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
    //         // await output.CopyToAsync(discord);
    //         // await discord.FlushAsync().ConfigureAwait(false);
    //
    //         await RAWAudio.PlayPCM_RAW(path, audioClient);
    //     }
    //
    //
    //     [Command(CommandPhrase, RunMode = RunMode.Async)]
    //     public async Task PlayNoise(string noise)
    //     {
    //         //Bot is reloading noise database, ignore all commands
    //         if (_databaseReloading) return;
    //
    //         //Tells bot to reload database
    //         if (noise == ReloadCommand)
    //         {
    //             ReloadDatabase();
    //             return;
    //         }
    //
    //         var guildID = Context.Guild.Id;
    //
    //         //Check if this guild is already executing bot commands
    //         if (_voiceActiveGuilds.Contains(guildID)) return;
    //
    //         //Add guild into execution list so no conflicting commands
    //         _voiceActiveGuilds.Add(guildID);
    //
    //         var userVoice = (Context.User as IGuildUser)?.VoiceChannel;
    //         if (userVoice == null) return;
    //         // if (((Context.Client.CurrentUser as SocketUser) as IGuildUser)?.VoiceChannel != null) return;
    //
    //         //if bot is currently in a voice channel ignore this command
    //         var voice = Context.Guild.CurrentUser.VoiceChannel;
    //         if (voice != null)
    //         {
    //             Console.WriteLine($"Bot is currently in a voice channel {voice.Name} in guild {Context.Guild}");
    //             return;
    //         }
    //
    //         var path = await PlaysoundLibrary.GetPlaysound(noise);
    //
    //         //Couldnt find any path to the noises, ignore this command
    //         if (path == string.Empty) return;
    //
    //         var voiceClient = await JoinVoice(userVoice);
    //
    //         // await Task.Delay(2000);
    //         await AnnounceNoise(voiceClient, path);
    //         await LeaveVoice(userVoice);
    //     }
    //
    //     [Command(CommandPhrase, RunMode = RunMode.Async)]
    //     public async Task PlayNoiseInvalid(params string[] args)
    //     {
    //         //Bot is reloading noise database, ignore all commands
    //         if (_databaseReloading) return;
    //
    //         if (args.Length == 0)
    //         {
    //             await Context.User.SendMessageAsync(await PlaysoundLibrary.GetAvailableCommands());
    //             return;
    //         }
    //
    //         if (args.Length >= 2) return;
    //     }
    //
    //     private async Task ReloadDatabase()
    //     {
    //         _databaseReloading = true;
    //         await PlaysoundLibrary.ReloadDatabase();
    //         _databaseReloading = false;
    //     }
    // }

    public class HololiveModule : BaseCommandModule

    {
        [Command("test")]
        [RequireOwner]
        public async Task TestCommand(CommandContext ctx, string extraMessage)
        {
            await ctx.RespondAsync($"Hi! This is the extra message: {extraMessage}.");
        }
        
        [Command("hololive")]
        public async Task Playsound(CommandContext ctx, string playsound)
        {
            Console.WriteLine("getting sound");
            var soundPath = await GetPlaysoundPathAsync(playsound);
            if (string.IsNullOrEmpty(soundPath))
            {
                Console.WriteLine($"Playsound -- {playsound} not found");
                return;
            }

            await PlaysoundSystem.Play(ctx, soundPath, Cdn.ContentDomain);

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

            // var soundPath = await GetPlaysoundPathAsync(playsound);
            // if (string.IsNullOrEmpty(soundPath))
            // {
            //     Console.WriteLine($"Playsound -- {playsound} not found");
            //     return;
            // }

            // //get lavalink client
            // var lava = ctx.Client.GetLavalink();
            // if (lava == null)
            // {
            //     await ctx.RespondAsync("Lavalink not initalized.");
            //     return;
            // }

            // //get node
            // //var node = await GetLavalinkNodeAsync(lava);
            // var node = lava.GetIdealNodeConnection();
            // if (node == null)
            // {
            //     await ctx.RespondAsync("No node found.");
            //     return;
            // }


            // //load track to play
            // var result = await node.Rest.GetTracksAsync(new FileInfo(soundPath));
            // if (result.LoadResultType == LavalinkLoadResultType.LoadFailed
            //     || result.LoadResultType == LavalinkLoadResultType.NoMatches)
            // {
            //     await ctx.RespondAsync($"Playsound failed. Reason: {result.Exception.Message}.");
            //     return;
            // }

            // var track = result.Tracks.First();
            //
            // //connect to channel
            // var conn = await node.ConnectAsync(channel);
            //
            // //add to queue 
            //
            //
            // //lavalink play
            // //await conn.PlayAsync(result.Tracks.First());
            //
            //
            // // Console.WriteLine($"Playing playsound: {playsound}");
            // // Console.WriteLine($"Path: {soundPath}");
            // // Console.WriteLine($"Playsound details: \r\n" +
            // //                   $"Author: {track.Author} \r\n" +
            // //                   $"Title: {track.Title} \r\n" +
            // //                   $"Length: {track.Length}");
            //
            //
            // conn.PlaybackFinished += async (c, args) => { await c.DisconnectAsync(); };

            // await LeaveVoiceChannelAsync(ctx);
        }

        //[Command("holotest")]
        [RequireOwner]
        public async Task Playsound(CommandContext ctx)
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

            //get lavalink client
            var lava = ctx.Client.GetLavalink();
            if (lava == null)
            {
                await ctx.RespondAsync("Lavalink not initalized.");
                return;
            }

            //get node
            //var node = await GetLavalinkNodeAsync(lava);
            var node = lava.GetIdealNodeConnection();
            if (node == null)
            {
                await ctx.RespondAsync("No node found.");
                return;
            }


            //load track to play
            //var result = await node.Rest.GetTracksAsync(new FileInfo(soundPath));
            var result = await node.Rest.GetTracksAsync(("koro disco"), LavalinkSearchType.Youtube);
            if (result.LoadResultType is LavalinkLoadResultType.LoadFailed)
            {
                await ctx.RespondAsync($"Load Failed. Reason: {result.Exception.Message}.");
                return;
            }

            if (result.LoadResultType == LavalinkLoadResultType.SearchResult)
            {
                await ctx.RespondAsync($"Loaded stuffs!");
            }

            // if (result.LoadResultType == LavalinkLoadResultType.NoMatches)
            // {
            //     await ctx.RespondAsync($"No matches");
            //     return;
            // }


            //connect to channel
            var conn = await node.ConnectAsync(channel);

            //lavalink play
            var track = result.Tracks.First();
            await conn.PlayAsync(track);

            conn.PlaybackFinished += async (sender, args) => { await sender.DisconnectAsync(); };

            // Console.WriteLine($"Playing playsound: {playsound}");
            // Console.WriteLine($"Path: {soundPath}");
            // Console.WriteLine($"Playsound details: \r\n" +
            //                   $"Author: {track.Author} \r\n" +
            //                   $"Title: {track.Title} \r\n" +
            //                   $"Length: {track.Length}");

            //await LeaveVoiceChannelAsync(ctx);
        }

        // [Command("holostop")]
        // [RequireOwner]
        // public async Task StopTest(CommandContext ctx)
        // {
        //     var lava = ctx.Client.GetLavalink();
        //     var node = lava.GetIdealNodeConnection();
        //     var conn = node.GetGuildConnection(ctx.Member.Guild);
        //     await conn.StopAsync();
        //     await conn.DisconnectAsync();
        // }

        // private async Task<LavalinkNodeConnection> GetLavalinkNodeAsync(LavalinkExtension lava)
        // {
        //     if (!lava.ConnectedNodes.Any())
        //     {
        //         //await ctx.RespondAsync("Lavalink connection not established");
        //         return null;
        //     }
        //
        //     return lava.GetIdealNodeConnection();
        // }


        private async Task<bool> LeaveVoiceChannelAsync(CommandContext ctx)
        {
            var channel = ctx.Member.VoiceState.Channel;

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("Lavalink connection not established");
                return false;
            }

            //var node = lava.ConnectedNodes.Values.First();
            var node = lava.GetIdealNodeConnection();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return false;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return false;
            }

            await conn.DisconnectAsync();
            return true;
        }

        // private async Task DisconnectAfterFinishPlayback(LavalinkGuildConnection connection, TrackFinishEventArgs t)
        // {
        //     connection.
        // }

        private async Task<string> GetPlaysoundPathAsync(string sound)
        {
            return await PlaysoundLibrary.GetPlaysoundAsync(sound);
        }

        [Command("hololive")]
        public async Task SoundCommandsAsync(CommandContext ctx)
        {
            var commands = await PlaysoundLibrary.GetAvailableCommandsAsync();
            await ctx.Member.SendMessageAsync($"Available playsounds: {commands}");
        }

        [Command("holoyeet")]
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
            // await conn.StopAsync();
            //
            // await Task.Delay(250);
            // if (conn.CurrentState.CurrentTrack == null) return;
            //
            // if (conn.IsConnected) await conn.DisconnectAsync();

            await PlaysoundSystem.LeaveVoice(ctx);
        }

        [Command("node")]
        [RequireOwner]
        public async Task GetNode(CommandContext ctx)
        {
            await PlaysoundSystem.SeeNode(ctx);
        }

        // [Command("!holoreload")]
        // [RequireOwner]
        // public async Task ReloadPlaysoundDatabase(CommandContext ctx)
        // {
        //     await PlaysoundLibrary.ReloadDatabaseAsync();
        // }
    }
}