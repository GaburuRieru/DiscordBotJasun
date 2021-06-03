using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using AudioLibrary;
//using Discord;
//using Discord.Commands;
//using Discord.WebSocket;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TokenType = Discord.TokenType;


namespace CuteNoisesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //new Program().MainAsync().GetAwaiter().GetResult();
            new Program().MainDsharpAsync().GetAwaiter().GetResult();
        }

        // private DiscordSocketClient _client;
        // private CommandService _commands;
        // private CommandHandler _commandHandler;
        // private IServiceProvider _services;
        //
        // private const string _prefix = "!";
        // private const string _joinRoom = "join";
        //
        // private const string _leaveRoom = "leave";
        //
        // private Task Log(LogMessage msg)
        // {
        //     Console.WriteLine(msg.ToString());
        //     return Task.CompletedTask;
        // }
        //
        // private async Task MainAsync()
        // {
        //     _client = new DiscordSocketClient();
        //
        //     _commands = new CommandService();
        //     _services = ConfigureServices();
        //     _commandHandler = new CommandHandler(_client, _commands, _services);
        //
        //     await _commandHandler.InstallCommandsAsync();
        //
        //     //Initialzie Audio Stuffs
        //     new AudioStream();
        //
        //     _client.Log += Log;
        //     _client.Ready += ClientReady;
        //
        //
        //     var token = await TokenHandler.GetTokenAsync();
        //
        //     await _client.LoginAsync(TokenType.Bot, token);
        //     await _client.StartAsync();
        //
        //
        //     //Block this task until program is closed
        //     await Task.Delay(-1);
        // }
        //
        // private Task ClientReady()
        // {
        //     _client.SetGameAsync(Globals.ActivityStatus(), null, ActivityType.Listening);
        //     
        //     //await Task.Delay(2500);
        //     Console.WriteLine("Client is listening to commands");
        //     //_client.MessageReceived += CheckReceivedMessages;
        //     return Task.CompletedTask;
        // }


        private DiscordClient _discordClient;

        private async Task MainDsharpAsync()
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = await TokenHandler.GetTokenAsync(),
                TokenType = DSharpPlus.TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            });

            var endpoint = new ConnectionEndpoint()
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration()
            {
                Password = "ruru",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = _discordClient.UseLavalink();

            await _discordClient.ConnectAsync(new DiscordActivity("!noise", ActivityType.ListeningTo));
            await lavalink.ConnectAsync(lavalinkConfig);

            var service = ConfigureServices();
            // var services = new ServiceCollection()
            //     .AddSingleton<NoiseModule>()
            //     .BuildServiceProvider();

            var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {"!nois"},
                Services = service
            });

            commands.RegisterCommands<NoiseModule>();
            //commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await Task.Delay(-1);
        }


        private ServiceProvider ConfigureServices() => new ServiceCollection()
            //.AddSingleton<DiscordSocketClient>()
            //.AddSingleton<CommandHandler>()
            //.AddSingleton<CommandService>()
            //.AddSingleton<TestModule>()
            //.AddSingleton<VoiceChannelSimpleModule>()
            .AddSingleton<NoiseModule>()
            .BuildServiceProvider();
    }
}