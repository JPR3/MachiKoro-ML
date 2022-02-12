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
        public readonly string name;
        public int NumCoins { get; private set; }
        readonly Random random = new Random();
        readonly List<Card> cards = new List<Card>();
        public bool HasTrain { get; private set; }
        public bool HasMall { get; private set; }
        public bool HasPark { get; private set; }
        public bool HasRadio { get; private set; }
        public bool HasStadium { get; private set; }
        public bool HasStation { get; private set; }
        public bool HasCenter { get; private set; }
        public int NumLandmarks { get; private set; } = 0;
        public bool canReroll = false;
        public readonly Game game;
        public readonly ComputerBase parentComputer;
        public readonly bool shouldLog;
        public PlayerHandler(Game game, string nm, bool log)
        {
            shouldLog = log;
            name = nm;
            this.game = game;
            NumCoins = 3;
            AddCard(new Card(Card.Establishments.wheat_field, this, game));
            AddCard(new Card(Card.Establishments.bakery, this, game));
        }
        public PlayerHandler(Game game, string nm, ComputerBase cpu, bool log)
        {
            shouldLog = log;
            name = nm;
            this.game = game;
            parentComputer = cpu;
            parentComputer.SetHandler(this);
            NumCoins = 3;
            AddCard(new Card(Card.Establishments.wheat_field, this, game));
            AddCard(new Card(Card.Establishments.bakery, this, game));
        }
        public void AddTrain()
        {
            HasTrain = true;
            NumLandmarks++;
        }
        public void AddMall()
        {
            HasMall = true;
            NumLandmarks++;
        }
        public void AddPark()
        {
            HasPark = true;
            NumLandmarks++;
        }
        public void AddRadio()
        {
            HasRadio = true;
            canReroll = true;
            NumLandmarks++;
        }
        public void AddStadium()
        {
            HasStadium = true;
        }
        public void AddStation()
        {
            HasStation = true;
        }
        public void AddCenter()
        {
            HasCenter = true;
        }
        public int GetMaxSteal(int targetMax)
        {
            int toSteal = NumCoins;
            if(toSteal > targetMax) { toSteal = targetMax; }
            return toSteal;
        }
        public bool ChangeCoins(int c)
        {
            if(c < 0 && NumCoins + c < 0) { return false; }
            NumCoins += c;
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

        public RollData Roll(bool rollTwo)
        {
            int firstRoll;
            int secondRoll = 0;
            firstRoll = random.Next(1, 7);
            if(rollTwo)
            {
                secondRoll = random.Next(1, 7);
            }
            return new RollData(firstRoll, secondRoll, (firstRoll == secondRoll) && HasPark);
        }

        override public string ToString()
        {
            return name;
        }

        public string GetInfo()
        {
            string str = $"Name: {name}\r\nCoins: {NumCoins}\r\nCards\r\n";
            for(int i = 0; i < cards.Count; i++)
            {
                str += $"\t{cards[i]}\r\n";
            }
            if(parentComputer != null)
            {
                if(parentComputer.genome != null)
                {
                    str += $"\r\nGenome: {parentComputer.genome}\r\n";
                    str += new string(' ', parentComputer.GenomeIndex + 8) + "^\r\n";
                    str += $"Target: {parentComputer.GetTargetName()}\r\n";
                }
                else
                {
                    str += "No genome\r\n";
                }
            }
            return str;
        }
        public Card[] GetCardsAsArray()
        {
            return cards.ToArray();
        }
        public int NumOfSymbol(Card.Symbols targetSymbol)
        {
            int count = 0;
            foreach(Card c in cards)
            {
                if(c.Symbol == targetSymbol)
                {
                    count++;
                }
            }
            return count;
        }
    }
    public class RollData
    {
        public readonly int rollVal1;
        public readonly int rollVal2;
        public readonly bool doubles;

        public RollData(int val1, int val2, bool db)
        {
            rollVal1 = val1;
            rollVal2 = val2;
            doubles = db;
        }
    }
}
