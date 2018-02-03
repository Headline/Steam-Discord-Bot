using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using MarkovSharp.TokenisationStrategies;
using Newtonsoft.Json.Linq;

class Markov
{
    private Dictionary<string, StringMarkov> dict;
    private Dictionary<string, int> nextWalk; // used for `.Walk()`ing
    private Dictionary<string, string> nextResponse;

    public Markov(string[] guilds)
    {
        dict = new Dictionary<string, StringMarkov>();
        nextWalk = new Dictionary<string, int>();
        nextResponse = new Dictionary<string, string>();

        foreach (string guild in guilds)
        {
            nextWalk.Add(guild, 0);
            var markov = new StringMarkov(1);
            try
            {
                markov.Learn(File.ReadAllLines(BuildPath(guild + ".txt")));
            }
            catch { } // file not created yet
            dict.Add(guild, markov);
            this.BuildNext(guild);
        }
    }

    public Markov()
    {
        dict = new Dictionary<string, StringMarkov>();
        nextWalk = new Dictionary<string, int>();
        nextResponse = new Dictionary<string, string>();
    }

    public void AddGuild(string guild)
    {
        var markov = new StringMarkov(1);
        try
        {
            markov.Learn(File.ReadAllLines(BuildPath(guild + ".txt")));
        }
        catch { } // file not created yet
        dict.Add(guild, markov);
        nextWalk.Add(guild, 0);
        BuildNext(guild);
    }

    public void WriteToGuild(string guild, string line)
    {
        if (WriteLineToFile(guild + ".txt", line))
            this.dict[guild].Learn(line);
    }

    public int RemoveFromGuild(string guild, string term)
    {
        for (int i = 0; i < this.dict[guild].SourceLines.Count; i++)
        {
            var line = this.dict[guild].SourceLines.ElementAt(i);
            if (line.Contains(term))
            {
                this.dict[guild].SourceLines.RemoveAt(i);
            }
        }
        return Markov.RemoveTermFromFile(guild + ".txt", term);
    }
    public string ReadFromGuild(string guild)
    {
        return nextResponse[guild];
    }

    public void BuildNext(string guild)
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


    public string GetHastebinLink(string guild)
    {
        string[] inputArray = this.dict[guild].SourceLines.ToArray();

        string input = "";

        foreach (string line in inputArray)
            input += line + "\n";

        using (var client = new WebClient())
        {
            client.Headers[HttpRequestHeader.ContentType] = "text/plain";

            var response = client.UploadString("https://hastebin.com/documents", input);
            JObject obj = JObject.Parse(response);

            if (!obj.HasValues)
            {
                return "";
            }

            string key = (string)obj["key"];
            string hasteUrl = "https://hastebin.com/" + key + ".txt";

            return hasteUrl;
        }
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
          || line.Contains("}"))
        {
            return false;
        }


        using (FileStream stream = new FileStream(BuildPath(file), FileMode.Append))
        {
            Byte[] info = new UTF8Encoding(true).GetBytes((line + "\n"));

            stream.Write(info, 0, info.Length);
        }
        return true;
    }

    public static int RemoveTermFromFile(string file, string line)
    {
        string[] lines = File.ReadAllLines(BuildPath(file));
        List<string> array = new List<string>(lines);
        int count = 0;
        for (int i = array.Count()-1; i > 0; i--)
        {
            if (array[i].ToLower().Contains(line))
            {
                count++;
                array.RemoveAt(i);
            }
        }

        File.WriteAllLines(BuildPath(file), array.ToArray());
        return count;
    }

    public string GetPhraseFromFile(string file, string term = null)
    {
        return this.dict[file].Walk().First();
    }

    public static string BuildPath(string file)
    {
        string exe = System.Reflection.Assembly.GetEntryAssembly().Location;
        string[] pieces = exe.Split('/');
        string combo = "";

        for (int i = 0; i < pieces.Length-1; i++)
        {
            combo += pieces + "/";
        }

        return combo + file;
    }
}