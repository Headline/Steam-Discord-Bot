using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot
{
    class FactHandler
    {
        private Dictionary<ulong, List<string>> facts;

        public FactHandler()
        {
            facts = new Dictionary<ulong, List<string>>();
        }

        public void WriteToGuild(ulong guild, string fact)
        {
            try
            {
                using (FileStream stream = new FileStream(Helpers.BuildPath(guild + "_facts.txt"), FileMode.Append))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes((fact + "\n"));

                    stream.Write(info, 0, info.Length);
                }
            }
            catch (Exception e)
            {
                Program.Instance.Log(new LogMessage(LogSeverity.Info, "FactHandler.WriteToGuild", e.Message));
            }
        }

        public async Task AddGuild(ulong guild)
        {
            var path = Helpers.BuildPath(guild + "_facts.txt");
            var list = new List<string>();
            facts.Add(guild, list);

            if (!File.Exists(path))
            {
                File.CreateText(path); // guild non-existant. Make empty file and move on
                return;
            }

            await LoadList(list, path);
        }

        public List<string> GetList(ulong id)
        {
            return facts[id];
        }

        private static async Task LoadList(List<string> list, string path)
        {
            try
            {
                /* 
                 * Unforunately with huge files, using StreamReader alone is too slow,
                 * so we use BufferedStream to speed things up a bit. 
                 */
                using (FileStream fileStream = File.Open(path, FileMode.Open))
                using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                using (StreamReader reader = new StreamReader(bufferedStream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        list.Add(line);
                    }
                }
            }
            catch { }
        }
    }
}
