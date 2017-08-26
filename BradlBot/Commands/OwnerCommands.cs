using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BradlBot.Commands
{
    public class OwnerCommands
    {
        [RequireOwner]
        [Command("shutdown")]
        [Description("Turns the bot off")]
        public async Task Shutdown(CommandContext ctx,
            [Description("Turns the program off completely if y")] string hardShutdown = "n")
        {
            await ctx.TriggerTypingAsync();
            switch (hardShutdown.ToLower().Trim())
            {
                    case "hard":
                    case "force":
                    case "y":
                        CommandsCommon.RespondWithSuccess(ctx,"Shutting down forcefully");
                        await Task.Delay(100);
                        Environment.Exit(0);
                        break;
                    case "n":
                        CommandsCommon.RespondWithSuccess(ctx,
                            $"Shutting down.\n {ctx.User.Mention} use backend console to turn back on.");
                        await ctx.Client.DisconnectAsync();
                        Console.ReadKey();
                        Program.Main(null);
                        break;
                    default:
                        throw new ArgumentException($"Only y or n accepted after {ctx.Command.Name}");
            }
        }

        [RequireOwner]
        [Command("restart")]
        [Aliases("reboot")]
        [Description("Restarts the bot")]
        public async Task Restart(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            CommandsCommon.RespondWithWarning(ctx,"Restarting");
            await ctx.Client.DisconnectAsync();
            await Task.Delay(1000);
            Program.Main(null);
        }
    }
}