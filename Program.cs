using System;
using System.Collections.Generic;

namespace MachiKoro_ML
{
    class Initialize
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("Welcome to Machi Koro!\r\nType 'help' for a list of commands");
            Program prog = new Program();
            prog.ReadCommands();
        }
    }
    public class Program //Rename at some point
    {
        Game game;
        Command HELP;
        Command CLEAR;
        Command<int> PLAY;
        Command<int, int> COMPLAY;
        Command<int> COMPUTERGAME;
        Command PLAYER;
        Command PLAYERS;
        Command ROLL;
        Command<int> FORCEROLL;
        Command ROLLONE;
        Command BALANCE;
        Command<string> FORCEBUY;
        Command EXIT;
        Command RULES;
        Command<string> INFO;
        Command CLOSE;
        Command DUMMYGAME;
        Command TESTGENOME;
        List<object> outCommands;
        List<object> playingCommands;
        List<object> commandList;
        public void ReadCommands()
        {
            HELP = new Command("help", "shows a list of currently usable commands", "help", () =>
            {
                for(int i = 0; i < commandList.Count; i++)
                {
                    CommandBase commandBase = commandList[i] as CommandBase;
                    string output = $"{commandBase.commandId} - {commandBase.commandDescription}";
                    Console.WriteLine(output);
                }
            });
            PLAY = new Command<int>("play", "begins a game with up to four human players", "play <player count>", (x) =>
            {
                commandList = playingCommands;
                if (x > 4 || x < 1) 
                {
                    Console.WriteLine("Must specify between 1 and 4 players");
                    return; 
                }
                Console.WriteLine("Starting a game!");
                game = new Game(x, this);
                
            });
            COMPLAY = new Command<int, int>("complay", "begins a game with up to four human or computer players", "complay <player count> <computer count>", (h, c) =>
            {
                commandList = playingCommands;
                if (h + c > 4 || h + c < 1)
                {
                    Console.WriteLine("Must specify between 1 and 4 players");
                    return;
                }
                Console.WriteLine("Starting a game!");
                game = new Game(h, c, this, true);
                
            });
            COMPUTERGAME = new Command<int>("computergame", "runs a single game set of up to four random computers", "computergame <computer count>", (x) =>
            {
                commandList = playingCommands;
                if (x > 4 || x < 1)
                {
                    Console.WriteLine("Must specify between 1 and 4 computers");
                    return;
                }
                game = new Game(0, x, this, false);
                
            });
            DUMMYGAME = new Command("dummygame", "plays a game with two dumb computers", "dummygame", () =>
            {
                commandList = playingCommands;
                Console.WriteLine("Starting a game!");
                game = new Game(1, 0, this, true);
                
            });
            PLAYER = new Command("player", "shows info about the current player", "player", () =>
            {
                Console.WriteLine(game.currentPlayer.GetInfo());
            });
            PLAYERS = new Command("players", "shows info about all players", "players", () =>
            {
                foreach(PlayerHandler p in game.players)
                {
                    Console.WriteLine(p.GetInfo());                
                }
            });
            ROLL = new Command("roll", "rolls one or two dice", "roll", () =>
            {
                RollData data = game.currentPlayer.Roll(game.currentPlayer.hasTrain);
                int rollNum = data.rollVal1 + data.rollVal2;
                if(data.rollVal2 != 0)
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
                game.EvaluateRoll(rollNum, data.doubles);
            });
            FORCEROLL = new Command<int>("froll", "rolls with a predetermined number", "froll <number>", (x) =>
            {
                Console.WriteLine($"Forced a {x}");
                game.EvaluateRoll(x, false);
            });
            ROLLONE = new Command("rollone", "rolls only one die", "rollone", () =>
            {
                RollData data = game.currentPlayer.Roll(false);
                int rollNum = data.rollVal1;
                Console.WriteLine($"Rolled a {rollNum}");
                game.EvaluateRoll(rollNum, false);
            });
            BALANCE = new Command("balance", "shows the coin balance of each player", "balance", () =>
            {
                game.PrintBalances();
            });
            FORCEBUY = new Command<string>("fbuy", "buy any card at no cost", "fbuy <card>", (x) =>
            {
                if (Enum.TryParse(x.ToLower(), out Card.Establishments est))
                {
                    Card newCard = new Card(est, game.currentPlayer, game);
                    game.currentPlayer.AddCard(newCard);
                    Console.WriteLine($"Force-bought {newCard}");
                }
                else
                {
                    Console.WriteLine("Please enter a valid card name");
                }
            });
            EXIT = new Command("exit", "stops the current game", "exit", () =>
            {
                EndGame(null);
            });
            CLEAR = new Command("clear", "clears the console", "clear", () =>
            {
                Console.Clear();
            });
            RULES = new Command("rules", "prints the rules of the game", "rules", () =>
            {
                Console.WriteLine("Machi Koro is a game played with 2 to 4 players, where players take turns rolling dice and collecting income\r\n" +
                    "with the goal of buying all four landmark cards to win. Players earn money through the establishments (cards) they own,\r\n" +
                    "as each card has an effect related to earning money that will activate when it's corresponding number is rolled.\r\n" +
                    "A turn consists of rolling, earning income, and buying up to one establishment.");
            });
            INFO = new Command<string>("info", "displays info about a card type", "info <type>", (x) =>
            {
                if(Enum.TryParse(x, out Card.Establishments est))
                {
                    Console.WriteLine(Card.GetEstDesc(est));
                }
                else
                {
                    Console.WriteLine($"{x} is not a valid card type");
                }
            });
            CLOSE = new Command("close", "closes the application", "close", () =>
            {
                Environment.Exit(0);
            });
            TESTGENOME = new Command("genome", "generates a sample genome", "genome", () =>
            {
                Genome g = new Genome();
                Console.WriteLine("Genome: " + g);
            });
            playingCommands = new List<object>
            {
                CLOSE,
                HELP,
                PLAYER,
                PLAYERS,
                ROLL,
                FORCEROLL,
                ROLLONE,
                FORCEBUY,
                BALANCE,
                EXIT,
                CLEAR,
                INFO
            };
            outCommands = new List<object>
            {
                CLOSE,
                HELP,
                PLAY,
                COMPLAY,
                COMPUTERGAME,
                DUMMYGAME,
                RULES,
                CLEAR,
                INFO,
                TESTGENOME

            };
            commandList = outCommands;
            while (true) //Read the console for commands
            {
                string input = Console.ReadLine();
                string[] args = input.Split(' ');
                for (int i = 0; i < commandList.Count; i++)
                {
                    CommandBase commandBase = commandList[i] as CommandBase;

                    if (args[0] == (commandBase.commandId))
                    {
                        if (commandList[i] as Command != null && args.Length == 1)
                        {
                            Console.WriteLine();
                            (commandList[i] as Command).Invoke();
                            Console.WriteLine();
                        }
                        else if(commandList[i] as Command<int> != null && args.Length == 2)
                        {
                            Console.WriteLine();
                            (commandList[i] as Command<int>).Invoke(int.Parse(args[1]));
                            Console.WriteLine();
                        }
                        else if (commandList[i] as Command<string> != null && args.Length == 2)
                        {
                            Console.WriteLine();
                            (commandList[i] as Command<string>).Invoke(args[1]);
                            Console.WriteLine();
                        }
                        else if (commandList[i] as Command<int, int> != null && args.Length == 3)
                        {
                            Console.WriteLine();
                            (commandList[i] as Command<int, int>).Invoke(int.Parse(args[1]), int.Parse(args[2]));
                            Console.WriteLine();
                        }
                    }
                    
                }
            }
        }
        public void EndGame(PlayerHandler winner)
        {
            if(winner != null)
            {
                Console.WriteLine($"{winner} has collected all four landmark cards, they win!");
            }
            Console.WriteLine("Game ended");
            game = null;
            commandList = outCommands;
        }
    }
    
}
