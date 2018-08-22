using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace SteamDiscordBot.Commands
{
    public class DuckrCommand : ModuleBase
    {
        [Command("duckr"), Summary("Appends input text to a picture of psychonic. Removes original message")]
        public async Task Say(params string[] args)
        {
            await Context.Message.DeleteAsync();
            await new DuckCommand().Say(args);
        }
    }

    public class DuckCommand : ModuleBase
    { 
            [Command("duck"), Summary("Appends input text to a picture of psychonic.")]
        public async Task Say(params string[] args)
        {
            string input = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                input += args[i] + " ";
            }
            input += args[args.Length - 1];

            try
            {
                Random rand = Program.Instance.random;

                int count = Directory.GetFiles("ducks/", "*", SearchOption.TopDirectoryOnly).Length;
                string filename = string.Format("ducks/duck{0}.jpg", rand.Next(1, count+1));

                Bitmap original = new Bitmap(filename);
                Bitmap bmp = new Bitmap(original, new Size(1200, 912));
                RectangleF rect = new RectangleF(70, 90, bmp.Width-70, bmp.Height-90);

                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.High;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                GraphicsPath p = new GraphicsPath();
                p.AddString(input, FontFamily.GenericSansSerif, (int)FontStyle.Bold, 92, rect, new StringFormat());

                g.DrawPath(new Pen(Color.Black, 5.0f), p);
                g.FillPath(Brushes.White, p);
                g.Flush();

                if (File.Exists("temp.jpeg"))
                    File.Delete("temp.jpeg");

                bmp.Save("temp.jpeg", ImageFormat.Jpeg);
                bmp.Dispose();

                await Context.Channel.SendFileAsync("temp.jpeg");
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(ex.StackTrace);
            }
        }
    }
}
