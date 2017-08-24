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
        
        //Has Control over other users, so may need this tool
        [RequirePermissions(Permissions.ManageMessages)]
        [Command("user")]
        [Description("Gets user information about the person")]
        public async Task UserInfo(CommandContext ctx,
            [Description("Discord member to get information from")] DiscordMember memberToCheck)
        {
            //Show typing
            await ctx.TriggerTypingAsync();

            string title =
                $"{memberToCheck.Mention}: {memberToCheck.Username}#{memberToCheck.Discriminator} aka {memberToCheck.Nickname ?? "<No Nickname>"}";
            
            //Date created
            string accountCreatedDate = $"Created: {memberToCheck.CreationDate.ToUniversalTime().DateTime} UTC";
            string accountJoinedDate = $"Joined: {memberToCheck.JoinedAt.ToUniversalTime().DateTime} UTC";
            
            //Roles setup
            var listOfRoles = memberToCheck.Roles.ToList();
            string rolesTxt = null;
            foreach (var role in listOfRoles)
            {
                rolesTxt += $"    {role.Name}\n";
            }
            
            //Emoji setup
            var checkMark = DiscordEmoji.FromName(ctx.Client ,":white_check_mark:");
            var crossMark = DiscordEmoji.FromName(ctx.Client, ":negative_squared_cross_mark:");
            var questionMark = DiscordEmoji.FromName(ctx.Client, ":grey_question:");

            //Security Settings setup
            string securitySettings = null;
            //    Verified
            securitySettings += "    Verified ";
            switch (memberToCheck.Verified)
            {
                    case true:
                        securitySettings += $"{checkMark}\n";
                        break;
                    case false:
                        securitySettings += $"{crossMark}\n";
                        break;
                    default:
                        securitySettings += $"{questionMark}\n";
                        break;
            }

            //    2fa
            securitySettings += "    2fa ";
            switch (memberToCheck.MfaEnabled)
            {
                    case true:
                        securitySettings += $"{checkMark}\n";
                        break;
                    case false:
                        securitySettings += $"{crossMark}\n";
                        break;
                    default:
                        securitySettings += $"{questionMark}\n";
                        break;
            }

            string botWarning = memberToCheck.IsBot ? "Warning user is a bot.\n" : null;

            
            string message = $"Unique ID: {memberToCheck.Id}\n";
            message += $"{accountCreatedDate}\n";
            message += $"{accountJoinedDate}\n";
//            message += "Security: \n";
//            message += $"{securitySettings}";
            message += $"{botWarning}";
            message += "Avatar:\n";
            
            //Image
            DiscordEmbedImage image = new DiscordEmbedImage()
            {
                Url = memberToCheck.AvatarUrl
            };

            DiscordEmbed embedMessage = new DiscordEmbed()
            {
                Image = image,
                Title = title,
                Description = message,
                Color = 0x0000FF
            };
            await ctx.RespondAsync(null,embed:embedMessage);
        }
    }
}