using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotLibrary;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace HololivePlaySound
{
    public class PlaySoundModule : ModuleBase<SocketCommandContext>
    {
        private const string CommandPhrase = "hololive";
        private const string ReloadCommand = "reload";

        //private Dictionary<ulong, ulong> _activeVoiceGuilds = new Dictionary<ulong, ulong>();
        private List<ulong> _voiceActiveGuilds = new List<ulong>();
        private bool _databaseReloading = false;

        private async Task<IAudioClient> JoinVoice(IVoiceChannel channel)
        {
            //_connectedVoiceChannel = channel;
            //_audioClient = await channel.ConnectAsync(true);
            return await channel.ConnectAsync(true);
        }

        private async Task LeaveVoice(IVoiceChannel channel)
        {
            await channel.DisconnectAsync();
           // _audioClient = null;
        }

        private async Task AnnounceNoise(IAudioClient audioClient, string path)
        {
            // if (_connectedChannels.TryGetValue(Context.Guild.Id, out IAudioClient audioClient))
            //if (_audioClient != null)
            {
                // await Task.Run(async () =>
                //{
               // Console.WriteLine($"Attempting to decode audio file {path}.");
                //var output = AudioStream.CreateStream(path).StandardOutput.BaseStream;
                using var ffmpeg = AudioStreamProcess.CreateStream(path);
                await using var output = ffmpeg.StandardOutput.BaseStream;
                await using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
                await output.CopyToAsync(discord);
                await discord.FlushAsync().ConfigureAwait(false);
                // try
                // {
                //     await output.CopyToAsync(discord);
                // }
                // finally
                // {
                //     await discord.FlushAsync();
                // }

                //Console.WriteLine($"Output Stream created: {output}");
                //var stream = _audioClient.CreateDirectPCMStream(AudioApplication.Mixed);
                //Console.WriteLine($"PCM stream created {stream}.");
                //await output.CopyToAsync(stream);
                //Console.WriteLine("Output stream copied.");
                //await stream.FlushAsync().ConfigureAwait(false);
               // Console.WriteLine("Flushing stream.");

                // });
                //}

                // {
                //     Console.WriteLine($"No audio client found.");
                // }
            }
        }
        

        [Command(CommandPhrase, RunMode = RunMode.Async)]
        public async Task PlayNoise(string noise)
        {
            //Bot is reloading noise database, ignore all commands
            if (_databaseReloading) return;

            //Tells bot to reload database
            if (noise == ReloadCommand)
            {
                ReloadDatabase();
                return;
            }

            var guildID = Context.Guild.Id;
            
            //Check if this guild is already executing bot commands
            if (_voiceActiveGuilds.Contains(guildID)) return;
            
            //Add guild into execution list so no conflicting commands
            _voiceActiveGuilds.Add(guildID);
            
            var userVoice = (Context.User as IGuildUser)?.VoiceChannel;
            if (userVoice == null) return;
           // if (((Context.Client.CurrentUser as SocketUser) as IGuildUser)?.VoiceChannel != null) return;
           
           //if bot is currently in a voice channel ignore this command
           var voice = Context.Guild.CurrentUser.VoiceChannel;
           if (voice != null)
           {
               Console.WriteLine($"Bot is currently in a voice channel {voice.Name} in guild {Context.Guild}");
               return;
           }
            
            var path = await PlaysoundLibrary.GetPlaysound(noise);

            //Couldnt find any path to the noises, ignore this command
            if (path == string.Empty) return;

            var voiceClient = await JoinVoice(userVoice);

            // await Task.Delay(2000);
            await AnnounceNoise(voiceClient, path);
            await LeaveVoice(userVoice);
        }

        [Command(CommandPhrase, RunMode = RunMode.Async)]
        public async Task PlayNoiseInvalid(params string[] args)
        {
            //Bot is reloading noise database, ignore all commands
            if (_databaseReloading) return;
            
            if (args.Length == 0)
            {
                await Context.User.SendMessageAsync(await PlaysoundLibrary.GetAvailableCommands());
                return;
            }

            if (args.Length >= 2) return;
        }

        private async Task ReloadDatabase()
        {
            _databaseReloading = true;
            await PlaysoundLibrary.ReloadDatabase();
            _databaseReloading = false;
        }
    }
}