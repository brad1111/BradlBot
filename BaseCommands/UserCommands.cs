using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using AddonsBackend;
using BradlBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace BaseCommands
{
    public class UserCommands : BaseCommandModule
    {

        private static readonly TimeSpan second = TimeSpan.FromSeconds(1);

        [Command("ping")]
        [Description("Reply's if the bot is on")]
        [Aliases("on", "up", "online")]
        public async Task Ping(CommandContext ctx)
        {
            var startTime = ctx.Message.CreationTimestamp;

            //Show that we are doing stuff
            await ctx.TriggerTypingAsync();
            

            var wave = DiscordEmoji.FromName(ctx.Client, ":wave:");
            var pong = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            var currentTime = DateTimeOffset.Now;

            var ping = currentTime - startTime;

            string output = ping < second //if ping is smaller than a second output in ms, else do it normally
                ? ping.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + "ms"
                : ping.ToString();
            

            await ctx.RespondAsync($"{wave}{pong}: {output}");
        }

        [Command("uptime")]
        [Description("The time that the bot has been online.")]
        [Aliases("onlinefor", "upfor", "onfor")]
        public async Task Online(CommandContext ctx)
        {
            //show that stuff's happening
            await ctx.TriggerTypingAsync();

            var clock = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");

            //Get date started
            var timeSinceStart = DateTime.UtcNow.Subtract(CommandsCommon.TimeStarted);

            await ctx.RespondAsync(
                $"{clock} The bot has been up for {timeSinceStart.Days} Day[s], {timeSinceStart.Hours % 24} Hour[s], {timeSinceStart.Minutes % (24 * 60)} Minute[s], {timeSinceStart.Seconds % (24 * 60 * 60)} Second[s]");
        }

        [Command("info")]
        [Description("Information about the bot")]
        [Aliases("botinfo")]
        public async Task Info(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            Assembly thisAssembly = CommandsCommon.FrontEndAssembly;
            string title = $"{thisAssembly.GetName().Name} - Version {thisAssembly.GetName().Version}";
            string output = String.Empty;
            
            //find dependancies and add to string
            output += "•Dependencies:\n";
            foreach (var assemblyReference in CommandsCommon.ReferencedAssemblies)
            {
                output += $"    ‣{assemblyReference.Name} - Version {assemblyReference.Version}\n";
            }

            output += "•Addons:\n";
            foreach (var addons in CommandsCommon.AddonAssemblies)
            {
                output += $"    ‣{addons.GetName().Name} - Version {addons.GetName().Version}\n";
            }

            //Tell them that it's running off a DotNetCoreApp 1.1
            var targetFw = thisAssembly.GetCustomAttributes<TargetFrameworkAttribute>();
            output += $"•Targets: {targetFw.ToList()[0].FrameworkName}\n";
            
            //Get os
            output += $"•OS: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}\n";
            
            //output github
            output += "•Github Repo: https://www.github.com/thebradad1111/bradlbot";
            await CommandsCommon.Respond(ctx,title,output, new DiscordColor(0x00FF00));
        }
    
    }
}