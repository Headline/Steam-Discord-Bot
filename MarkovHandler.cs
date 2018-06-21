using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Threading.Tasks;
using SteamDiscordBot;
using Discord;

using Markov;

class MarkovHandler
{
    private ConcurrentDictionary<ulong, MarkovChain<string>> dict;
    private ConcurrentDictionary<ulong, string> nextResponse;
    private Random rand;

    public MarkovHandler()
    {
        dict = new ConcurrentDictionary<ulong, MarkovChain<string>>();
        nextResponse = new ConcurrentDictionary<ulong, string>();
        rand = new Random();
    }

    public async Task AddGuild(ulong guild)
    {
        if (dict.ContainsKey(guild)) // we must've disconnected briefly, but still have our data
            return;

        var path = Helpers.BuildPath(guild + ".txt");
        var markov = new MarkovChain<string>(1);

        dict.TryAdd(guild, markov);
        nextResponse.TryAdd(guild, "");

        if (!File.Exists(path))
        {
            File.CreateText(path); // guild non-existant. Make empty file and move on
            return;
        }

        await LoadGraph(markov, path, guild);
        BuildNext(guild);
    }

    private async Task LoadGraph(MarkovChain<string> markov, string path, ulong guild)
    {
        try
        {
            /* 
             * Unforunately with huge files, using StreamReader alone is too slow,
             * so we use BufferedStream to speed things up a bit. 
             */
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                using (StreamReader reader = new StreamReader(bufferedStream))
                {

                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] pieces = line.Split(' ');
                        markov.Add(pieces, 1);
                    }
                }
            }
        }
        catch { }
    }

    public void WriteToGuild(ulong guild, string line)
    {
        if (WriteLineToFile(guild + ".txt", line))
        {
            string[] pieces = line.Split(' ');
            this.dict[guild].Add(pieces, 1);
        }
    }

    public async Task<int> RemoveFromGuild(ulong guild, string term)
    {
        int retval = await MarkovHandler.RemoveTermFromFile(guild + ".txt", term);
        var markov = new MarkovChain<string>(1);
        dict.TryAdd(guild, markov);

        await LoadGraph(markov, Helpers.BuildPath(guild + ".txt"), guild);
        BuildNext(guild);
        return retval;
    }

    public string ReadFromGuild(ulong guild)
    {
        var str = nextResponse[guild];
        BuildNext(guild);

        return str;
    }

    public void BuildNext(ulong guild)
    {
        var markovstr = this.dict[guild];
        this.nextResponse[guild] = string.Join(" ", markovstr.Chain(rand));

        if (this.nextResponse[guild] == "" && this.dict[guild].GetStates().Count() > 0)
        {
            BuildNext(guild);
        }
    }


    public string GetHastebinLink(ulong guild)
    {
        string[] inputArray = File.ReadAllLines(guild + ".txt");

        string input = "";
        foreach (string line in inputArray)
            input += line + "\n";

        return Helpers.UploadHastebin(input);
    }

    private static bool WriteLineToFile(string file, string line)
    {
        if (line.ToLower().Contains("http")
          || line.ToLower().Contains(".com")
          || line.Split(' ').Length < 3
          || line.Contains("<")
          || line[line.Length - 1] == ';'
          || line.Contains("()")
          || line.Contains("{")
          || line.Contains("}")
          || line.Contains("```"))
        {
            return false;
        }

        try
        {
            using (FileStream stream = new FileStream(Helpers.BuildPath(file), FileMode.Append))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes((line + "\n"));

                stream.Write(info, 0, info.Length);
            }
            return true;
        }
        catch (Exception e)
        {
            Program.Instance.Log(new LogMessage(LogSeverity.Info, "MarkovHandler.WriteLineToFile", e.Message));
            return false;
        }
    }

    public static async Task<int> RemoveTermFromFile(string file, string needle)
    {
        int count = 0;
        List<string> array = new List<string>();

        using (FileStream fileStream = File.Open(Helpers.BuildPath(file), FileMode.Open))
        using (BufferedStream bufferedStream = new BufferedStream(fileStream))
        using (StreamReader reader = new StreamReader(bufferedStream))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!line.Contains(needle))
                    array.Add(line);
                else
                    count++;
            }
        }


        using (FileStream fileStream = File.Open(Helpers.BuildPath(file), FileMode.Create))
        using (BufferedStream bufferedStream = new BufferedStream(fileStream))
        using (StreamWriter reader = new StreamWriter(bufferedStream))
        {
            foreach(string line in array)
            {
                await reader.WriteLineAsync(line);
            }
        }

        return count;
    }

    public string GetPhraseFromFile(ulong file, string term = null)
    {
        return this.nextResponse[file];
    }
}