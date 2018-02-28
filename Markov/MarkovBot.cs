using System;

namespace SteamDiscordBot.Markov
{
    public interface IMarkovBot
    {
        /**
         * Train the MarkovBov a phrase, which is separated by spaces.
         * 
         * @param input Phrase to train.
         */
        void Train(String input);

        /**
         * Train the MarkovBot individual words and maintain their links together. If you're intending
         * on separating words by something other than spaces, split into an array and use this instead 
         * of train(String);
         * 
         * @param input array of inputs to feed. 
         */
        void Train(String[] input);

        /**
         * Generates a random phrase created from inputs. The more training given, the better the responses.
         * 
         * @return randomly generated phrase
         */
        String GetPhrase();

        /**
         * Gets a phrase which contains the following word, if it's contained in the graph
         * 
         * @param string input
         * @return Phrase, or null if input does not exist in the graph
         */
        String GetPhraseContains(String str);

        /**
         * Determines if the MarkovBot contains an input with the
         * given word.
         *
         * @param word Word to determine the existence of
         * @return True if found.
         */
        bool Contains(String word);
    }
}
