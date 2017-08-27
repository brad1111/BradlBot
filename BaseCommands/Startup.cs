using System;
using System.Runtime.CompilerServices;
using DSharpPlus.CommandsNext;
using BradlBot;

namespace BaseCommands
{
    [Addon]
    public class Startup
    {
        public Startup(CommandsNextModule Commands)
        {
            Commands.RegisterCommands<UserCommands>();
            Commands.RegisterCommands<ModCommands>();
        }
    }
}