using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class PlayerHandler
    {
        string name;
        public int numCoins { get; private set; }
        Random random = new Random();
        List<Card> cards = new List<Card>();
        int diceToRoll = 1;
        Game game;

        public PlayerHandler(Game game, string nm)
        {
            name = nm;
            this.game = game;
            numCoins = 3;
            AddCard(new Card(Card.Establishments.wheat_field, this));
            AddCard(new Card(Card.Establishments.bakery, this));
        }

        public bool ChangeCoins(int c)
        {
            if(c < 0 && numCoins + c < 0) { return false; }
            numCoins += c;
            return true;
        }
        public void AddCard(Card card)
        {
            cards.Add(card);
            game.allCards.Add(card);
        }

        public int Roll()
        {
            return random.Next(1, (6 * diceToRoll) + 1);
        }

        override public string ToString()
        {
            return name;
        }

        public string GetInfo()
        {
            string str = $"Name: {name}\r\nCoins: {numCoins}\r\nCards\r\n";
            for(int i = 0; i < cards.Count; i++)
            {
                str += $"\t{cards[i]}\r\n";
            }
            return str;
        }
    }
}
