using System;
using System.Collections.Generic;
using DSharpPlus;

namespace BradlBot.Commands
{
    /// <summary>
    /// This class is for storage of variables in things like Y/N or give a value etc.
    /// </summary>
    public class ResponseStorage
    {
        #region YesNoResponse

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
        
            public static ResponseAction? ResponseActionYN
            {
                get => YNCommandInProgress ? _responseActionYN : null;
                set
                {
                    if (!YNCommandInProgress)
                        _responseActionYN = value;
                }
            }

            private static ResponseAction? _responseActionYN;
        
            //Locks changing the items
            public static bool YNCommandInProgress
            {
                //Set default value as false (default value is unlocked)
                get => _yNCommandInProgress ?? false;
                set => _yNCommandInProgress = value;
            }

            private static bool? _yNCommandInProgress; 
        
            public enum ResponseAction
            {
                Kick,
                Ban,
                Warn
            }
        
            
        #endregion
        
        
        
    }
}