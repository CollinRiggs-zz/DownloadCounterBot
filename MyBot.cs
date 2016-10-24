using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DownloadBot
{
    class MyBot
    {

        DiscordClient discord;
        CommandService commands;
        Channel channel;

        System.Threading.Timer timer;

        public MyBot()
        {


            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            timer = new System.Threading.Timer((e) =>
            {
                timerTick();
            }, null, 0, 1000);

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });


            commands = discord.GetService<CommandService>();

            RegisterDownloadCommand();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MjM5NDY3NDM2ODYzMjU4NjQ1.Cu6VEA.7Bek5nyQR8LtuiALO0Bbsw8WkqE", TokenType.Bot);
            });
            
        }

        private Boolean updated = false;
        private void timerTick()
        {

            int minute = DateTime.Now.Minute;
            if (minute == 0 && DateTime.Now.Second == 0)
            {
                if (!updated)
                {
                    channel.SendMessage("Downloads: " + getDownloads());
                    channel.Edit(null, "Downloads: " + getDownloads());
                    updated = true;
                }
            } else
            {
                updated = false;
            }
        }

        private void RegisterDownloadCommand()
        {
            commands.CreateCommand("downloads").Do(async (e) =>
            {
                if (e.Channel.Name == "tesla_essentials")
                {
                    this.channel = e.Channel;
                    await e.Channel.Edit(null, "Downloads: " + getDownloads());
                    await e.Channel.SendMessage("Downloads: " + getDownloads());
                }
            });
        }

        private string getDownloads()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://minecraft.curseforge.com/projects/tesla-essentials?gameCategorySlug=mc-mods&projectID=248607");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                string[] matches = data.Split(new string[] { "<div class=\"info-data\">" }, StringSplitOptions.None);
                string[] temp = matches[3].Split(new string[] { "</div>" }, StringSplitOptions.None);

                return temp[0];
            }
            return null;
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
