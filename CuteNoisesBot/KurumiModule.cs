using System;
using System.Collections.Generic;
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
        //private int _cooldown;
       // private bool _moduleAvailable = true;

        private Dictionary<ulong, int> _cooldowns = new Dictionary<ulong, int>();

        [Command("kurumi")]
        public async Task KurumiPic(CommandContext ctx)
        {
            if (ctx.Member.IsBot) return;
            
            
            if (!GuildModuleAvailable(ctx.Guild.Id))
            {
                await ctx.RespondAsync($"!kurumi is currently on cooldown. Time remaining: {GetGuildModuleCooldown(ctx.Guild.Id).ToString()}s.");
                return;
            }

            const string kuru = "https://pbs.twimg.com/profile_images/1389334615979032581/TII0GRz9.jpg";
            const string kuruSmile = "https://instagram.fkul8-1.fna.fbcdn.net/v/t51.2885-15/e35/194380710_1025109481226667_3324846550204340503_n.jpg?tp=1&_nc_ht=instagram.fkul8-1.fna.fbcdn.net&_nc_cat=103&_nc_ohc=zYAcc7vcLdMAX_8YZd0&edm=AABBvjUBAAAA&ccb=7-4&oh=c7d3775d0a53035d737d74a8921708de&oe=60D4B45D&_nc_sid=83d603";

            _ = Task.Run(() => StartModuleCooldown(ctx.Guild.Id));
            
            //var akkun = (await ctx.Guild.GetMemberAsync(120843233864581120)); //jasn
           //var akkun = await ctx.Client.GetUserAsync(805956503113564190); //akkun
           DiscordMember akkun;
           
           try
           {
               akkun = await ctx.Guild.GetMemberAsync(805956503113564190);
           }
           catch (Exception e)
           {
               //Console.WriteLine(e);
               akkun = null;
           }

           var embed = new DiscordEmbedBuilder().WithImageUrl(
               new Uri(kuruSmile)).Build();
           
           if (akkun != null)

           {
               //Console.WriteLine($"Send embed with mention");
               var mention = new UserMention(akkun);
               await new DiscordMessageBuilder().WithContent($"{akkun.Mention} xd").WithAllowedMention(mention).WithEmbed(embed).SendAsync(ctx.Channel);
           }

           else
           {
               //Console.WriteLine($"Send embed without mention");
               await new DiscordMessageBuilder().WithEmbed(embed).SendAsync(ctx.Channel);
           }

             //Console.WriteLine($"Mentioned user: {message.MentionedUsers.First().Username}"); 
             
             //await ctx.RespondAsync(message);
        }
        

        private async Task EmbedKurumiAndTagAkkun()
        {
            await Task.Delay(1);
        }

        private async Task StartModuleCooldown(ulong guildId)
        {
            //Console.WriteLine($"StartCooldown");
            _cooldowns.Add(guildId, Cooldown);
            while (GetGuildModuleCooldown(guildId) > 0)
            {
                await Task.Delay(1000);
                GuildCooldownDecrement(guildId);
                //Console.WriteLine($"Waiting. Time left: {_cooldown.ToString()}s.");
            }
            _cooldowns.Remove(guildId);
        }

        private bool GuildModuleAvailable(ulong guildId) => !_cooldowns.ContainsKey(guildId);
        private int GetGuildModuleCooldown(ulong guildId) => _cooldowns[guildId];

        private void GuildCooldownDecrement(ulong guildId)
        {
            var cd = _cooldowns[guildId];
            cd--;
            _cooldowns[guildId] = cd;
        }

    }
}