using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace CuteNoisesBot
{
    public class VoiceChannelSimpleModule : ModuleBase<SocketCommandContext>
    {
        // The command's Run Mode MUST be set to RunMode.Async, otherwise, being connected to a voice channel will block the gateway thread.
        [Command("testjoin", RunMode = RunMode.Async)]
        [RequireOwner]
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
        
        
        [Command("testleave")]
        [RequireOwner]
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
    
}