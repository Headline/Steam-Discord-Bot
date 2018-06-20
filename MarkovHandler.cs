using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Threading.Tasks;
using SteamDiscordBot;
using Discord;

using MarkovSharp.TokenisationStrategies;

class MarkovHandler
{
    private Dictionary<ulong, StringMarkov> dict;
    private Dictionary<ulong, string> nextResponse;
    private Dictionary<ulong, int> nextWalk;

    public MarkovHandler()
    {
        dict = new Dictionary<ulong, StringMarkov>();
        nextResponse = new Dictionary<ulong, string>();
        nextWalk = new Dictionary<ulong, int>();
    }

    public async Task AddGuild(ulong guild)
    {
        if (dict.ContainsKey(guild)) // we must've disconnected briefly, but still have our data
            return;

        var path = Helpers.BuildPath(guild + ".txt");
        var markov = new StringMarkov();

        dict.Add(guild, markov);
        nextWalk.Add(guild, 0);
        nextResponse.Add(guild, "");

        if (!File.Exists(path))
        {
            File.CreateText(path); // guild non-existant. Make empty file and move on
            return;
        }

        await LoadGraph(markov, path, guild);
    }

    private async Task LoadGraph(StringMarkov markov, string path, ulong guild)
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
                    markov.Learn(line);
                }
            }
            BuildNext(guild);
        }
        catch { }
    }

    public void WriteToGuild(ulong guild, string line)
    {
        if (WriteLineToFile(guild + ".txt", line))
            this.dict[guild].Learn(line);
    }

    public async Task<int> RemoveFromGuild(ulong guild, string term)
    {
        int retval = await MarkovHandler.RemoveTermFromFile(guild + ".txt", term);
        var markov = new StringMarkov();
        dict.Add(guild, markov);

        await LoadGraph(markov, Helpers.BuildPath(guild + ".txt"), guild);
        return retval;
    }

    public string ReadFromGuild(ulong guild)
    {
        return nextResponse[guild];
    }

    public void BuildNext(ulong guild)
    {
        var markovstr = this.dict[guild];
        var next = this.nextWalk[guild];

        if (next >= markovstr.Walk().Count()) /* if we used everything, rebuild */
        {
            markovstr.Retrain(1);
            this.nextWalk[guild] = 0;
        }

        nextResponse[guild] = markovstr.Walk().ElementAt(this.nextWalk[guild]++);
    }


    public string GetHastebinLink(ulong guild)
    {
        string[] inputArray = this.dict[guild].SourceLines.ToArray();

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