using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BradlBot
{
    public class UserCommands
    {

        [Command("ping")]
        [Description("Reply's if the bot is on")]
        public async Task Ping(CommandContext ctx)
        {
            //Show that we are doing stuff
            await ctx.TriggerTypingAsync();

            var wave = DiscordEmoji.FromName(ctx.Client, ":wave:");
            var pong = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            await ctx.RespondAsync($"{wave}{pong}");
        }
    }
}