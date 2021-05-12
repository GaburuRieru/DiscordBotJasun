using System;
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
        //private readonly NoiseService _noise;

        //private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels =
        //new ConcurrentDictionary<ulong, IAudioClient>();

        private const string CommandPhrase = "hololive";
        
        //private IAudioClient _audioClient;

        //private IVoiceChannel _connectedVoiceChannel;


        // [Command("join", RunMode = RunMode.Async)]
        // [RequireOwner]
        // public async Task ManualJoin()
        // {
        //     //await JoinVoice((Context.User as IGuildUser)?.VoiceChannel);
        //
        //     //Get audio channel
        //     var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
        //     if (voiceChannel == null)
        //     {
        //         await Context.Channel.SendMessageAsync(
        //             $"{Context.User.Mention} : You must be in a voice channel first.");
        //         return;
        //     }
        //
        //     await JoinVoice(voiceChannel);
        //
        //     // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
        //     // if (voiceChannel != null)
        //     // {
        //     //     var audioClient = await voiceChannel.ConnectAsync();
        //     // }
        // }

        // [Command("leave")]
        // [RequireOwner]
        // public async Task ManualLeave()
        // {
        //     await LeaveVoice((Context.User as IGuildUser)?.VoiceChannel);
        // }

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

                // else
                // {
                //     Console.WriteLine($"No audio client found.");
                // }
            }
        }

        // public async Task CuteNoise(SocketUser user, DiscordSocketClient client)
        // {
        //     // Console.WriteLine("A user has joined, left or switched voice channel");
        //
        //     //if the client that joined is self or bot, ignore this event
        //     //if (user.IsBot) return;
        //
        //     //if bot is currently in a channel, ignore this event
        //     var isBotInChannel = ((client.CurrentUser as SocketUser) as IGuildUser)?.VoiceChannel != null;
        //     if (isBotInChannel)
        //     {
        //         Console.WriteLine("Bot is currently in a channel. CuteNoiseAnnounce Event ignored.");
        //         return;
        //     }
        //
        //     //If user is not in a channel (aka left the channel) we ignore this event
        //     var channel = (user as IGuildUser)?.VoiceChannel;
        //     if (channel == null) return;
        //
        //     Console.WriteLine($"{user.Username} joined a voice channel - {channel.Name}");
        //
        //     //if the channel already has at least one user prior to current user joining, ignore this event
        //     // if ((channel as SocketVoiceChannel)?.Users.Count > 2)
        //     // {
        //     //     Console.WriteLine($"{channel.Name} currently has more than 1 user, ignore this event call.");
        //     //     return;
        //     // }
        //
        //     //Console.WriteLine($"Voice channel currently only has that user");
        //     Console.WriteLine($"Bot will now attempt to join the channel and playsound");
        //
        //     // await Task.Run(async () =>
        //     // {
        //     IAudioClient voiceClient = await JoinVoice(channel);
        //     // await Task.Delay(1000);
        //     await AnnounceNoise(voiceClient, string.Empty);
        //     await LeaveVoice(channel);
        //     // });
        // }


        // private async Task SendNoise(string path)
        // {
        // }

        // [Command("cute", RunMode = RunMode.Async)]
        // public async Task PlayKlee()
        // {
        //     var userVoice = (Context.User as IGuildUser)?.VoiceChannel;
        //     if (userVoice == null) return;
        //     if (((Context.Client.CurrentUser as SocketUser) as IGuildUser)?.VoiceChannel != null) return;
        //     
        //     string path = "Klee_bombbomb.mp3";
        //
        //     IAudioClient voiceClient = await JoinVoice(userVoice);
        //
        //     // await Task.Delay(2000);
        //     await AnnounceNoise(voiceClient, path);
        //     await LeaveVoice(userVoice);
        // }

        [Command(CommandPhrase, RunMode = RunMode.Async)]
        public async Task PlayNoise(string noise)
        {
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
            if (args.Length == 0)
            {
                await Context.User.SendMessageAsync(await PlaysoundLibrary.GetAvailableCommands());
                return;
            }

            if (args.Length >= 2) return;
        }
    }
}