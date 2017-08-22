using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BradlBot.Commands
{
    public class UserCommands
    {

        [Command("ping")]
        [Description("Reply's if the bot is on")]
        [Aliases("on","up","online")]
        public async Task Ping(CommandContext ctx)
        {
            //Show that we are doing stuff
            await ctx.TriggerTypingAsync();

            var wave = DiscordEmoji.FromName(ctx.Client, ":wave:");
            var pong = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            await ctx.RespondAsync($"{wave}{pong}");
        }

        [Command("uptime")]
        [Description("The time that the bot has been online.")]
        [Aliases("onlinefor","upfor","onfor")]
        public async Task Online(CommandContext ctx)
        {
            //show that stuff's happening
            await ctx.TriggerTypingAsync();

            var clock = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");
            
            //Get date started
            var timeSinceStart = DateTime.UtcNow.Subtract(Program.TimeStarted);

            await ctx.RespondAsync(
                $"{clock} The bot has been up for {timeSinceStart.Days} Day[s], {timeSinceStart.Hours % 24} Hour[s], {timeSinceStart.Minutes % (24 * 60)} Minute[s], {timeSinceStart.Seconds % (24 * 60 * 60)} Second[s]");
        }
    }
}