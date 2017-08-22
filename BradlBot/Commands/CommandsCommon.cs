using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace BradlBot.Commands
{
    public class CommandsCommon
    {
        public static void RespondWithError(CommandContext ctx, string Error)
        {
            DiscordEmoji stopEmoji = DiscordEmoji.FromName(ctx.Client,":no_entry:");
            
            DiscordEmbed embed = new DiscordEmbed()
            {
                Title = "Error",
                Description = $"{stopEmoji}{Error}",
                Color = 0xFF0000
            };
            ctx.RespondAsync(null,embed:embed);
        }
    }
}