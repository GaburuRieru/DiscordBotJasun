using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace CuteNoisesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _commandHandler;
        private IServiceProvider _services;

        private const string _prefix = "!";
        private const string _joinRoom = "join";

        private const string _leaveRoom = "leave";

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _commands = new CommandService();
            _services = ConfigureServices();
            _commandHandler = new CommandHandler(_client, _commands, _services);

            await _commandHandler.InstallCommandsAsync();

            //Initialzie Audio Stuffs
            new AudioStream();

            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.UserVoiceStateUpdated += VoiceJoinCuteNoiseAnnounce;

            var token = await TokenHandler.GetTokenAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();


            //Block this task until program is closed
            await Task.Delay(-1);
        }

        private Task ClientReady()
        {
            _client.SetGameAsync("!noise", null, ActivityType.Listening);
            
            //await Task.Delay(2500);
            Console.WriteLine("Client is listening to commands");
            //_client.MessageReceived += CheckReceivedMessages;
            return Task.CompletedTask;
        }

        private Task VoiceJoinCuteNoiseAnnounce(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            // Task.Run(async () =>
            // {
            //     Console.WriteLine("A user has joined, left or switched voice channel");
            //
            //     Console.WriteLine("Calling the sub-Module command responsible for this");
            //
            //     var noise = _services.GetService<NoiseModule>();
            //     await noise.CuteNoise(user);
            // });

            if (state1.VoiceChannel == null)
            {
                Console.WriteLine($"State1 is null");
            }
            else
            {
                Console.WriteLine($"{state1.VoiceChannel.Name}");
            }

            if (state2.VoiceChannel == null)
            {
                Console.WriteLine($"State2 is null");
            }
            else
            {
                Console.WriteLine($"{state2.VoiceChannel.Name}");
            }
            

            return Task.CompletedTask;
            
            //If the client that joined is a bot (including self), ignore this event
            if (user.IsBot) return Task.CompletedTask;
            
            Console.WriteLine("A user has joined, left or switched voice channel");

            Console.WriteLine("Calling the sub-Module command responsible for this");

            //DoCuteNoise(user);

            return Task.CompletedTask;

        }

        private Task DoCuteNoise(SocketUser user)
        {
            _services.GetService<NoiseModule>().CuteNoise(user, _client);
            return Task.CompletedTask;
        }

        #region Unused Old testing code

        // private Task CheckReceivedMessages(SocketMessage message)
        // {
        //     var channel = message.Channel;
        //     var user = message.Author;
        //     var content = message.Content;
        //
        //     if (user == (SocketUser) _client.CurrentUser) //return Task.CompletedTask;
        //
        //         if (!content.StartsWith(_prefix)) //return Task.CompletedTask;
        //
        //             if (content.Contains(_joinRoom))
        //             {
        //                 var voice = ((SocketGuildUser) user).VoiceChannel;
        //                 if (voice == null)
        //                 {
        //                    channel.SendMessageAsync(user.Mention + ": you are not in a voice channel!");
        //                     //return Task.CompletedTask;
        //                 }
        //
        //                 SocketGuildUser self = voice.GetUser(_client.CurrentUser.Id);
        //                 if (self != null)
        //                 {
        //                     channel.SendMessageAsync(user.Mention + ": I am already in your voice channel!");
        //                     // return Task.CompletedTask;
        //                 }
        //
        //                 ConnectToVoiceChannel(voice);
        //             }
        //
        //     if (content.Contains(_leaveRoom))
        //     {
        //         var voice = ((SocketGuildUser) user).VoiceChannel;
        //         if (voice == null)
        //         {
        //            channel.SendMessageAsync(user.Mention + ": you are not in a voice channel!");
        //             //return Task.CompletedTask;
        //         }
        //
        //         DisconnectFromVoiceChannel(voice);
        //     }
        //
        //     return Task.CompletedTask;
        // }
        //
        // private async Task ConnectToVoiceChannel(IAudioChannel audioChannel)
        // {
        //     await audioChannel.ConnectAsync(true);
        // }
        //
        // private async Task DisconnectFromVoiceChannel(IAudioChannel audioChannel)
        // {
        //     // var idself = audioChannel.GetUserAsync(_client.CurrentUser.Id);
        //     // if
        //
        //     await audioChannel.DisconnectAsync();
        // }
        //
        // [Command("join", RunMode = RunMode.Async)]
        // public async Task JoinChannel(IAudioChannel audioChannel)
        // {
        //     
        // }

        #endregion

        private ServiceProvider ConfigureServices() => new ServiceCollection()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<CommandService>()
            .AddSingleton<TestModule>()
            .AddSingleton<VoiceChannelSimpleModule>()
            .AddSingleton<NoiseModule>()
            .BuildServiceProvider();
    }
}