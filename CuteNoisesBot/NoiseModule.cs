using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using DiscordExample;

namespace CuteNoisesBot
{
    public class NoiseModule : ModuleBase<SocketCommandContext>
    {
        //private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels =
        //new ConcurrentDictionary<ulong, IAudioClient>();

        private IVoiceChannel _connectedVoiceChannel;
        private IAudioClient _audioClient;

        private DiscordSocketClient _client;
        //private readonly NoiseService _service;

        public NoiseModule(DiscordSocketClient client)
        {
            _client = client;
            _client.UserVoiceStateUpdated += NoiseOnEmptyAnnounce;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task ManualJoin()
        {
            //await JoinVoice((Context.User as IGuildUser)?.VoiceChannel);

            IVoiceChannel voiceChannel = null;

            //Get audio channel
            voiceChannel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (voiceChannel == null)
            {
                await Context.Channel.SendMessageAsync(
                    $"{Context.User.Mention} : You must be in a voice channel first.");
                return;
            }

            await JoinVoice(voiceChannel);

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            // if (voiceChannel != null)
            // {
            //     var audioClient = await voiceChannel.ConnectAsync();
            // }
        }

        [Command("leave")]
        public async Task ManualLeave()
        {
            //await LeaveVoice();
        }

        private async Task JoinVoice(IVoiceChannel channel)
        {
            _audioClient = await channel.ConnectAsync(true);
            _connectedVoiceChannel = channel;
        }

        private async Task LeaveVoice()
        {
            // //Get the voice channel the command user is in
            // var userVoiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
            //
            // //if user is not in a voice channel we exit
            // if (userVoiceChannel == null)
            // {
            //     Console.WriteLine($"{Context.User.Username} is not in a voice channel. Code Exit");
            //     return;
            // }
            //
            // //Get the voice channel the bot is in
            // var botVoiceChannel = (await Context.Channel.GetUserAsync(Context.Client.CurrentUser.Id) as IGuildUser)
            //     ?.VoiceChannel;
            //
            // //if bot is not in a voice channel we exit
            // if (botVoiceChannel == null)
            // {
            //     Console.WriteLine("Bot is not in a voice channel. Code Exit.");
            //     return;
            // }
            //
            // //Check if command user and bot are in the same channel
            // //And execute leave voice channel if true
            // if (userVoiceChannel.Id == botVoiceChannel.Id)
            // {
            //     await userVoiceChannel.DisconnectAsync();
            //     Console.WriteLine($"Bot disconnected from {botVoiceChannel}");
            // }

            await _connectedVoiceChannel.DisconnectAsync();
            _connectedVoiceChannel = null;
            _audioClient = null;
        }
        
        public async Task AnnounceNoise()
        {
            string path = "Klee_bombbomb.mp3";

            // if (_connectedChannels.TryGetValue(Context.Guild.Id, out IAudioClient audioClient))
            if (_audioClient != null)
            {
                Console.WriteLine($"Attempting to decode audio file {path}.");
                //var output = AudioStream.CreateStream(path).StandardOutput.BaseStream;
                using (var ffmpeg = AudioStream.CreateStream(path))
                using (var output = ffmpeg.StandardOutput.BaseStream)

                using (var discord = _audioClient.CreatePCMStream(AudioApplication.Voice))
                {
                    try { await output.CopyToAsync(discord); }
                    finally { await discord.FlushAsync(); }
                }
                //Console.WriteLine($"Output Stream created: {output}");
                //var stream = _audioClient.CreateDirectPCMStream(AudioApplication.Mixed);
                //Console.WriteLine($"PCM stream created {stream}.");
                //await output.CopyToAsync(stream);
                Console.WriteLine($"Output stream copied.");
                //await stream.FlushAsync().ConfigureAwait(false);
                Console.WriteLine("Flushing stream.");
                
 
            }

            else
            {
                Console.WriteLine($"No audio client found.");
            }
        }

        public async Task NoiseOnEmptyAnnounce(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            Console.WriteLine("A user has joined, left or switched voice channel");

            //var channel = (user as IGuildUser)?.VoiceChannel;
            var channel = state1.VoiceChannel;

            //If user is not in a channel we ignore this event
            if (channel == null) return;

            //if the channel already has at least one user prior to current user joining, ignore this event
            if (channel.Users.Count != 1) return;

            //if bot is currently in a channel, ignore this event
            if (_connectedVoiceChannel != null) return;

            Console.WriteLine($"{user.Username} joined a voice channel - {channel.Name}");
            Console.WriteLine($"Voice channel currently only has that user");
            Console.WriteLine($"Bot will now attempt to join the channel");

            await JoinVoice(channel);
            await Task.Delay(1000);
            await AnnounceNoise();
            await LeaveVoice();
        }

        private async Task SendNoise(string path)
        {
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task TestPlay()
        {
            var userVoice = (Context.User as IGuildUser)?.VoiceChannel;

            if (userVoice == null) return;

            await JoinVoice(userVoice);
          // await Task.Delay(2000);
            await AnnounceNoise();
            await LeaveVoice();
        }
    }
    
    
}