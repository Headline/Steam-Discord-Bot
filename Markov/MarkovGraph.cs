using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SteamDiscordBot.Markov
{
    public class MarkovGraph : Graph<string>, IMarkovBot
    {

        private Random rand;
        private HashSet<SimpleVertex<string>> startingWords;
        private List<string> sourceLines;

        public List<string> SourceLines { get => sourceLines; }

        /**
	     * Default constructor for MarkovGraph
	     */
        public MarkovGraph()
        {
            this.sourceLines = new List<string>();
            this.rand = new Random();
            this.startingWords = new HashSet<SimpleVertex<string>>();
        }

        /**
         * Constructor for MarkovGraph which will train a
         * collection of phrases
         */
        public MarkovGraph(ICollection<string> phrases)
        {
            this.sourceLines = new List<string>();
            this.rand = new Random();
            this.startingWords = new HashSet<SimpleVertex<string>>();

            foreach (string phrase in phrases)
            {
                if (phrase != null)
                {
                    this.Train(phrase);
                }
            }
        }

        /**
	     * Recursive method for building sentences starting from a given
	     * vertex.
	     * 
	     * @param vertex Vertex to start from
	     * @param sb string Builder to build words
	     */
        private void BuildStringFromVertex(SimpleVertex<string> vertex, StringBuilder sb)
        {
            sb.Append(vertex.GetData());
            sb.Append(" ");

            List<SimpleVertex<string>> list = new List<SimpleVertex<string>>();

            foreach (SimpleEdge<string> edge in vertex.GetAdjacencies())
            {
                if (edge != null)
                {
                    int weight = edge.GetWeight();
                    while (weight > 0)
                    {
                        list.Add(edge.GetTo());

                        weight--;
                    }
                }
            }

            if (list.Count > 0)
            {
                SimpleVertex<string> lucky = list.ElementAt(rand.Next(list.Count));
                BuildStringFromVertex(lucky, sb);
            }
        }

        public bool Contains(string word)
        {
            foreach (SimpleVertex<string> vertex in this.adjList)
            {
                if (vertex != null)
                {
                    if (vertex.GetData().Equals(word))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetPhraseContains(string value)
        {
            if (!this.adjList.Contains(new SimpleVertex<string>(value)))
            {
                return null;
            }

            bool found = false;
            string attempt = "";
            while (!found)
            {
                attempt = this.GetPhrase();
                if (attempt.Contains(value))
                {
                    found = true;
                }
            }

            return attempt;
        }

        public void Train(string input)
        {
            this.sourceLines.Add(input);
            string[] temp = input.Split(' ');
            this.Train(temp);
        }

        public void Train(string[] input)
        {
            List<SimpleVertex<string>> verticies = new List<SimpleVertex<string>>();

            for (int i = 0; i < input.Length; i++)
            {
                SimpleVertex<string> vertex = new SimpleVertex<string>(input[i]);
                verticies.Add(vertex);
                if (i == 0)
                {
                    startingWords.Add(vertex);
                }
            }

            for (int i = 0; i < verticies.Count - 1; i++)
            {
                SimpleVertex<string> vertex = verticies.ElementAt(i);

                this.AddEdge(vertex, verticies.ElementAt(i + 1), SimpleEdge<string>.DEFAULT_WEIGHT);
            }

            this.AddVertex(verticies.ElementAt(0));
        }

        public string GetPhrase()
        {
            StringBuilder sb = new StringBuilder();
            if (this.startingWords.Count != 0)
            {
                SimpleVertex<string> vertex = GetStartingVertex();
                BuildStringFromVertex(vertex, sb);
            }
            return sb.ToString();
        }

  
        /**
	     * Returns a random starting vertex.
	     * 
	     * @return random starting vertex.
	     */
        private SimpleVertex<string> GetStartingVertex()
        {
            List<SimpleVertex<string>> list = new List<SimpleVertex<string>>();
            foreach (SimpleVertex<string> vertex in this.startingWords)
            {
                if (vertex != null)
                {
                    list.Add(vertex);
                }
            }

            return list.ElementAt(rand.Next(0, list.Count));
        }
    }
}
