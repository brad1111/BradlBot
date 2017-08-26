using System;
using System.Collections.Generic;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace BradlBot.Commands
{
    public class CommandsCommon
    {
        public static DateTime TimeStarted { get; set; }
        public static Assembly FrontEndAssembly { get; set; } 
        public static List<AssemblyName> ReferencedAssemblies { get; set; }
        
        public enum RespondType
        {
            Error,
            Warning,
            Success
        }

        public static void RespondWithType(RespondType type, CommandContext ctx, string Message)
        {
            switch (type)
            {
                case RespondType.Error:
                    RespondWithError(ctx, Message);
                    break;
                case RespondType.Warning:
                    RespondWithWarning(ctx, Message);
                    break;
                case RespondType.Success:
                    RespondWithSuccess(ctx,Message);
                    break;
            }
        }
        
        public static void RespondWithError(CommandContext ctx, string Error)
        {
            DiscordEmoji stopEmoji = DiscordEmoji.FromName(ctx.Client,":no_entry:");
            Respond(ctx,"Error",$"{stopEmoji}{Error}",0xFF0000);
        }

        public static void RespondWithWarning(CommandContext ctx, string warningMessage, string customTitle = null)
        {
            DiscordEmoji warningEmoji = DiscordEmoji.FromName(ctx.Client,":warning:");
            Respond(ctx, customTitle ?? "Warning", $"{warningEmoji}{warningMessage}", 0xFFF000);
        }

        public static void RespondWithSuccess(CommandContext ctx, string successMessage)
        {
            DiscordEmoji successEmoji = DiscordEmoji.FromName(ctx.Client,":white_check_mark:");
            Respond(ctx, "Success", $"{successEmoji}{successMessage}",0x00FF00);
        }
        
        public static void Respond(CommandContext ctx, string title, string message, int color)
        {
            DiscordEmbed embed = new DiscordEmbed()
            {
                Title = title,
                Description = message,
                Color = color
            };
            ctx.RespondAsync(null, embed: embed);
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