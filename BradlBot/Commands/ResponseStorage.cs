using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace BradlBot.Commands
{
    /// <summary>
    /// This class is for storage of variables in things like Y/N or give a value etc.
    /// </summary>
    public class ResponseStorage
    {
        public static List<string> ValidResponses = new List<string>()
        {
            "Y","N"
        };
        
        public static List<YesNoResponse> ListOfYesNoResponses = new List<YesNoResponse>();
        
        #region YesNoResponse
/*            public static DiscordGuild guild { get; set; }
            public static List<string> YNResponses = new List<string>(){"Y","N"};
        
        
            public static List<DiscordUser> DiscordUsersToInteractWithYN
            {
                get => YNCommandInProgress ? _discordUsersToInteractWithYN : null;
                set
                {
                    if (!YNCommandInProgress)
                        _discordUsersToInteractWithYN = value;
                }
            }
            private static List<DiscordUser> _discordUsersToInteractWithYN;
            

            public static DiscordUser UserToRespond
            {
                get => YNCommandInProgress ? _userToRespond : null;
                set
                {
                    if (!YNCommandInProgress)
                        _userToRespond = value;
                }
            }
    
            private static DiscordUser _userToRespond;
        
            public static YNResponseAction? YnResponseActionYn
            {
                get => YNCommandInProgress ? _ynResponseActionYn : null;
                set
                {
                    if (!YNCommandInProgress)
                        _ynResponseActionYn = value;
                }
            }

            private static YNResponseAction? _ynResponseActionYn;
        
            //Locks changing the items
            public static bool YNCommandInProgress
            {
                //Set default value as false (default value is unlocked)
                get => _yNCommandInProgress ?? false;
                set => _yNCommandInProgress = value;
            }

            private static bool? _yNCommandInProgress; 
        
            public enum YNResponseAction
            {
                Kick,
                Ban,
                Warn
            }

            public static void YNRespondToAction()
            {
                switch (YnResponseActionYn)
                {
                        case YNResponseAction.Kick:
                            ModCommands.KickConfirmedAsync(DiscordUsersToInteractWithYN);
                }                
            }
        
            */
        #endregion
        
        
        
    }

    public class YesNoResponse
    {
        public YesNoResponse(CommandContext ctx)
        {
            context = ctx;
            ResponseStorage.ListOfYesNoResponses.Add(this);
        }
        
        public CommandContext context { get; private set; }
        public DiscordGuild guild => context.Guild;
        
        public static List<DiscordGuild> LockedGuilds = new List<DiscordGuild>();
        
        public static List<string> YNResponses = new List<string>(){"Y","N"};
        
        
        public List<DiscordUser> DiscordUsersToInteractWithYN
        {
            get => _discordUsersToInteractWithYN;
            set => _discordUsersToInteractWithYN = value;
        }
        private List<DiscordUser> _discordUsersToInteractWithYN;
        

        public DiscordUser UserToRespond
        {
            get => _userToRespond;
            set => _userToRespond = value;
        }

        private DiscordUser _userToRespond;
    
        public  YNResponseAction? YnResponseActionYn
        {
            get => _ynResponseActionYn;
            set => _ynResponseActionYn = value;
        }

        private YNResponseAction? _ynResponseActionYn; 
    
        public enum YNResponseAction
        {
            Kick,
            Ban,
            Warn
        }

        public async Task YNRespondToAction()
        {
            switch (YnResponseActionYn)
            {
                    case YNResponseAction.Kick:
                        await ModCommands.KickConfirmedAsync(DiscordUsersToInteractWithYN, guild, Content,context);
                        break;
            }                
        }
        
        public string Content { get; set; }
    }
}