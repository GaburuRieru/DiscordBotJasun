using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BlowWaterBot
{
    public class WaterModule : ModuleBase<SocketCommandContext>
    {
        private readonly WaterSource _waterSource;

        public WaterModule(WaterSource waterSource)
        {
            _waterSource = waterSource;
        }
        

        [Command("water")]
        public async Task BlowWater()
        {
           await _waterSource.SayWater(Context.Channel);
        }

        [Command("addwater")]
        //[RequireUserPermission(ChannelPermission.ViewChannel)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddWater(string newWater)
        {
            await _waterSource.AddLineToCsv(newWater, Context.User.Username, Context.Channel);
        }

        [Command("addwater")]
        public async Task AddWaterBadArgument(params string[] arg)
        {
            if (arg.Length == 0)
            {
                await ReplyAsync($"{Context.User.Mention} you need to supply a water quote." +
                                 $"\n " +
                                 $"Remember to wrap them in double quotes \"\".");
                return;
            }

            if (arg.Length > 1)
            {
                await ReplyAsync($"{Context.User.Mention} wrap your quotes in double quotation marks \"\".");
                return;
            }
        }

        [Command("reload")]
        public async Task ReloadWater()
        {
            await _waterSource.ReloadWater();
        }
    }
}