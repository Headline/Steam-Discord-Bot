using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using MarkovSharp.TokenisationStrategies;
using Newtonsoft.Json.Linq;

static class MarkovHelper
{
    public static void WriteLineToFile(string file, string line)
    {
        if (line.ToLower().Contains("http")
          || line.ToLower().Contains(".com")
          || line.Split(' ').Length < 3
          || line.Contains("<"))
        {
            return;
        }


        using (FileStream stream = new FileStream(BuildPath(file), FileMode.Append))
        {
            Byte[] info = new UTF8Encoding(true).GetBytes((line + "\n"));

            stream.Write(info, 0, info.Length);
        }
    }

    public static string GetHastebinLink(string file)
    {
        string[] inputArray = File.ReadAllLines(BuildPath(file));

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



    public static string GetPhraseFromFile(string file, string term = null)
    {
        string[] lines = File.ReadAllLines(BuildPath(file));

        StringMarkov model = new StringMarkov(1);
        model.Learn(lines);

        if (term == null)
        {
            string result = model.Walk().First();
            return result;
        }
        else
        {
            var results = model.GetMatches(term).Where(val => val != null && !val.Equals("")).ToList();

            foreach (string line in results) Console.WriteLine(line);

            if (results.Count() == 0)
            {
                return "I cant :(";
            }
            else
            {
                return results[new Random().Next(0, results.Count())];
            }
        }
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