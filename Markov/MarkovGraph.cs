using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot.Markov
{
    public class MarkovGraph : Graph<String>, IMarkovBot
    {

        private Random rand;
        private HashSet<SimpleVertex<String>> startingWords;
        private List<String> sourceLines;

        public List<string> SourceLines { get => sourceLines; }

        /**
	     * Default constructor for MarkovGraph
	     */
        public MarkovGraph()
        {
            this.rand = new Random();
            this.startingWords = new HashSet<SimpleVertex<String>>();
        }

        /**
         * Constructor for MarkovGraph which will train a
         * collection of phrases
         */
        public MarkovGraph(ICollection<String> phrases)
        {
            this.rand = new Random();
            this.startingWords = new HashSet<SimpleVertex<String>>();

            foreach (String phrase in phrases)
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
	     * @param sb String Builder to build words
	     */
        private void BuildStringFromVertex(SimpleVertex<String> vertex, StringBuilder sb)
        {
            sb.Append(vertex.GetData());
            sb.Append(" ");

            List<SimpleVertex<String>> list = new List<SimpleVertex<String>>();

            foreach (SimpleEdge<String> edge in vertex.GetAdjacencies())
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
                SimpleVertex<String> lucky = list.ElementAt(rand.Next(list.Count));
                BuildStringFromVertex(lucky, sb);
            }
        }

        public bool Contains(String word)
        {
            foreach (SimpleVertex<String> vertex in this.adjList)
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

        public String GetPhraseContains(String value)
        {
            if (!this.adjList.Contains(new SimpleVertex<String>(value)))
            {
                return null;
            }

            bool found = false;
            String attempt = "";
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

        public void Train(String input)
        {
            this.sourceLines.Add(input);
            String[] temp = input.Split(' ');
            this.Train(temp);
        }

        public void Train(String[] input)
        {
            List<SimpleVertex<String>> verticies = new List<SimpleVertex<String>>();

            for (int i = 0; i < input.Length; i++)
            {
                SimpleVertex<String> vertex = new SimpleVertex<String>(input[i]);
                verticies.Add(vertex);
                if (i == 0)
                {
                    startingWords.Add(vertex);
                }
            }

            for (int i = 0; i < verticies.Count - 1; i++)
            {
                SimpleVertex<String> vertex = verticies.ElementAt(i);

                this.AddEdge(vertex, verticies.ElementAt(i + 1), SimpleEdge<String>.DEFAULT_WEIGHT);
            }

            this.AddVertex(verticies.ElementAt(0));
        }

        public String GetPhrase()
        {
            StringBuilder sb = new StringBuilder();
            SimpleVertex<String> vertex = GetStartingVertex();
            BuildStringFromVertex(vertex, sb);
            return sb.ToString();
        }

        /**
	     * Returns a random starting vertex.
	     * 
	     * @return random starting vertex.
	     */
        private SimpleVertex<String> GetStartingVertex()
        {
            List<SimpleVertex<String>> list = new List<SimpleVertex<String>>();
            foreach (SimpleVertex<String> vertex in this.startingWords)
            {
                if (vertex != null)
                {
                    list.Add(vertex);
                }
            }

            return list.ElementAt(rand.Next(list.Count));
        }
    }
}
