using System;
using System.Reflection;
using System.Threading.Tasks;
using AudioLibrary;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ActivityType = Discord.ActivityType;
using TokenType = Discord.TokenType;

namespace HololivePlaySound
{
    internal class HoloPlaysoundProgram
    {
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;
        private CommandService _commands;
        private IServiceProvider _services;

        public static void Main(string[] args)
        {
            //new HoloPlaysoundProgram().MainAsync().GetAwaiter().GetResult();
            
            new HoloPlaysoundProgram().MainAsyncDsharp().ConfigureAwait(false).GetAwaiter().GetResult();
        }

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
            //new AudioStream();

            await PlaysoundLibrary.ReloadDatabaseAsync();

            _client.Log += Log;
            _client.Ready += ClientReady;
            //_client.UserVoiceStateUpdated += VoiceJoinCuteNoiseAnnounce;

            var token = await TokenHandler.GetTokenAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();


            //Block this task until program is closed
            await Task.Delay(-1);
        }

        private Task ClientReady()
        {
            _client.SetGameAsync("!hololive", null, ActivityType.Listening);

            //await Task.Delay(2500);
            Console.WriteLine("Client is listening to commands");
            //_client.MessageReceived += CheckReceivedMessages;
            return Task.CompletedTask;
        }
        


        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<CommandService>()
               // .AddSingleton<PlaySoundModule>()
                .BuildServiceProvider();
        }
        
        //------------------------------------------------------------------------------------------------------------

        private DiscordClient _discordClient;
        
        private async Task MainAsyncDsharp()
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = await TokenHandler.GetTokenAsync(),
                TokenType = DSharpPlus.TokenType.Bot,
                MinimumLogLevel = LogLevel.Information
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

            await _discordClient.ConnectAsync(new DiscordActivity("!hololive", DSharpPlus.Entities.ActivityType.ListeningTo));
            await lavalink.ConnectAsync(lavalinkConfig);

            var services = new ServiceCollection()
                .AddSingleton<HololiveModule>()
                .BuildServiceProvider();
            
            var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] {"!"},
                Services = services,
                IgnoreExtraArguments = true
            });

            commands.RegisterCommands<HololiveModule>();

            await PlaysoundLibrary.ReloadDatabaseAsync();
            
            //commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // commands.CommandErrored += async (sender, args) =>
            // {
            //      //await args.Context.Channel.SendMessageAsync($"Command: {args.Command} failed. Reason: {args.Exception}");
            //      Console.WriteLine($"Command: {args.Command} failed. Reason: {args.Exception}");
            // };
            
            
            

            await Task.Delay(-1);
        }

        
    }
}