using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadBot
{
    class MyBot
    {

        DiscordClient discord;
        CommandService commands;

        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            commands = discord.GetService<CommandService>();

            RegisterDownloadCommand();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("TOKEN", TokenType.Bot);
            });
        }

        private void RegisterDownloadCommand()
        {
            commands.CreateCommand("downloads").Do(async (e) =>
            {
                if (e.Channel.Name == "general")
                {
                    await e.Channel.Edit(null, "Downloads: " + getDownloads());
                }
            });
        }

        private string getDownloads()
        {
            int counter = 0;
            string line;
            
            System.IO.StreamReader file =
               new System.IO.StreamReader("C:\\Users\\colli\\Desktop\\DownloadCounterBot\\downloads.tmp");

            string toReturn = "";
            while ((line = file.ReadLine()) != null)
            {
                toReturn += line;
                counter++;
            }

            file.Close();

            return toReturn;
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
