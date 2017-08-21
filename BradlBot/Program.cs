using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;


namespace BradlBot
{
    class Program
    {
        public DiscordClient Client { get; set; }
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
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
                var cfg = new DiscordConfig()
                {
                    Token = cfgjson.Token,
                    TokenType = TokenType.Bot,

                    AutoReconnect = true,
                    LogLevel = LogLevel.Debug,
                    UseInternalLogHandler = true
                };

                await SetupBotAsync(cfg);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with config or runtime error: {e.GetType()} - {e.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }

        }
        
        
        public async Task SetupBotAsync(DiscordConfig cfg)
        {

            //Create instance of client
            this.Client = new DiscordClient(cfg);
            
            //Create events
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientError += this.Client_ClientError;
            
            //Connect and login
            await this.Client.ConnectAsync();
            
            //Prevent premature quitting
            await Task.Delay(-1);
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