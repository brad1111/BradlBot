using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BradlBot.Commands
{
    public class ModCommands
    {   
        
        [RequirePermissions(Permissions.KickMembers)]
        [Command("kick")]
        [Description("Kicks a member from the server")]
        public async Task Kick(CommandContext ctx)
        {
            //show stuff's happening
            await ctx.TriggerTypingAsync();

            //List of users to kick
            var usersToKick = ctx.Message.MentionedUsers;

            //if mentioned users = none then give error
            if (usersToKick.Count == 0)
            {
                CommandsCommon.RespondWithError(ctx,"You cannot kick 0 people, please @mention the person you want to kick.");   
                //Return early
                return;
            }


            var boot = DiscordEmoji.FromName(ctx.Client, ":boot:");

            string userToKickTxt = String.Empty;

            for (int i = 0; i < usersToKick.Count; i++)
            {
                //If value is not final nor penultimate, then show ', ' if penultimate show ' and ' else nothing
                string suffix = i < usersToKick.Count - 2
                    ? ",\n"
                    : i == usersToKick.Count - 1
                        ? String.Empty
                        : " and\n";

                userToKickTxt += usersToKick[i].Username + " #" + usersToKick[i].Discriminator + suffix;
            }

            DiscordEmbed embeddedKick = new DiscordEmbed()
            {
                Title = "Kick?",
                Description = $"{boot}Kick these users:\n{userToKickTxt}?\n[Y/N]"
            };

            //Setup Storage
            if (ResponseStorage.YNCommandInProgress)
            {
                CommandsCommon.RespondWithError(ctx,"Please answer the previous Y/N answer first.");
                return;
            }
            else
            {
                ResponseStorage.ResponseActionYN = ResponseStorage.ResponseAction.Kick;
                ResponseStorage.DiscordUsersToInteractWithYN = usersToKick.ToList();
                ResponseStorage.UserToRespond = ctx.User;
                ResponseStorage.YNCommandInProgress = true;
            }
            
            
            await ctx.RespondAsync(null, embed: embeddedKick);
        }
    }
}