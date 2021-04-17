using System;
using System.Threading.Tasks;
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
            
        }

        [Command("addwater")]
        public async Task AddWater(string newWater)
        {
            
        }

        [Command("reload")]
        public async Task ReloadWater()
        {
            
        }
    }
}