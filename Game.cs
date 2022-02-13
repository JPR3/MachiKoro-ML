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
        public PlayerHandler CurrentPlayer { get; private set; }
        int currentPlayerIndex;
        public List<Card> allCards = new List<Card>();
        private readonly Program prog;
        private readonly bool isMatch;
        public Game(int numPlayers, Program program)
        {
            currentPlayerIndex = 0;
            players = new PlayerHandler[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new PlayerHandler(this, "Player " + (i + 1), true);
            }
            CurrentPlayer = players[0];
            prog = program;
        }
        public Game(int numHumans, int numComputers, Program program, bool enableLogging, bool isSet)
        {
            currentPlayerIndex = 0;
            this.isMatch = isSet;
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
                players[i] = new PlayerHandler(this, "Player " + (i + 1), true);
            }
            if(numComputers != 0)
            {
                //Add computer players
                for (int j = 0; j < numComputers; j++)
                {
                    PlayerHandler newComputer = new PlayerHandler(this, "Computer " + (j + 1), new ComputerBase(new Genome()), enableLogging);
                    players[playersIndex] = newComputer;
                    playersIndex++;
                }
            }
            else //Dummy test game
            {
                players[playersIndex] = new PlayerHandler(this, "Wheatly", new ComputerBase(Card.Establishments.wheat_field), enableLogging);
                players[playersIndex + 1] = new PlayerHandler(this, "Gump", new ComputerBase(Card.Establishments.forest), enableLogging);
            }
            CurrentPlayer = players[0];
            prog = program;
            if(numHumans == 0)
            {
                CurrentPlayer.parentComputer.TakeTurn();
            }
        }
        public Game(Genome[] genomes, Program program, bool enableLogging, bool isMatch)
        {
            currentPlayerIndex = 0;
            this.isMatch = isMatch;
            players = new PlayerHandler[genomes.Length];
            //Add computer players
            for (int j = 0; j < genomes.Length; j++)
            {
                PlayerHandler newComputer = new PlayerHandler(this, "Computer " + (j + 1), new ComputerBase(genomes[j]), enableLogging);
                players[j] = newComputer;
            }
            CurrentPlayer = players[0];
            prog = program;
            CurrentPlayer.parentComputer.TakeTurn();
        }
        public void IncrementTurn()
        {
            //Reset values for current player
            if(CurrentPlayer.HasRadio)
            {
                CurrentPlayer.canReroll = true;
            }
            //Check for a win
            if(CurrentPlayer.HasMall && CurrentPlayer.HasPark && CurrentPlayer.HasRadio && CurrentPlayer.HasTrain)
            {
                //Order the winners
                PlayerHandler[] orderedResults = new PlayerHandler[players.Length];
                Array.Copy(players, orderedResults, players.Length);
                PlayerHandler temp;
                for(int i = 0; i < orderedResults.Length; i++)
                {
                    for (int j = i + 1; j < orderedResults.Length; j++)
                    {
                        if (orderedResults[i].NumLandmarks < orderedResults[j].NumLandmarks || orderedResults[i].NumLandmarks == orderedResults[j].NumLandmarks && orderedResults[i].NumCoins < orderedResults[j].NumCoins)
                        {
                            
                            temp = orderedResults[i];
                            orderedResults[i] = orderedResults[j];
                            orderedResults[j] = temp;
                        }
                    }
                }
                if(isMatch)
                {
                    prog.AddMatchResults(orderedResults);
                    return;
                }
                prog.EndGame(orderedResults);
                return;
            }
            //Change the current player
            currentPlayerIndex++;
            if(currentPlayerIndex == players.Length) { currentPlayerIndex = 0; }
            CurrentPlayer = players[currentPlayerIndex];
            if(CurrentPlayer.shouldLog)
            {
                Console.WriteLine($"{CurrentPlayer}'s turn!");
            }
            //If the current player is a computer, have them take their turn
            if(CurrentPlayer.parentComputer != null)
            {
                CurrentPlayer.parentComputer.TakeTurn();
            }
        }
        public void EvaluateRoll(int roll, bool doubles)
        {
            if (CurrentPlayer.HasRadio && CurrentPlayer.canReroll && CurrentPlayer.parentComputer == null) //Allow computers to reroll at some point
            {
                Console.WriteLine("Would you like to reroll your dice? (Y/N)");
                while (true)
                {
                    string str = Console.ReadLine();
                    if (str.ToLower().Equals("y"))
                    {
                        CurrentPlayer.canReroll = false;
                        RollData data;
                        if (CurrentPlayer.HasTrain)
                        {
                            //Determine how many dice to roll
                            Console.WriteLine("Roll two dice? (Y/N)");
                            while (true)
                            {
                                string str2 = Console.ReadLine();
                                if (str2.ToLower().Equals("y"))
                                {
                                    data = CurrentPlayer.Roll(true);
                                    break;
                                }
                                else if (str2.ToLower().Equals("n"))
                                {
                                    data = CurrentPlayer.Roll(false);
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
                            data = CurrentPlayer.Roll(false);
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
                if (c.ActivationNums == null) { continue; }
                for (int i = 0; i < c.ActivationNums.Length; i++)
                {
                    if (c.ActivationNums[i] == roll)
                    {
                        c.Invoke(CurrentPlayer);
                    }
                }
            }
            
            if(CurrentPlayer.shouldLog)
            {
                Console.WriteLine();
                PrintBalances();
            }
            
            if (CurrentPlayer.NumCoins == 0)
            {
                if (CurrentPlayer.shouldLog)
                {
                    Console.WriteLine($"\r\n{CurrentPlayer} does not have enough money to buy anything");
                }
                if (!doubles)
                {
                    IncrementTurn();
                }
                else 
                {
                    if (CurrentPlayer.shouldLog)
                    {
                        Console.WriteLine($"\r\n{CurrentPlayer} goes again, because they rolled doubles");
                    }
                    if(CurrentPlayer.parentComputer != null)
                    {
                        CurrentPlayer.parentComputer.TakeTurn();
                    }
                }
                return;
            }

            //Buy phase
            if(CurrentPlayer.parentComputer == null)
            {
                PlayerBuy(doubles);
            }
            else //Computer is buying
            {
                Card newCard = new Card(CurrentPlayer.parentComputer.Target, CurrentPlayer, this);
                if (newCard.Cost <= CurrentPlayer.NumCoins)
                {
                    CurrentPlayer.AddCard(newCard);
                    if (newCard.Symbol == Card.Symbols.landmark)
                    {
                        newCard.Invoke(CurrentPlayer);
                    }
                    CurrentPlayer.ChangeCoins(-newCard.Cost);
                    if (CurrentPlayer.shouldLog)
                    {
                        Console.WriteLine($"\r\nBought {newCard}\r\n{CurrentPlayer} has {CurrentPlayer.NumCoins} coins remaining\r\n");
                    }
                    CurrentPlayer.parentComputer.IncGenome();
                }
                else if (CurrentPlayer.shouldLog)
                {
                    Console.WriteLine($"\r\n{CurrentPlayer} did not buy anything");
                }

                if (!doubles)
                {
                    IncrementTurn();
                }
                else 
                {
                    if (CurrentPlayer.shouldLog)
                    {
                        Console.WriteLine($"\r\n{CurrentPlayer} goes again, because they rolled doubles");
                    }
                    CurrentPlayer.parentComputer.TakeTurn();
                }
                return;
            }
            
        }

        private void PlayerBuy(bool doubles)
        {
            Console.WriteLine("\r\nChoose a card to buy, or pass\r\n");
            Console.WriteLine($"Purchasable cards:\r\n{Card.GetPurchasableEstablishments(CurrentPlayer)}");
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
                        Console.WriteLine($"\r\n{CurrentPlayer} goes again, because they rolled doubles");
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
                    Card newCard = new Card(est, CurrentPlayer, this);
                    if (newCard.Cost <= CurrentPlayer.NumCoins)
                    {
                        CurrentPlayer.AddCard(newCard);
                        if(newCard.Symbol == Card.Symbols.landmark)
                        {
                            newCard.Invoke(CurrentPlayer);
                        }
                        CurrentPlayer.ChangeCoins(-newCard.Cost);
                        Console.WriteLine($"\r\nBought {newCard}\r\n{CurrentPlayer} has {CurrentPlayer.NumCoins} coins remaining\r\n");
                        if (!doubles)
                        {
                            IncrementTurn();
                        }
                        else
                        {
                            Console.WriteLine($"\r\n{CurrentPlayer} goes again, because they rolled doubles");
                        }
                        return;

                    }
                    else
                    {
                        Console.WriteLine($"Cannot afford {newCard} - you have {CurrentPlayer.NumCoins} coins");
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
                Console.WriteLine($"{p}: {p.NumCoins}");
                if (p.parentComputer != null)
                {
                    string str = "";
                    str += $"\r\nGenome: {p.parentComputer.genome}\r\n";
                    str += new string(' ', p.parentComputer.GenomeIndex + 8) + "^\r\n";
                    str += $"Target: {p.parentComputer.GetTargetName()}\r\n";
                    Console.WriteLine(str);
                }
            }
        }
        bool CheckBuyValidity(Card.Establishments est)
        {
            switch(est)
            {
                case Card.Establishments.stadium:
                    if (CurrentPlayer.HasStadium) { return false; }
                    break;
                case Card.Establishments.tv_station:
                    if (CurrentPlayer.HasStation) { return false; }
                    break;
                case Card.Establishments.business_center:
                    if (CurrentPlayer.HasCenter) { return false; }
                    break;
                case Card.Establishments.train_station:
                    if (CurrentPlayer.HasTrain) { return false; }
                    break;
                case Card.Establishments.shopping_mall:
                    if (CurrentPlayer.HasMall) { return false; }
                    break;
                case Card.Establishments.amusement_park:
                    if (CurrentPlayer.HasPark) { return false; }
                    break;
                case Card.Establishments.radio_tower:
                    if (CurrentPlayer.HasRadio) { return false; }
                    break;
            }
            return true;
        }
    }
}
