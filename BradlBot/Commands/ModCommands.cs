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
        public async Task Kick(CommandContext ctx,[Description("The @member to be kicked")] DiscordMember memberToKick,
            [Description("The reason that they were kicked")] string reason = null)
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

        [RequirePermissions(Permissions.BanMembers)]
        [Command("ban")]
        [Description("Bans a member from the server")]
        public async Task Ban(CommandContext ctx,
            [Description("Discord member to ban")] DiscordMember discordMemberToBan, string reason = null)
        {
            //Show typing
            await ctx.TriggerTypingAsync();

            var banHammer = DiscordEmoji.FromName(ctx.Client,":hammer:");
            CommandsCommon.Respond(ctx,"Ban?",$"{banHammer}Ban this user: {discordMemberToBan.Mention}? [Y/N]", 0);

            var interactivity = ctx.Client.GetInteractivityModule();
            var confirmMessage = await interactivity.WaitForMessageAsync(msg =>
                msg.Author == ctx.Member &&
                (msg.Content.ToLower().Trim() == "y" || msg.Content.ToLower().Trim() == "n"), TimeSpan.FromMinutes(1));
            switch (confirmMessage?.Content.ToLower().Trim())
            {
                    case null:
                        CommandsCommon.RespondWithError(ctx,$"Ban by {ctx.User.Mention} for {discordMemberToBan.Mention} was cancelled due to timeout.");
                        break;
                    case "n":
                        CommandsCommon.RespondWithWarning(ctx,$"{ctx.User.Mention} did not ban {discordMemberToBan.Mention}",customTitle:"Phew");
                        break;
                    case "y":
                        await ctx.Guild.BanMemberAsync(discordMemberToBan);
                        break;
            }
        }
    }
}