using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace CuteNoisesBot
{
    public class NovaModule : ModuleBase<SocketCommandContext>
    {        
        //private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels =
        //new ConcurrentDictionary<ulong, IAudioClient>();

        private IVoiceChannel _connectedVoiceChannel;
        private IAudioClient _audioClient;

        private DiscordSocketClient _client;
        //private readonly NoiseService _service;

        public NovaModule(DiscordSocketClient client)
        {
            _client = client;
            Console.WriteLine(_client);
        }
        
        [Command("join", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task ManualJoin()
        {
            //await JoinVoice((Context.User as IGuildUser)?.VoiceChannel);

            //Get audio channel
            var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
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
        [RequireOwner]
        public async Task ManualLeave()
        {
            await LeaveVoice();
        }

        private async Task JoinVoice(IVoiceChannel channel)
        {
            _connectedVoiceChannel = channel;
            _audioClient = await channel.ConnectAsync(true);
        }

        private async Task LeaveVoice()
        {
            await _connectedVoiceChannel.DisconnectAsync();
            _connectedVoiceChannel = null;
            _audioClient = null;
        }
        
        private async Task AnnounceNova()
        {
            string path = "novanova.mp3";

            // if (_connectedChannels.TryGetValue(Context.Guild.Id, out IAudioClient audioClient))
            if (_audioClient != null)
            {
                await Task.Run(async () =>
                {
                    Console.WriteLine($"Attempting to decode audio file {path}.");
                    //var output = AudioStream.CreateStream(path).StandardOutput.BaseStream;
                    using (var ffmpeg = AudioStream.CreateStream(path))
                    using (var output = ffmpeg.StandardOutput.BaseStream)

                    using (var discord = _audioClient.CreatePCMStream(AudioApplication.Voice))
                    {
                        try
                        {
                            await output.CopyToAsync(discord);
                        }
                        finally
                        {
                            await discord.FlushAsync();
                        }
                    }

                    //Console.WriteLine($"Output Stream created: {output}");
                    //var stream = _audioClient.CreateDirectPCMStream(AudioApplication.Mixed);
                    //Console.WriteLine($"PCM stream created {stream}.");
                    //await output.CopyToAsync(stream);
                    Console.WriteLine($"Output stream copied.");
                    //await stream.FlushAsync().ConfigureAwait(false);
                    Console.WriteLine("Flushing stream.");

                });
            }

            else
            {
                Console.WriteLine($"No audio client found.");
            }
        }

        public async Task NovaNova(SocketUser user)
        {
            //if the client that joined is self or bot, ignore this event
            if (user.IsBot) return;
            
            //if bot is currently in a channel, ignore this event
            var isBotInChannel = ((_client.CurrentUser as SocketUser) as IGuildUser)?.VoiceChannel != null;
            if (isBotInChannel)
            {
                Console.WriteLine("Bot is currently in a channel. CuteNoiseAnnounce Event ignored.");
                return;
            }
            
            //If user is not in a channel (aka left the channel) we ignore this event
            var channel = (user as IGuildUser)?.VoiceChannel;
            if (channel == null) return;

            Console.WriteLine($"{user.Username} joined a voice channel - {channel.Name}");

            Console.WriteLine($"Attempting to join channel and play Nova");
            
            await JoinVoice(channel);
           // await Task.Delay(1000);
            await AnnounceNova();
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
            await AnnounceNova();
            await LeaveVoice();
        }
    }
    
    
}