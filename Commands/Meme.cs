using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SteamDiscordBot.Commands
{
    public class MemeCommand : ModuleBase
    {
        [Command("meme"), Summary("Replaces the last image posted with the text supplied")]
        public async Task Say(params string[] args)
        {
            try
            {
                string input = "";
                for (int i = 0; i < args.Length - 1; i++)
                {
                    input += args[i] + " ";
                }
                input += args[args.Length - 1];

                if (Context.Message.Attachments.Count == 0)
                {
                    var emb = new EmbedBuilder();
                    emb.Title = "Error!";
                    emb.WithDescription("You must send a file along with this command!");
                    emb.Color = Discord.Color.Red;

                    await Context.Channel.SendMessageAsync("", false, emb);
                    return;
                }

                var it = Context.Message.Attachments.GetEnumerator();
                it.MoveNext();
                var attachment = it.Current;

                using (WebClient client = new WebClient())
                using (Stream stream = client.OpenRead(attachment.Url))
                using (Bitmap bitmap = new Bitmap(stream))
                {
                    bitmap.Save("newfile.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                await Context.Message.DeleteAsync();

                DuckCommand.PlaceText(input, "newfile.jpg");
                await Context.Channel.SendFileAsync("temp.jpg");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + '\n' + ex.StackTrace);
            }
        }
    }
}
