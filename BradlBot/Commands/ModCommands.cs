using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;

namespace BradlBot.Commands
{
    public class ModCommands
    {   
        
        [RequirePermissions(Permissions.KickMembers)]
        [Command("kick")]
        [Description("Kicks a member from the server")]
        public async Task Kick(CommandContext ctx,  string user, string reason)
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
            if (ResponseStorage.ListOfYesNoResponses.Count > 0 && YesNoResponse.LockedGuilds.Contains(ctx.Guild))
            {
                //Guild is locked so give warning
                CommandsCommon.RespondWithError(ctx,"The guild already is waiting for a Y/N, please complete that first or ask a mod to unlock.");
                return;
            }
            else
            {
                //Convert usersToKick to proper list
                var usrList = usersToKick.ToList();
                
                YesNoResponse ynResp = new YesNoResponse(ctx)
                {
                    UserToRespond = ctx.User,
                    DiscordUsersToInteractWithYN = usrList,
                    YnResponseActionYn = YesNoResponse.YNResponseAction.Kick,
                    Content = reason
                };
                
                YesNoResponse.LockedGuilds.Add(ynResp.guild);
            }
            
            
            await ctx.RespondAsync(null, embed: embeddedKick);
        }

        public static async Task KickConfirmedAsync(List<DiscordUser> discordUsers, DiscordGuild guild, string reason, CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string message = String.Empty;
            CommandsCommon.RespondType rtype = CommandsCommon.RespondType.Success;
            try
            {
                //Kick all mentioned users
                foreach (var user in discordUsers)
                {
                    try
                    {
                        DiscordMemberConverter dmc = new DiscordMemberConverter();
                        DiscordMember member;
                        if (dmc.TryConvert(user.Id.ToString(), ctx, out member))
                        {
                            await guild.RemoveMemberAsync(member, reason);
                            message += $"{user.Username}#{user.Discriminator} has been kicked for '{reason  ?? "<unknown>"}'";
                        }
                        else
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                    catch (Exception e)
                    {
                        message += $"{user.Username}#{user.Discriminator} was not able to be kicked for '{reason ?? "<unknown>"}'";
                        rtype = CommandsCommon.RespondType.Warning;
                    }
                }
            }
            finally
            {
                //Cleanup and unlock
                var responseToRemove = ResponseStorage.ListOfYesNoResponses.Find(response => response.guild == guild);
                CommandsCommon.UnlockGuild(guild, new YesNoResponse(ctx));
                CommandsCommon.RespondWithType(rtype,ctx,message);
            }
        }
    }
}