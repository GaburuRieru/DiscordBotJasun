using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
namespace CuteNoisesBot
{
    public class ServiceInitializer
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;

        	// Ask if there are existing CommandService and DiscordSocketClient

	// instance. If there are, we retrieve them and add them to the
	// DI container; if not, we create our own.
        
        public ServiceInitializer(CommandService commands = null, DiscordSocketClient client = null)
        {
            _commands = commands ?? new CommandService();
            _client = client ?? new DiscordSocketClient();
            
        }

        public IServiceProvider BuildServiceProvider => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<TestModule>()
            .BuildServiceProvider();
    }
}