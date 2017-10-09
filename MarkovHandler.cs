using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MarkovSharp.TokenisationStrategies;

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