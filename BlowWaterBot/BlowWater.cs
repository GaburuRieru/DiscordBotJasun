using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BlowWaterBot
{
    public class BlowWater
    {
        //private const bool TEST_MODE = true;
        
        
        public static void Main(string[] args)
        => new BlowWater().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private const string YaokunCommand = "!water";
        private WaterSource _waterSource;
        private CommandHandler _commandHandler;
        private CommandService _commands;
        private IServiceProvider _services;

        private async Task MainAsync()
        {

            
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.Ready += OnReady;
            
            
            await Initialize();
            
            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository
            var token = "ODMxMDg0NTcxNTQ5NjMwNDc3.YHQF0g.TF6YyaFj4Pkc2DY8pdCDGoOLMb4";
            
            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

  
            
            //Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Initialize()
        {
            _waterSource = new WaterSource();
            await _waterSource.Initialization();

            _commands = new CommandService();
            _services = ConfigureServices();
            _commandHandler = new CommandHandler(_client, _commands, _services);

            await _commandHandler.InstallCommandsAsync();
        }
        
        private async Task OnReady()
        {
            
            await _client.SetGameAsync(YaokunCommand, null, ActivityType.Listening);
            _client.MessageReceived += SendRandomBlowWater;
            return;
        }

        private Task SendRandomBlowWater(SocketMessage message)
        {
            //SocketUserMessage userMessage = message as SocketUserMessage;

            ISocketMessageChannel messageChannel = message.Channel;
            string content = message.Content;
            
            if(content.Equals(YaokunCommand))
            {
                string water = _waterSource.GetRandomWater();
                messageChannel.SendMessageAsync(water);
            }
            
            return Task.CompletedTask;
        }
        
        // private Task TestMessageSendAsync()
        // {
        //     ulong channelId = 831121645526712362;
        //     string message = "Test send message via bot";
        //
        //     
        //     IMessageChannel channel = _client.GetChannel(channelId) as IMessageChannel;
        //     if (channel != null)
        //     {
        //         Console.WriteLine("Sending Message");
        //         channel?.SendMessageAsync(message, false, null, null, null, null);
        //     }
        //
        //     return Task.CompletedTask;
        // }

        // private Task GetCurrentGuild()
        // {
        //     
        // }

        // private async Task RetrieveAllGroups()
        // {
        //    //SocketChannel channel = _client.GetGroupChannelsAsync()
        // }
        
        private ServiceProvider ConfigureServices() => new ServiceCollection()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<CommandService>()
            .AddSingleton<WaterModule>()
            .BuildServiceProvider();

        #region Logging

                private Task Log(LogMessage msg)
                {
                    Console.WriteLine(msg.ToString());
                    return Task.CompletedTask;
                }

        #endregion

    }
}
