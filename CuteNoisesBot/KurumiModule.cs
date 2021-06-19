using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace CuteNoisesBot
{
    public class KurumiModule : BaseCommandModule
    {

        private static int Cooldown = 60;
        private int _cooldown;
        private bool _moduleAvailable = true;

        [Command("kurumi")]
        public async Task KurumiPic(CommandContext ctx)
        {
            if (ctx.Member.IsBot) return;
            
            if (!_moduleAvailable)
            {
                await ctx.RespondAsync($"!Kurumi is currently on cooldown. Time remaining: {_cooldown.ToString()}s.");
                return;
            }

            ModuleCooldown();
            
            const string kuru = "https://pbs.twimg.com/profile_images/1389334615979032581/TII0GRz9.jpg";
            const string kuruSmile = "https://instagram.fkul8-1.fna.fbcdn.net/v/t51.2885-15/e35/194380710_1025109481226667_3324846550204340503_n.jpg?tp=1&_nc_ht=instagram.fkul8-1.fna.fbcdn.net&_nc_cat=103&_nc_ohc=zYAcc7vcLdMAX_8YZd0&edm=AABBvjUBAAAA&ccb=7-4&oh=c7d3775d0a53035d737d74a8921708de&oe=60D4B45D&_nc_sid=83d603";

            //var akkun = (await ctx.Guild.GetMemberAsync(120843233864581120));
            var akkun = await ctx.Client.GetUserAsync(805956503113564190);

            if (akkun == null) return;
            
            var mention = new UserMention(akkun);
            
             var embed = new DiscordEmbedBuilder().WithImageUrl(
                 new Uri(kuruSmile)).Build();

             var message = await new DiscordMessageBuilder().WithContent($"{akkun.Mention} xd").WithAllowedMention(mention).WithEmbed(embed).SendAsync(ctx.Channel);
             //Console.WriteLine($"Mentioned user: {message.MentionedUsers.First().Username}"); 
             
             //await ctx.RespondAsync(message);
        }

        private async Task EmbedKurumiAndTagAkkun()
        {
            
        }

        private async Task ModuleCooldown()
        {
            _moduleAvailable = false;
            _cooldown = Cooldown;
            while (_cooldown > 0)
            {
                await Task.Delay(1000);
                _cooldown--;
                //Console.WriteLine($"Waiting. Time left: {_cooldown.ToString()}s.");
            }

            _moduleAvailable = true;
        }
    }
}