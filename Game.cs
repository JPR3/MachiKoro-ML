using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class Game
    {
        public readonly PlayerHandler[] players;
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
            //Reset values for current player
            if(currentPlayer.hasRadio)
            {
                currentPlayer.canReroll = true;
            }
            //Change the current player
            currentIndex++;
            if(currentIndex == players.Length) { currentIndex = 0; }
            currentPlayer = players[currentIndex];
            Console.WriteLine($"{currentPlayer}'s turn!");
        }
        public void EvaluateRoll(int roll, bool doubles)
        {
            if (currentPlayer.hasRadio && currentPlayer.canReroll)
            {
                Console.WriteLine("Would you like to reroll your dice? (Y/N)");
                while (true)
                {
                    string str = Console.ReadLine();
                    if (str.ToLower().Equals("y"))
                    {
                        currentPlayer.canReroll = false;
                        RollData data = currentPlayer.Roll();
                        int rollNum = data.rollVal1 + data.rollVal2;
                        if (data.rollVal2 != 0)
                        {
                            Console.WriteLine($"Rolled a {rollNum} ({data.rollVal1} + {data.rollVal2})");
                        }
                        else
                        {
                            Console.WriteLine($"Rolled a {rollNum}");
                        }
                        if (data.doubles)
                        {
                            Console.WriteLine("Doubles!");
                        }
                        EvaluateRoll(rollNum, data.doubles);
                        return;
                    }
                    else if (str.ToLower().Equals("n"))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Enter 'y' or 'n'");
                    }
                }
            }
            foreach (Card c in allCards)
            {
                if(c.activationNums == null) { continue; }
                for (int i = 0; i < c.activationNums.Length; i++)
                {
                    if (c.activationNums[i] == roll)
                    {

                        c.Invoke(currentPlayer);
                    }
                }
            }
            Console.WriteLine();
            PrintBalances();
            if (currentPlayer.numCoins == 0)
            {
                Console.WriteLine($"\r\n{currentPlayer} does not have enough money to buy anything");
                if(!doubles)
                {
                    IncrementTurn();
                }
                else
                {
                    Console.WriteLine($"\r\n{currentPlayer} goes again, because they rolled doubles");
                }
                return;
            }
            
            //Buy phase
            Console.WriteLine("\r\nBuy a card, or pass\r\n");
            Console.WriteLine($"Purchasable cards:\r\n{Card.GetPurchasableEstablishments(currentPlayer.numCoins)}");
            while (true)
            {
                string str = Console.ReadLine();
                string[] args = str.Split(' ');
                if (args[0].ToLower().Equals("pass"))
                {
                    if (!doubles)
                    {
                        IncrementTurn();
                    }
                    else
                    {
                        Console.WriteLine($"\r\n{currentPlayer} goes again, because they rolled doubles");
                    }
                    return;
                }
                else if (args[0].ToLower().Equals("buy"))
                {
                    if (Enum.TryParse(args[1].ToLower(), out Card.Establishments est))
                    {
                        Card newCard = new Card(est, currentPlayer, this);
                        if (newCard.cost <= currentPlayer.numCoins)
                        {
                            currentPlayer.AddCard(newCard);
                            currentPlayer.ChangeCoins(-newCard.cost);
                            Console.WriteLine($"\r\nBought {newCard}\r\n{currentPlayer} has {currentPlayer.numCoins} coins remaining\r\n");
                            if (!doubles)
                            {
                                IncrementTurn();
                            }
                            else
                            {
                                Console.WriteLine($"\r\n{currentPlayer} goes again, because they rolled doubles");
                            }
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"Cannot afford {newCard} - you have {currentPlayer.numCoins} coins");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid card name");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter 'buy' or 'pass'");
                }
            }
        }
        public void PrintBalances()
        {
            Console.WriteLine("Balances:");
            foreach (PlayerHandler p in players)
            {
                Console.WriteLine($"{p}: {p.numCoins}");
            }
        }
    }
}
