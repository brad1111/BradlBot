﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
 using System.Runtime.InteropServices.ComTypes;
 using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
 using AddonsBackend;
 using BradlBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
 using DSharpPlus.Entities;
 using DSharpPlus.EventArgs;
 using DSharpPlus.Interactivity;
using Newtonsoft.Json;


namespace BradlBot
{
    class Program
    {
        public DiscordClient Client { get; set; }

        public CommandsNextModule Commands
        {
            get => CommandsCommon.Commands;
            set => CommandsCommon.Commands = value;
        }

        static InteractivityModule Interactivity { get; set; }

        private static string AddonDirectory
        {
            get
            {
                string directory = Directory.GetCurrentDirectory();
                return (directory + "/addons");
            }
        }

        private static List<Message> _storedMessages = new List<Message>();

        public static void Main(string[] args)
        {
            //Pre-setup for AppVeyor/Travis CI
            if (args.Contains("-AddonsFolder".ToLower().Trim()))
            {
                SetupAddonsFolder().GetAwaiter().GetResult();
                Console.WriteLine("Addons folder setup successfully.");
                return;
            }

            Console.WriteLine("BradlBot Starting");
            
            //Setup config 
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("What do you want your command prefix to be?");
                string prefix = Console.ReadLine();
                Console.WriteLine("What do you want your token to be?");
                string token = Console.ReadLine();
                ConfigJson cfgj = new ConfigJson()
                {
                    CommandPrefix = prefix,
                    Token = token
                };

                string cfgstring = JsonConvert.SerializeObject(cfgj);
                using (var fs = File.Create("config.json"))
                    using (var sw = new StreamWriter(fs, new UTF8Encoding()))
                        sw.Write(cfgstring);
            }
            
            var prog = new Program();
            prog.CheckConfigAsync().GetAwaiter().GetResult();
        }

        
        public async Task CheckConfigAsync()
        {
            //Check config exists
            var json = "";
            try
            {
                //Actually start
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding()))
                    json = await sr.ReadToEndAsync();

                //Next load values from file
                var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
                var cfg = new DiscordConfiguration()
                {
                    Token = cfgjson.Token,
                    TokenType = TokenType.Bot,

                    AutoReconnect = true,
                    LogLevel = LogLevel.Debug,
                    UseInternalLogHandler = true,
                };
                
                await SetupBotAsync(cfg, cfgjson);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with config or runtime error: {e.GetType()} - {e.Message}");
#if DEBUG
                throw;
#endif
            }
            finally
            {
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }

        }
        
        
        public async Task SetupBotAsync(DiscordConfiguration cfg, ConfigJson cfgjson)
        {

            //Create instance of client
            this.Client = new DiscordClient(cfg);
            
            //Create events
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientErrored += this.Client_ClientError;
            this.Client.MessageCreated += async args =>
            {
                _storedMessages.Add(Message.GetFromDiscordMessage(args.Message));
            };
            this.Client.MessageDeleted += async args =>
            {
                //Checks to see if list has anything in it before checking
                if (_storedMessages.Count == 0)
                {
                    Console.WriteLine(
                        "Could not save a message due nothing being stored right now, please do not turn off the bot often.");
                    return;
                }

                //Checks to see if ID is stored anywhere
                if (_storedMessages.All(message => message.ID != args.Message.Id))
                {
                    Console.WriteLine("Could not save a message due to it not being stored.");
                    return;
                }
                
                if(_storedMessages.Any(message =>  message.ID == args.Message.Id && message.Channel.Name.ToLower().Trim() == "logs"))
                {
                    //Simply return as we don't log log deletion
                    return;
                }
                //Find last in case the message was changed
                var correctStoreMessage = _storedMessages.FindLast(message => message.ID == args.Message.Id);

                //Embedded message
                DiscordEmoji warningEmoji = DiscordEmoji.FromName(this.Client, ":warning:");
                DiscordEmbedBuilder.EmbedFooter footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"Message was deleted at {DateTime.UtcNow} UTC"
                };
                DiscordEmbedBuilder.EmbedAuthor author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = correctStoreMessage.Author.ToString()
                };
                DiscordEmbed embeddedMessage = new DiscordEmbedBuilder()
                {
                    Title = $"{warningEmoji}Warning",
                    Description = $"{correctStoreMessage.Content}",
                    Footer = footer,
                    Author = author,
                    Color = new DiscordColor(0xFFFF00)
                };

                //Logs channel
                
                try
                {
                    var logsChannel =
                        args.Guild.Channels.First(channels => channels.Name.ToLower().Trim() == "logs");
                    await logsChannel.SendMessageAsync(null, embed: embeddedMessage);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Couldn't write to logs channel");
                }
            };
            
            this.Client.MessageUpdated += async args =>
            {
                //Check to see if list has anything in it before checking
                if (_storedMessages.Count == 0)
                {
                    Console.WriteLine("Could not save a changed message as no messages have been saved.");
                    return;
                }
                //Check to see if the ID is already there to show update
                if (_storedMessages.All(message => message.ID != args.Message.Id))
                {
                    Console.WriteLine("Could not save a changed message as it was not saved.");
                    return;
                }
                //Check to see if the log was modified (don't log the logs)
                if(args.Channel.Name.ToLower().Trim() == "logs")
                    return;
                
                //Find correct message (has to be last in case it's been edited before)
                var correctStoredMessage = _storedMessages.FindLast(message => message.ID == args.Message.Id);
                
                
                //Setup embed
                DiscordEmoji warningEmoji = DiscordEmoji.FromName(this.Client, ":warning:");
                DiscordEmbedBuilder.EmbedFooter footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"Message was changed at {DateTime.UtcNow} UTC"
                };
                DiscordEmbed embeddedMessage = new DiscordEmbedBuilder()
                {
                    Title = $"{warningEmoji}Warning",
                    Description = $"{correctStoredMessage.Content}\n" +
                                  "Was changed to:\n" +
                                  $"{args.Message.Content}",
                    Footer = footer,
                    Author = new DiscordEmbedBuilder.EmbedAuthor(){ Name = args.Author.ToString()},
                    Color = new DiscordColor(0xFFFF00)
                };
                
                //Find logs channel and send to logs channel
                try
                {
                    var logsChannel = args.Guild.Channels.First(channel => channel.Name.ToLower().Trim() == "logs");
                    await logsChannel.SendMessageAsync(null, embed: embeddedMessage);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Could not find logs channel");
                }
            };
            //Comands config
            var ccfg = new DSharpPlus.CommandsNext.CommandsNextConfiguration()
            {
                StringPrefix = cfgjson.CommandPrefix,
                EnableDms = true,
                EnableMentionPrefix = true
            };

            this.Commands = this.Client.UseCommandsNext(ccfg);
                
            //Commands Events
            this.Commands.CommandExecuted += this.Command_CommandExecuted;
            this.Commands.CommandErrored += this.Command_CommandError;

            //Get components
            if (Directory.Exists(AddonDirectory))
            {
                //Check for addons
                string[] files = Directory.GetFiles(AddonDirectory);
                var addonAssemblies = new List<Assembly>();

                foreach (var file in files)
                {
                    //Checks to see if they are dlls
                    if (file.EndsWith(".dll".ToLower()))
                    {
                        try
                        {
                            //get assemblyname of dll
                            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                            var typesWithin = asm.GetTypes();

                            var startupTypes = typesWithin.Where(type =>
                                type.GetTypeInfo().GetInterfaces().Contains(typeof(IStartup)));

                            
                            foreach (var startupType in startupTypes)
                            {
                                object startupTypeObject =  Activator.CreateInstance(startupType);
                                IStartup startupTypeInterf = startupTypeObject as IStartup;
                                startupTypeInterf?.StartupLogic();
                                Console.WriteLine($"Dynamically Loaded: {startupType.FullName}");
                                addonAssemblies.Add(startupType.Assembly);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error could not load: {file}\n" +
                                              $"Error: {e.GetType()} - {e.Message}");
                        }
                    }
                }

                CommandsCommon.AddonAssemblies = addonAssemblies;
            }
            else
            {
                await SetupAddonsFolder();
            }
            
            //Commands registration
            this.Commands.RegisterCommands<OwnerCommands>();
            
            //Interactivity Setup
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration());
            
            //Connect and login
            await this.Client.ConnectAsync();
            
            //Save time started
            CommandsCommon.TimeStarted = DateTime.UtcNow;
            
            //Save the assemblys
            CommandsCommon.FrontEndAssembly = Assembly.GetEntryAssembly();
            CommandsCommon.ReferencedAssemblies = CommandsCommon.FrontEndAssembly.GetReferencedAssemblies().ToList();
            
            //Prevent premature quitting
            await Task.Delay(-1);
        }

        private static async Task SetupAddonsFolder()
        {
//Create folder
            Directory.CreateDirectory(AddonDirectory);
            string readmePath = AddonDirectory + "/addons.txt";
            string developersPath = AddonDirectory + "/developers.txt";
            Assembly thisAssembly = Assembly.GetEntryAssembly();

            string header = "NOTE THIS IS AN AUTOMATICALLY GENERATED DOCUMENT.\n" +
                            $"This was created by {thisAssembly.GetName().Name} - Version {thisAssembly.GetName().Version}\n" +
                            $"Github: https://www.github.com/thebradad1111/bradlbot\n" +
                            "\n";
            string readmeContents = header +
                                    "The addons within this folder will be automatically detected.\n" +
                                    "Addons should end with '.dll' or be of type 'Application Extension' in Windows\n" +
                                    "Addon authors are solely responsible for any damage caused by their addons and are solely responsible for support.\n" +
                                    "DLL's have the capability of being malicious so only download addons from people you trust.\n" +
                                    "Addons that do not work are not the responsibility of the author of BradlBot, please contact the addon author if possible";

            string developerContents = header +
                                       "Developers: you are solely responsible for any damaged caused by your addon and are solely responsible for support and maintainence.\n" +
                                       "To register your DLL with BradlBot, you will need to implement the interface 'IStartup' to a startup class (you can call this class what you want\n" +
                                       "this requires an AddonsBackend reference in the project, this must not contain your commands or a constructor).\n" +
                                       "In the void required by the interface make sure you write 'CommandsCommon.Commands.RegisterCommands<NameOfClassWithYourCommandsIn>();' to add these commands.\n" +
                                       "NOTE: Your class with your commands in must also not have a constructor otherwise it will crash on startup.\n" +
                                       "Then make sure you place the dll file from /bin/debug/{yourproject.dll} or /bin/release/{yourproject.dll} into this addons folder after compiling.\n" +
                                       "See https://www.github.com/thebradad1111/bradlbot for more details and examples.";

            var readmeStreamWriter = File.CreateText(readmePath);
            await readmeStreamWriter.WriteAsync(readmeContents);
            await readmeStreamWriter.FlushAsync();
            var developerStreamWriter = File.CreateText(developersPath);
            await developerStreamWriter.WriteAsync(developerContents);
            await developerStreamWriter.FlushAsync();
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            //lets log that it's occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BradlBot","Client is ready to process events", DateTime.Now);
            //return a completed task
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            //Lets log name of guild that was sent
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BradlBot",$"Guild available: {e.Guild.Name}",DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            //Log guild that was sent
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "BradlBot",$"Exception: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Command_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "BradlBot",
                $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}' on {e.Context.Guild.Name} - {e.Context.Channel.Name}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Command_CommandError(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "BradlBot",$"{e.Context.User.Username} tried to run '{e.Command?.QualifiedName ?? "<invalid_cmd>"}' on {e.Context.Guild.Name} - {e.Context.Channel.Name} but it gave the error: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);   
                    
            //Check to see if lack of permisions
            if (e.Exception is ChecksFailedException)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                CommandsCommon.Respond(e.Context, "Access Denied", $"{emoji} You do not have permission to execute this.", new DiscordColor(0xFF0000));
            }
            else if(e.Exception is ArgumentException)
            {
                //Arguments are wrong so tell them so
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":face_palm:");
                CommandsCommon.Respond(e.Context,"Error",$"{emoji}Incorrect arguments; see '!help {e.Command?.Name ?? "<Command Name>"}'", new DiscordColor(0xFF0000));
            }
            else
            {
                CommandsCommon.RespondWithError(e.Context, $"{e.Exception.GetType()} - {e.Exception.Message}");
            }
        }
    }
    
    //Holds config.json
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }
    }
}