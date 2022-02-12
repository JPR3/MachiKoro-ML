﻿using System;
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
        int currentPlayerIndex;
        public List<Card> allCards = new List<Card>();
        private readonly Program prog;
        public Game(int numPlayers, Program program)
        {
            currentPlayerIndex = 0;
            players = new PlayerHandler[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new PlayerHandler(this, "Player " + (i + 1), true);
            }
            currentPlayer = players[0];
            prog = program;
        }
        public Game(int numHumans, int numComputers, Program program, bool enableLogging)
        {
            currentPlayerIndex = 0;
            
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
                //Order the winners
                PlayerHandler[] orderedResults = new PlayerHandler[players.Length];
                Array.Copy(players, orderedResults, players.Length);
                for(int i = 0; i < orderedResults.Length; i++)
                {
                    PlayerHandler p1 = orderedResults[i];
                    for (int j = i + 1; j < orderedResults.Length; j++)
                    {
                        PlayerHandler p2 = orderedResults[j];
                        if (p1.numLandmarks < p2.numLandmarks || p1.numLandmarks == p2.numLandmarks && p1.numCoins < p2.numCoins)
                        {
                            orderedResults[i] = p2;
                            orderedResults[j] = p1;
                        }
                    }
                }
                prog.EndGame(orderedResults);
                return;
            }
            //Change the current player
            currentPlayerIndex++;
            if(currentPlayerIndex == players.Length) { currentPlayerIndex = 0; }
            currentPlayer = players[currentPlayerIndex];
            if(currentPlayer.shouldLog)
            {
                Console.WriteLine($"{currentPlayer}'s turn!");
            }
            //If the current player is a computer, have them take their turn
            if(currentPlayer.parentComputer != null)
            {
                currentPlayer.parentComputer.TakeTurn();
            }
        }
        public void EvaluateRoll(int roll, bool doubles)
        {
            if (currentPlayer.hasRadio && currentPlayer.canReroll && currentPlayer.parentComputer == null) //Allow computers to reroll at some point
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
            
            if(currentPlayer.shouldLog)
            {
                Console.WriteLine();
                PrintBalances();
            }
            
            if (currentPlayer.numCoins == 0)
            {
                if (currentPlayer.shouldLog)
                {
                    Console.WriteLine($"\r\n{currentPlayer} does not have enough money to buy anything");
                }
                if (!doubles)
                {
                    IncrementTurn();
                }
                else 
                {
                    if (currentPlayer.shouldLog)
                    {
                        Console.WriteLine($"\r\n{currentPlayer} goes again, because they rolled doubles");
                    }
                    if(currentPlayer.parentComputer != null)
                    {
                        currentPlayer.parentComputer.TakeTurn();
                    }
                }
                return;
            }

            //Buy phase
            if(currentPlayer.parentComputer == null)
            {
                PlayerBuy(doubles);
            }
            else //Computer is buying
            {
                Card newCard = new Card(currentPlayer.parentComputer.target, currentPlayer, this);
                if (newCard.cost <= currentPlayer.numCoins)
                {
                    currentPlayer.AddCard(newCard);
                    if (newCard.symbol == Card.Symbols.landmark)
                    {
                        newCard.Invoke(currentPlayer);
                    }
                    currentPlayer.ChangeCoins(-newCard.cost);
                    if (currentPlayer.shouldLog)
                    {
                        Console.WriteLine($"\r\nBought {newCard}\r\n{currentPlayer} has {currentPlayer.numCoins} coins remaining\r\n");
                    }
                    currentPlayer.parentComputer.IncGenome();
                }
                else if (currentPlayer.shouldLog)
                {
                    Console.WriteLine($"\r\n{currentPlayer} did not buy anything");
                }

                if (!doubles)
                {
                    IncrementTurn();
                }
                else 
                {
                    if (currentPlayer.shouldLog)
                    {
                        Console.WriteLine($"\r\n{currentPlayer} goes again, because they rolled doubles");
                    }
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
                        if(newCard.symbol == Card.Symbols.landmark)
                        {
                            newCard.Invoke(currentPlayer);
                        }
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
                if (p.parentComputer != null)
                {
                    string str = "";
                    str += $"\r\nGenome: {p.parentComputer.genome}\r\n";
                    str += new string(' ', p.parentComputer.genomeIndex + 8) + "^\r\n";
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
                    if (currentPlayer.HasStadium) { return false; }
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
