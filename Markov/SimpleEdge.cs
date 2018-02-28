using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamDiscordBot.Markov
{
    public class SimpleEdge<T>
    {
        public static readonly int DEFAULT_WEIGHT = 1;

        private int weight;
        private SimpleVertex<T> to;

        /**
         * Gets the weight for the edge
         * 
         * @return the weight
         */
        public int GetWeight()
        {
            return weight;
        }

        /**
         * Sets the weight for the edge
         * 
         * @param weight the weight to set
         */
        public void SetWeight(int weight)
        {
            this.weight = weight;
        }

        /**
         * Gets the next vertex
         * 
         * @return the next vertex
         */
        public SimpleVertex<T> GetTo()
        {
            return to;
        }

        /**
         * Sets the next vertex
         * 
         * @param to The next vertex
         */
        public void SetTo(SimpleVertex<T> to)
        {
            this.to = to;
        }

        /**
         * Constructs a simple vertex given some data and the edge leading to
         * the next vertex, or none if null.
         * 
         * @param to data The data.
         * @param weight next The edge.
         */
        public SimpleEdge(SimpleVertex<T> to, int weight)
        {
            this.weight = weight;
            this.to = to;
        }

        /**
         * Constructs a simple vertex, assigning both data and edge to null
         */
        public SimpleEdge()
        {
            this.weight = SimpleEdge<T>.DEFAULT_WEIGHT;
            this.to = null;
        }

        public override String ToString()
        {
            return "Weight: " + weight + " | Link: " + to.ToString();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            else
            {
                SimpleEdge<T> other = (SimpleEdge<T>)obj;

                return this.to.Equals(other.GetTo()) && this.GetWeight() == other.GetWeight();
            }
        }

        public override int GetHashCode()
        {
            return this.GetTo().GetHashCode();
        }
    }
}
