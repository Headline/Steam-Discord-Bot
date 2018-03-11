using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot.Markov
{
    public class Graph<T> : WeightedUndirectedGraph<SimpleVertex<string>, SimpleEdge<string>>
    {
        protected HashSet<SimpleVertex<string>> adjList;

        public Graph()
        {
            this.adjList = new HashSet<SimpleVertex<string>>();
        }

        protected override void AddVertex(SimpleVertex<string> vertex)
        {
            if (vertex.GetAdjacencies().Count == 0)
            {
                return;
            }

            bool retVal = adjList.Add(vertex);

            if (!retVal)
            {
                foreach (SimpleVertex<string> x in this.adjList)
                {
                    if (x.Equals(vertex))
                    {
                        x.AddAdjacencies(vertex.GetAdjacencies());
                    }
                }
            }

            HashSet<SimpleEdge<string>> set = vertex.GetAdjacencies();
            foreach (SimpleEdge<string> edge in set.ToList())
            {
                if (edge != null)
                {
                    this.AddVertex(edge.GetTo());
                }
            }
        }

        protected override void AddEdge(SimpleVertex<string> source, SimpleVertex<string> target, int weight)
        {
            SimpleEdge<string> edge = new SimpleEdge<string>(target, weight);

            source.GetAdjacencies().Add(edge);
        }

        protected override ISet<SimpleEdge<string>> EdgeSet(SimpleVertex<string> vertex)
        {
            return new HashSet<SimpleEdge<string>>(vertex.GetAdjacencies());
        }
    }
}
