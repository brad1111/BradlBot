﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
 using DSharpPlus.Interactivity;

namespace BradlBot.Commands
{
    public class ModCommands
    {   
        
        [RequirePermissions(Permissions.KickMembers)]
        [Command("kick")]
        [Description("Kicks a member from the server")]
        public async Task Kick(CommandContext ctx,[Description("The @member to be kicked")] DiscordMember memberToKick, [Description("The reason that they were kicked")] string reason = null)
        {
            //show stuff's happening
            await ctx.TriggerTypingAsync();

            var boot = DiscordEmoji.FromName(ctx.Client, ":boot:");
            DiscordEmbed embeddedKick = new DiscordEmbed()
            {
                Title = "Kick?",
                Description = $"{boot}Kick this user: {memberToKick.Mention}? [Y/N]"
            };
            
            
            await ctx.RespondAsync(null, embed: embeddedKick);

            var interactivity = ctx.Client.GetInteractivityModule();
            //Wait for message from user that is either Y or N
            var msg = await interactivity.WaitForMessageAsync(
                umsg => umsg.Author.Id == ctx.User.Id &&
                        (umsg.Content.ToLower().Trim() == "y" || umsg.Content.ToLower() == "n"),
                TimeSpan.FromMinutes(1));
            if (msg == null)
            {
                CommandsCommon.RespondWithError(ctx,$"Kick by {ctx.User.Mention} for {memberToKick.Mention} was cancelled due to timeout.");
            }
            else if(msg.Content.ToLower().Trim() == "y")
            {
                await ctx.Guild.RemoveMemberAsync(memberToKick);
                CommandsCommon.RespondWithSuccess(ctx,
                    $"{memberToKick.Username}#{memberToKick.Discriminator} was kicked for reason '{reason ?? "<unknown>"}'");
            }
            else
            {
                CommandsCommon.RespondWithWarning(ctx,$"{ctx.User.Mention} did not kick {memberToKick.Mention}",customTitle:"Phew");
            }
        }
    }
}