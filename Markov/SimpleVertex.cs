using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot.Markov
{
    public class SimpleVertex<T>
    {
        private T data;
        private HashSet<SimpleEdge<T>> adjacencies;

        /**
         * Gets the data stored.
         * 
         * @return the data
         */
        public T GetData()
        {
            return data;
        }

        /**
         * Sets the data to store
         * 
         * @param data the data to set
         */
        public void SetData(T data)
        {
            this.data = data;
        }

        /**
         * Adds input adjacencies to the calling object's adjacencies, merging and
         * incrementing weights as needed.
         * 
         * @param inputEdge edges to input
         */
        public void AddAdjacencies(HashSet<SimpleEdge<T>> inputEdge)
        {
            foreach (SimpleEdge<T> edge in inputEdge)
            {
                this.AddAdjacency(edge);
            }
        }

        /**
         * Adds input adjacency to the calling object's adjacency list, merging and
         * incrementing weights as needed.
         * 
         * @param inputEdge edge to add
         */
        protected void AddAdjacency(SimpleEdge<T> inputEdge)
        {
            /* Search for pre-existing adjacency */
            foreach (SimpleEdge<T> edge in adjacencies)
            {
                if (edge != null)
                {
                    /* If matched then we're gonna increase weight by 1 */
                    if (edge.Equals(inputEdge))
                    {
                        edge.SetWeight(edge.GetWeight() + 1);
                        return;
                    }
                }
            }

            /* If not found, then just add the adjacency */
            adjacencies.Add(inputEdge);
        }

        /**
         * Gets the edge
         * 
         * @return the adjacencies
         */
        public HashSet<SimpleEdge<T>> GetAdjacencies()
        {
            return adjacencies;
        }

        /**
         * Set the edge
         * 
         * @param adjacencies the edge adjacencies
         */
        public void SetAdjacencies(HashSet<SimpleEdge<T>> adjacencies)
        {
            this.adjacencies = adjacencies;
        }

        /**
         * Constructs a simple vertex given some data and the edge leading to
         * the next vertex, or none if null.
         * 
         * @param data The data.
         */
        public SimpleVertex(T data)
        {
            this.data = data;
            this.adjacencies = new HashSet<SimpleEdge<T>>();
        }

        /**
         * Constructs a simple vertex, assigning both data and edge to null
         */
        public SimpleVertex()
        {
            this.data = default(T);
            this.adjacencies = new HashSet<SimpleEdge<T>>();
        }

        public override String ToString()
        {
            return this.data.ToString();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            else
            {
                SimpleVertex<T> other = (SimpleVertex<T>)obj;

                return other.GetData().Equals(this.GetData());
            }
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }
    }
}
