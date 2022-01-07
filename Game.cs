using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class Game
    {
        PlayerHandler[] players;
        public PlayerHandler currentPlayer { get; private set; }
        int currentIndex;
        public List<Card> allCards = new List<Card>();

        public Game(int numPlayers)
        {
            currentIndex = 0;
            players = new PlayerHandler[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new PlayerHandler(this, "Player " + (i + 1));
            }
            currentPlayer = players[0];
        }
        public void IncrementTurn()
        {
            currentIndex++;
            if(currentIndex == players.Length) { currentIndex = 0; }
            currentPlayer = players[currentIndex];
            Console.WriteLine($"{currentPlayer}'s turn!");
        }
        public void EvaluateRoll(int roll)
        {
            foreach(Card c in allCards)
            {
                for(int i = 0; i < c.activationNums.Length; i++)
                {
                    if(c.activationNums[i] == roll)
                    {
                        
                        c.Invoke(currentPlayer);
                    }
                }
            }
            Console.WriteLine("\r\nBalances:");
            foreach(PlayerHandler p in players)
            {
                Console.WriteLine($"{p}: {p.numCoins}");
            }
        }
    }
}
