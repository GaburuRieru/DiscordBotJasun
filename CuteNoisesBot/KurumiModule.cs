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
        [Command("kurumi")]
        public async Task KurumiPic(CommandContext ctx)
        {
            const string kuru = "https://pbs.twimg.com/profile_images/1389334615979032581/TII0GRz9.jpg";

            //var akkun = (await ctx.Guild.GetMemberAsync(120843233864581120));
            var akkun = await ctx.Client.GetUserAsync(805956503113564190);

            if (akkun == null) return;
            
            var mention = new UserMention(akkun);
            
             var embed = new DiscordEmbedBuilder().WithImageUrl(
                 new Uri(kuru)).Build();

             var message = await new DiscordMessageBuilder().WithContent($"{akkun.Mention} xd").WithAllowedMention(mention).WithEmbed(embed).SendAsync(ctx.Channel);
             //Console.WriteLine($"Mentioned user: {message.MentionedUsers.First().Username}"); 
             
             //await ctx.RespondAsync(message);
        }
    }
}