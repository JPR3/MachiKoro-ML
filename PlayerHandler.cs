using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
        public int numRanches = 0;
        public int numNature = 0;
        public int numAgriculture = 0;

        public PlayerHandler(Game game, string nm)
        {
            name = nm;
            this.game = game;
            numCoins = 3;
            AddCard(new Card(Card.Establishments.wheat_field, this, game));
            AddCard(new Card(Card.Establishments.bakery, this, game));
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
        public void ReplaceCard(Card oldCard, Card newCard)
        {
            if(cards.Contains(oldCard))
            {
                cards.Remove(oldCard);
                cards.Add(newCard);
                newCard.SetNewOwner(this);
            }
            else
            {
                Console.WriteLine($"{this}'s card to trade away does not exist!");
                Trace.TraceError($"{this} does not contain {oldCard} card, or it cannot be found");
            }
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
        public Card[] GetCardsAsArray()
        {
            return cards.ToArray();
        }
    }
}
