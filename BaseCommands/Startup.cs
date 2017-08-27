using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using AddonsBackend;
using DSharpPlus.CommandsNext;
using BradlBot;
using BradlBot.Commands;

namespace BaseCommands
{
    public class Startup : IStartup
    {
        public void StartupLogic()
        {
            CommandsCommon.Commands.RegisterCommands<UserCommands>();
            CommandsCommon.Commands.RegisterCommands<ModCommands>();
        }
    }
}