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
        private readonly Program prog;
        public Game(int numPlayers, Program program)
        {
            currentIndex = 0;
            players = new PlayerHandler[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new PlayerHandler(this, "Player " + (i + 1));
            }
            currentPlayer = players[0];
            prog = program;
        }
        public Game(int numHumans, int numComputers, Program program)
        {
            currentIndex = 0;
            
            if (numComputers != 0)
            {
                players = new PlayerHandler[numHumans + numComputers];
                
            }
            else
            {
                players = new PlayerHandler[numHumans + 2];
            }
            int playersIndex = numHumans;
            //Add human players
            for (int i = 0; i < numHumans; i++)
            {
                players[i] = new PlayerHandler(this, "Player " + (i + 1));
            }
            if(numComputers != 0)
            {
                //Add computer players
                for (int j = 0; j < numComputers; j++)
                {
                    PlayerHandler newComputer = new PlayerHandler(this, "Computer " + (j + 1), new ComputerBase(new Genome()));
                    players[playersIndex] = newComputer;
                    playersIndex++;
                }
            }
            else //Dummy test game
            {
                players[playersIndex] = new PlayerHandler(this, "Wheatly", new ComputerBase(Card.Establishments.wheat_field));
                players[playersIndex + 1] = new PlayerHandler(this, "Gump", new ComputerBase(Card.Establishments.forest));
            }
            currentPlayer = players[0];
            prog = program;
            if(numHumans == 0)
            {
                currentPlayer.parentComputer.TakeTurn();
            }
        }
        public void IncrementTurn()
        {
            //Reset values for current player
            if(currentPlayer.hasRadio)
            {
                currentPlayer.canReroll = true;
            }
            //Check for a win
            if(currentPlayer.hasMall && currentPlayer.hasPark && currentPlayer.hasRadio && currentPlayer.hasTrain)
            {
                prog.EndGame(currentPlayer);
                return;
            }
            //Change the current player
            currentIndex++;
            if(currentIndex == players.Length) { currentIndex = 0; }
            currentPlayer = players[currentIndex];
            Console.WriteLine($"{currentPlayer}'s turn!");
            //If the current player is a computer, have them take their turn
            if(currentPlayer.parentComputer != null)
            {
                currentPlayer.parentComputer.TakeTurn();
            }
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
                        RollData data;
                        if (currentPlayer.hasTrain)
                        {
                            //Determine how many dice to roll
                            Console.WriteLine("Roll two dice? (Y/N)");
                            while (true)
                            {
                                string str2 = Console.ReadLine();
                                if (str2.ToLower().Equals("y"))
                                {
                                    data = currentPlayer.Roll(true);
                                    break;
                                }
                                else if (str2.ToLower().Equals("n"))
                                {
                                    data = currentPlayer.Roll(false);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Enter 'y' or 'n'");
                                }
                            }
                        }
                        else
                        {
                            data = currentPlayer.Roll(false);
                        }
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
            //Activate cards
            foreach (Card c in allCards)
            {
                if (c.activationNums == null) { continue; }
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

            //Buy phase
            if(currentPlayer.parentComputer == null)
            {
                PlayerBuy(doubles);
            }
            else
            {
                Card newCard = new Card(currentPlayer.parentComputer.target, currentPlayer, this);
                if (newCard.cost <= currentPlayer.numCoins)
                {
                    currentPlayer.AddCard(newCard);
                    currentPlayer.ChangeCoins(-newCard.cost);
                    Console.WriteLine($"\r\nBought {newCard}\r\n{currentPlayer} has {currentPlayer.numCoins} coins remaining\r\n");
                }
                else
                {
                    Console.WriteLine($"\r\n{currentPlayer} did not buy anything");
                }

                if (!doubles)
                {
                    IncrementTurn();
                }
                else
                {
                    Console.WriteLine($"\r\n{currentPlayer} goes again, because they rolled doubles");
                    currentPlayer.parentComputer.TakeTurn();
                }
                return;
            }
            
        }

        private void PlayerBuy(bool doubles)
        {
            Console.WriteLine("\r\nChoose a card to buy, or pass\r\n");
            Console.WriteLine($"Purchasable cards:\r\n{Card.GetPurchasableEstablishments(currentPlayer)}");
            while (true)
            {
                string str = Console.ReadLine();
                if (str.ToLower().Equals("pass"))
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
                else if (Enum.TryParse(str.ToLower(), out Card.Establishments est))
                {
                    if (!CheckBuyValidity(est))
                    {
                        Console.WriteLine($"Cannot buy duplicates of {est} - you already have one");
                        continue;
                    }
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
                    Console.WriteLine("Please enter a valid card name, or 'pass'");
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
        bool CheckBuyValidity(Card.Establishments est)
        {
            switch(est)
            {
                case Card.Establishments.stadium:
                    if (currentPlayer.hasStadium) { return false; }
                    break;
                case Card.Establishments.tv_station:
                    if (currentPlayer.hasStation) { return false; }
                    break;
                case Card.Establishments.business_center:
                    if (currentPlayer.hasCenter) { return false; }
                    break;
                case Card.Establishments.train_station:
                    if (currentPlayer.hasTrain) { return false; }
                    break;
                case Card.Establishments.shopping_mall:
                    if (currentPlayer.hasMall) { return false; }
                    break;
                case Card.Establishments.amusement_park:
                    if (currentPlayer.hasPark) { return false; }
                    break;
                case Card.Establishments.radio_tower:
                    if (currentPlayer.hasRadio) { return false; }
                    break;
            }
            return true;
        }
    }
}
