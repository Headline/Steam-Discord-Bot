using Discord;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SteamDiscordBot
{
    class FactHandler
    {
        private ConcurrentDictionary<ulong, List<string>> facts;

        public FactHandler()
        {
            facts = new ConcurrentDictionary<ulong, List<string>>();
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

        public async Task<int> RemoveFromGuild(ulong guild, string fact)
        {
            // remove from cache
            int count = facts[guild].RemoveAll((x) => x.Equals(fact));
            if (count == 0)
            {
                return 0;
            }

            // remove from file
            count = 0;
            List<string> array = new List<string>();

            using (FileStream fileStream = File.Open(Helpers.BuildPath(guild + "_facts.txt"), FileMode.Open))
            using (BufferedStream bufferedStream = new BufferedStream(fileStream))
            using (StreamReader reader = new StreamReader(bufferedStream))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!line.Contains(fact))
                        array.Add(line);
                    else
                        count++;
                }
            }


            using (FileStream fileStream = File.Open(Helpers.BuildPath(guild + "_facts.txt"), FileMode.Create))
            using (BufferedStream bufferedStream = new BufferedStream(fileStream))
            using (StreamWriter reader = new StreamWriter(bufferedStream))
            {
                foreach (string line in array)
                {
                    await reader.WriteLineAsync(line);
                }
            }

            return count;
        }

        public async Task AddGuild(ulong guild)
        {
            if (facts.ContainsKey(guild)) // we must've disconnected briefly, but still have our data
                return;

            var path = Helpers.BuildPath(guild + "_facts.txt");
            var list = new List<string>();
            facts.TryAdd(guild, list);

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
