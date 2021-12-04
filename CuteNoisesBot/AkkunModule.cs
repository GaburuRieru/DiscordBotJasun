using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace CuteNoisesBot
{
    public class AkkunModule : BaseCommandModule
    {

        private static string Greeting =
            "KONMALAM MINNA-SAN, BOKU WA OTAFIUSU SEKUNDO MASKOTTO NO SAIKO, SAISEN, SAIDAI. AKKUN DESUUUUUUUUUUUU";
        
        private static int Cooldown = 60;

        private static ulong AllowedChannel = 719172607789760563;
        
        //private int _cooldown;
       // private bool _moduleAvailable = true;

       // private Dictionary<ulong, int> _cooldowns = new Dictionary<ulong, int>();
        private int _cooldown;

        [Command("akkun")]
        public async Task KonMalam(CommandContext ctx)
        {
            if (ctx.Member.IsBot) return;
            if (ctx.Channel.Id != AllowedChannel) return;
            if (_cooldown > 0) return;
            // if (!GuildModuleAvailable(ctx.Guild.Id))
            // {
            //     await ctx.RespondAsync($"!kurumi is currently on cooldown. Time remaining: {GetGuildModuleCooldown(ctx.Guild.Id).ToString()}s.");
            //     return;
            // }

            _ = Task.Run(StartModuleCooldown);
            
            //var akkun = (await ctx.Guild.GetMemberAsync(120843233864581120)); //jasn
           //var akkun = await ctx.Client.GetUserAsync(805956503113564190); //akkun
           //DiscordMember akkun;
           
           // try
           // {
           //     akkun = await ctx.Guild.GetMemberAsync(805956503113564190);
           // }
           // catch (Exception e)
           // {
           //     //Console.WriteLine(e);
           //     akkun = null;
           // }

           // var embed = new DiscordEmbedBuilder().WithImageUrl(
           //     new Uri(kuruSmile)).Build();
           
           //if (akkun != null)

           //{
               //Console.WriteLine($"Send embed with mention");
              // var mention = new UserMention(akkun);
              // await new DiscordMessageBuilder().WithContent($"{akkun.Mention} xd").WithAllowedMention(mention).WithEmbed(embed).SendAsync(ctx.Channel);
              await new DiscordMessageBuilder().WithContent($"{Greeting}").SendAsync(ctx.Channel);
          // }

          // else
          // {
               //Console.WriteLine($"Send embed without mention");
               //await new DiscordMessageBuilder().WithEmbed(embed).SendAsync(ctx.Channel);
          // }

             //Console.WriteLine($"Mentioned user: {message.MentionedUsers.First().Username}"); 
             
             //await ctx.RespondAsync(message);
        }
        

        private async Task EmbedKurumiAndTagAkkun()
        {
            await Task.Delay(1);
        }

        private async Task StartModuleCooldown()
        {
            //Console.WriteLine($"StartCooldown");
            //_cooldowns.Add(guildId, Cooldown);
            _cooldown = Cooldown;
            while (_cooldown > 0)
            {
                await Task.Delay(1000);
                _cooldown --;
                //Console.WriteLine($"Waiting. Time left: {_cooldown.ToString()}s.");
            }
            //_cooldowns.Remove(guildId);
        }

        // private bool GuildModuleAvailable(ulong guildId) => !_cooldowns.ContainsKey(guildId);
        // private int GetGuildModuleCooldown(ulong guildId) => _cooldowns[guildId];
        //
        // private void GuildCooldownDecrement(ulong guildId)
        // {
        //     var cd = _cooldowns[guildId];
        //     cd--;
        //     _cooldowns[guildId] = cd;
        // }

    }
}