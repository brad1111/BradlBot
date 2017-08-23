using System.Linq;
using System.Threading.Tasks;
using BradlBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace BradlBot
{
    public class Checks
    {
        public static async Task MessageCreated_Checks(MessageCreateEventArgs e)
        {
            //Check to see if message is a response
            if (ResponseStorage.ValidResponses.Contains(e.Message.Content.ToUpper().Trim()))
            {
                //Check to see if it is a Y/N response (currently yes 100% of time) and is accepting a Y/N answer
                if (YesNoResponse.YNResponses.Contains(e.Message.Content.ToUpper().Trim()) && YesNoResponse.LockedGuilds.Contains(e.Guild))
                {
                    //Get specific locked guild item
                    var lockedResponse = ResponseStorage.ListOfYesNoResponses.Find(response => response.guild == e.Guild);
                    
                    //Check to see user can respond to lock
                    if (lockedResponse.UserToRespond == e.Author)
                    {
                        switch (e.Message.Content.ToLower().Trim())
                        {
                            case "y":
                                await lockedResponse.YNRespondToAction();
                                break;
                            case "n":
                                await Output_Cancelled(lockedResponse.context, "kick");
                                CommandsCommon.UnlockGuild(lockedResponse.guild, new YesNoResponse(lockedResponse.context));
                                break;
                        }
                    }
                }
            }
        }

        private static async Task Output_Cancelled(CommandContext ctx,string nameOfCommand = null)
        {
            var x = DiscordEmoji.FromName(ctx.Client,":x:");
            DiscordEmbed embedCancelled = new DiscordEmbed()
            {
                Title = "Cancelled",
                Description = $"{x}The '{nameOfCommand ?? "<unspecified>"}' command was cancelled",
                Color = 0xFF7F00
            };
            await ctx.RespondAsync(null, embed: embedCancelled);
        }
    }
}