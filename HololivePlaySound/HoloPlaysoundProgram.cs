using System;
using System.Linq;
using System.Reflection;
using System.Text;
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
    class HoloPlaysoundProgram
    {
        // private DiscordSocketClient _client;
        // private CommandHandler _commandHandler;
        // private CommandService _commands;
        // private IServiceProvider _services;

        public static void Main(string[] args)
        {
            //new HoloPlaysoundProgram().MainAsync().GetAwaiter().GetResult();
            
            new HoloPlaysoundProgram().MainAsyncDsharp().ConfigureAwait(false).GetAwaiter().GetResult();
        }

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
        //     //new AudioStream();
        //
        //     //await PlaysoundLibrary.ReloadDatabaseAsync();
        //
        //     _client.Log += Log;
        //     _client.Ready += ClientReady;
        //     //_client.UserVoiceStateUpdated += VoiceJoinCuteNoiseAnnounce;
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
        //     _client.SetGameAsync("!hololive", null, ActivityType.Listening);
        //
        //     //await Task.Delay(2500);
        //     Console.WriteLine("Client is listening to commands");
        //     //_client.MessageReceived += CheckReceivedMessages;
        //     return Task.CompletedTask;
        // }
        //
        //
        //
        // private ServiceProvider ConfigureServices()
        // {
        //     return new ServiceCollection()
        //         .AddSingleton<DiscordSocketClient>()
        //         .AddSingleton<CommandHandler>()
        //         .AddSingleton<CommandService>()
        //        // .AddSingleton<PlaySoundModule>()
        //         .BuildServiceProvider();
        // }
        
        //------------------------------------------------------------------------------------------------------------

        private DiscordClient _discordClient;
        
        private async Task MainAsyncDsharp()
        {
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = await TokenHandler.GetTokenAsync(),
                TokenType = DSharpPlus.TokenType.Bot,
                MinimumLogLevel = LogLevel.Information,
                ShardCount = 1,
                ShardId = 0,
            });
            
            await _discordClient.ConnectAsync(new DiscordActivity("!hololive", DSharpPlus.Entities.ActivityType.ListeningTo));

            //Get all regions available to the client
            var voiceRegions = await _discordClient.ListVoiceRegionsAsync();
            // var regions = _discordClient.VoiceRegions;
            //
            // var regionsString = new StringBuilder("List of regions from IDictionary: ");
            // foreach (var region in regions)
            // {
            //     regionsString.Append($" {region.Key}");
            // }
            //
            // await _discordClient.SendMessageAsync(
            //     ((await _discordClient.GetGuildAsync(831121644575260703)).GetChannel(848232664548376627)),
            //     regionsString.ToString());
            //
            //
            // var voiceString = new StringBuilder($"List of regions from Async: {voiceRegions.Count}");
            // foreach (var region in voiceRegions)
            // {
            //     voiceString.Append($"({region.Id},{region.Name})");
            // }
            //
            // await _discordClient.SendMessageAsync(
            //     ((await _discordClient.GetGuildAsync(831121644575260703)).GetChannel(848232664548376627)),
            //     voiceString.ToString());

            var lavaNodeHost = await LavalinkConnection.GetNode("Singapore");

            if (lavaNodeHost == null)
            {
                Console.WriteLine($"No lavalink node config found.");
                Console.WriteLine("Abort bot");
                return;
            }
            
            
            
            var endpointSg = new ConnectionEndpoint()
            {
                Hostname = lavaNodeHost.Hostname,
                Port = lavaNodeHost.Port
            };

            var lavalinkConfigSg = new LavalinkConfiguration()
            {
                Password = lavaNodeHost.Password,
                RestEndpoint = endpointSg,
                SocketEndpoint = endpointSg,
                Region = voiceRegions.First(x => x.Id == lavaNodeHost.RegionId) 
            };

            var lavalink = _discordClient.UseLavalink();

            await lavalink.ConnectAsync(lavalinkConfigSg);

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

            //await PlaysoundLibrary.ReloadDatabaseAsync();
            
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