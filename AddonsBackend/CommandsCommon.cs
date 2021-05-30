using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace BradlBot.Commands
{
    public class CommandsCommon
    {
        public static DateTime TimeStarted { get; set; }
        public static Assembly FrontEndAssembly { get; set; } 
        public static List<AssemblyName> ReferencedAssemblies { get; set; }
        public static List<Assembly> AddonAssemblies { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        
        public enum RespondType
        {
            Error,
            Warning,
            Success
        }

        public static async Task RespondWithType(RespondType type, CommandContext ctx, string message)
        {
            switch (type)
            {
                case RespondType.Error:
                    await RespondWithError(ctx, message);
                    break;
                case RespondType.Warning:
                    await RespondWithWarning(ctx, message);
                    break;
                case RespondType.Success:
                    await RespondWithSuccess(ctx, message);
                    break;
            }
        }
        
        public static async Task RespondWithError(CommandContext ctx, string Error)
        {
            DiscordEmoji stopEmoji = DiscordEmoji.FromName(ctx.Client,":no_entry:");
            await Respond(ctx, "Error", $"{stopEmoji}{Error}", new DiscordColor(0xFF0000));
        }

        public static async Task RespondWithWarning(CommandContext ctx, string warningMessage, string customTitle = null)
        {
            DiscordEmoji warningEmoji = DiscordEmoji.FromName(ctx.Client,":warning:");
            await Respond(ctx, customTitle ?? "Warning", $"{warningEmoji}{warningMessage}", new DiscordColor(0xFFF000));
        }

        public static async Task RespondWithSuccess(CommandContext ctx, string successMessage)
        {
            DiscordEmoji successEmoji = DiscordEmoji.FromName(ctx.Client,":white_check_mark:");
            await Respond(ctx, "Success", $"{successEmoji}{successMessage}", new DiscordColor(0x00FF00));
        }
        
        public static async Task Respond(CommandContext ctx, string title, string message, DiscordColor color)
        {
            DiscordEmbed embed = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = message,
                Color = color
            };
            await ctx.RespondAsync(null, embed: embed);
        }


        /*public static void UnlockGuild(DiscordGuild guild, object TypeOfResponse)
        {
            if (TypeOfResponse is YesNoResponse)
            {
                //Get correct YN response
                var YNResp = ResponseStorage.ListOfYesNoResponses.Find(response => response.guild == guild);
                
                //Cleanup YN response
                ResponseStorage.ListOfYesNoResponses.Remove(YNResp);
                YesNoResponse.LockedGuilds.Remove(guild);
            }
        }*/
    }
}