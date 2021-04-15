using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


namespace CuteNoisesBot
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        // say !test -> Test OK!
        [Command("test")]
        [Summary("Sends a test message")]
        public async Task TestAsync()
        {
            await ReplyAsync("Module Test Successful");
        }
    }

    public class JoinVoiceModule : ModuleBase<SocketCommandContext>
    {
        // The command's Run Mode MUST be set to RunMode.Async, otherwise, being connected to a voice channel will block the gateway thread.
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinVoiceAsync(IVoiceChannel voiceChannel = null)
        {
            //Get audio channel
            voiceChannel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (voiceChannel == null)
            {
                await Context.Channel.SendMessageAsync(
                    $"{Context.User.Mention} : You must be in a voice channel first.");
            }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            if (voiceChannel != null)
            {
                var audioClient = await voiceChannel.ConnectAsync();
            }
        }
    }

    public class LeaveVoiceModule : ModuleBase<SocketCommandContext>
    {
        [Command("leave")]
        public async Task LeaveVoiceAsync()
        {
            //Get the voice channel the command user is in
            var userVoiceChannel = (Context.User as IGuildUser)?.VoiceChannel;

            //if user is not in a voice channel we exit
            if (userVoiceChannel == null)
            {
                Console.WriteLine($"{Context.User.Username} is not in a voice channel. Code Exit");
                return;
            }

            //Get the voice channel the bot is in
            var botVoiceChannel = (await Context.Channel.GetUserAsync(Context.Client.CurrentUser.Id) as IGuildUser)
                ?.VoiceChannel;
            
            //if bot is not in a voice channel we exit
            if (botVoiceChannel == null)
            {
                Console.WriteLine("Bot is not in a voice channel. Code Exit.");
                return;
            }
            
            //Check if command user and bot are in the same channel
            //And execute leave voice channel if true
            if (userVoiceChannel.Id == botVoiceChannel.Id)
            {
                await userVoiceChannel.DisconnectAsync();
                Console.WriteLine($"Bot disconnected from {botVoiceChannel}");
            }
        }
    }

    public class TestPlayNoiseModule : ModuleBase<SocketCommandContext>
    {
        private IServiceProvider _services;

        public TestPlayNoiseModule(IServiceProvider services)
        {
            _services = services;
        }

        [Command("testplay", RunMode = RunMode.Async)]
        public async Task PlayTest()
        {
            
        }
    }
}