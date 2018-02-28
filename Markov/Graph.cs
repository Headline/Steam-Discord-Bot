using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot.Markov
{
    public class Graph<T> : WeightedUndirectedGraph<SimpleVertex<String>, SimpleEdge<String>>
    {
        protected HashSet<SimpleVertex<String>> adjList;

        public Graph()
        {
            this.adjList = new HashSet<SimpleVertex<String>>();
        }

        protected override void AddVertex(SimpleVertex<String> vertex)
        {
            if (vertex.GetAdjacencies().Count == 0)
            {
                return;
            }

            bool retVal = adjList.Add(vertex);

            if (!retVal)
            {
                foreach (SimpleVertex<String> x in this.adjList)
                {
                    if (x.Equals(vertex))
                    {
                        x.AddAdjacencies(vertex.GetAdjacencies());
                    }
                }
            }

            HashSet<SimpleEdge<String>> set = vertex.GetAdjacencies();
            foreach (SimpleEdge<String> edge in set)
            {
                if (edge != null)
                {
                    this.AddVertex(edge.GetTo());
                }
            }
        }

        protected override void AddEdge(SimpleVertex<String> source, SimpleVertex<String> target, int weight)
        {
            SimpleEdge<String> edge = new SimpleEdge<String>(target, weight);

            source.GetAdjacencies().Add(edge);
        }

        protected override ISet<SimpleEdge<String>> EdgeSet(SimpleVertex<String> vertex)
        {
            return new HashSet<SimpleEdge<String>>(vertex.GetAdjacencies());
        }
    }
}
