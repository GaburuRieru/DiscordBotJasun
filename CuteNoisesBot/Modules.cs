using System;
using System.Threading.Channels;
using System.Threading.Tasks;
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