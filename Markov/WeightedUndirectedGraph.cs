using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot.Markov
{
    public abstract class WeightedUndirectedGraph<V, E>
    {
        /**
         * Adds the specified vertex into the graph, if it does not already exist in the graph.
         * @param vertex vertex to add
         */
        protected abstract void AddVertex(V vertex);

        /**
         * Creates an edge between the to vertices
         * @param source Source vertex
         * @param target Target vertex
         */
        protected void AddEdge(V source, V target)
        {
            this.AddEdge(source, target, SimpleEdge<E>.DEFAULT_WEIGHT);
        }

        /**
         * Creates an edge between the to vertices
         * @param source Source vertex
         * @param target Target vertex
         * @param weight for the edge
         */
        protected abstract void AddEdge(V source, V target, int weight);

        /**
         * Returns a set of all edges of a given vertex
         * @param vertex to get the Set for
         * @return a set of edges
         */
        protected abstract ISet<E> EdgeSet(V vertex);
    }
}
