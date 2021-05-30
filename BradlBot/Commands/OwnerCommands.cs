using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BradlBot.Commands
{
    public class OwnerCommands : BaseCommandModule
    {
        [RequireOwner]
        [Command("shutdown")]
        [Description("Turns the bot off")]
        public async Task Shutdown(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await CommandsCommon.RespondWithSuccess(ctx,"Shutting down");
            await Task.Delay(100);
            Environment.Exit(0);
        }

        [RequireOwner]
        [Command("restart")]
        [Aliases("reboot")]
        [Description("Restarts the bot")]
        public async Task Restart(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await CommandsCommon.RespondWithWarning(ctx,"Restarting");
            await ctx.Client.DisconnectAsync();
            await Task.Delay(1000);
            Program.Main(new string[0]);
        }
    }
}