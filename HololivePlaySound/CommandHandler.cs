using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace HololivePlaySound
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        //Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }


        public async Task InstallCommandsAsync()
        {
            //Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            //Hook CommandExecuted to handle post-execution logic
            _commands.CommandExecuted += CommandExecutedAsync;

            _commands.Log += LogAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            //Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;

            //Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            //Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            //Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            //Execute the command with the command context we just 
            //created, along with the service provider for precondition checks
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }

        private async Task LogAsync(LogMessage logMessage)
        {
            if (logMessage.Exception is CommandException cmdException)
            {
                //Send a message in discord notifying something went wrong
                await cmdException.Context.Channel.SendMessageAsync(
                    "Something went horribly wrong! Check the console.");

                //Log to console
                Console.WriteLine(
                    $"{cmdException.Context.User} failed to execute {cmdException.Command.Name} in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException.ToString());
            }

            else
            {
                Console.WriteLine(logMessage);
            }
        }
    }
}