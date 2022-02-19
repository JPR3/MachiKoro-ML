using System;
using System.Collections.Generic;

namespace MachiKoro_ML
{
    static class Extensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    class Initialize
    {
        public static void Main()
        {
            Console.WriteLine("Welcome to Machi Koro!\r\nType 'help' for a list of commands");
            Program prog = new Program();
            prog.ReadCommands();
        }
    }
    public class Program //Rename at some point
    {
        readonly Random random = new Random();
        Game game;
        Command HELP;
        Command<string> SYNTAX;
        Command CLEAR;
        Command<int> PLAY;
        Command<int, int> COMPLAY;
        Command<int> COMPUTERGAME;
        Command TESTMATCH;
        Command TESTGEN;
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
        List<PlayerHandler[]> matchResults = new List<PlayerHandler[]>();
        List<Genome[]> genResults = new List<Genome[]>();
        static int genNumber = 0;
        bool allowCommands = true;
        public void SetCommands(bool allow)
        {
            allowCommands = allow;
        }
        public void ReadCommands()
        {
            HELP = new Command("help", "shows a list of currently usable commands", "help", () =>
            {
                for (int i = 0; i < commandList.Count; i++)
                {
                    CommandBase commandBase = commandList[i] as CommandBase;
                    string output = $"{commandBase.commandName} - {commandBase.commandDescription}";
                    Console.WriteLine(output);
                }
            });
            SYNTAX = new Command<string>("help <command>", "help", "shows the proper syntax for a command", "help <command>", (x) =>
            {
                for (int i = 0; i < commandList.Count; i++)
                {
                    CommandBase commandBase = commandList[i] as CommandBase;
                    if (commandBase.commandId.Equals(x))
                    {
                        Console.WriteLine($"{commandBase.commandDescription} - '{commandBase.commandSyntax}'");
                    }
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
                game.HandleTurns();

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
                game = new Game(h, c, this, true, false);
                game.HandleTurns();

            });
            COMPUTERGAME = new Command<int>("computergame", "runs a single game set of up to four random computers", "computergame <computer count>", (x) =>
            {
                commandList = playingCommands;
                if (x > 4 || x < 1)
                {
                    Console.WriteLine("Must specify between 1 and 4 computers");
                    return;
                }
                game = new Game(0, x, this, false, false);
                game.HandleTurns();

            });
            TESTMATCH = new Command("match", "runs a single match of four random computers", "match", () =>
            {
                commandList = playingCommands;
                Genome[] genomes = { new Genome(), new Genome(), new Genome(), new Genome() };
                RunMatch(genomes, false);
                matchResults.Clear();
                commandList = outCommands;
            });
            TESTGEN = new Command("generation", "runs a single generation", "generation", () =>
            {
                commandList = playingCommands;
                Genome[][] genomes = new Genome[10][];
                for (int i = 0; i < genomes.Length; i++)
                {
                    genomes[i] = new Genome[4];
                    for (int j = 0; j < 4; j++)
                    {
                        genomes[i][j] = new Genome();
                    }
                }
                for (int i = 0; i < 50; i++)
                {
                    genomes = RunGen(genomes, 50);
                }

                commandList = outCommands;

            });
            DUMMYGAME = new Command("dummygame", "plays a game with two dumb computers", "dummygame", () =>
            {
                commandList = playingCommands;
                Console.WriteLine("Starting a game!");
                game = new Game(1, 0, this, true, false);
                game.HandleTurns();

            });
            PLAYER = new Command("player", "shows info about the current player", "player", () =>
            {
                Console.WriteLine(game.CurrentPlayer.GetInfo());
            });
            PLAYERS = new Command("players", "shows info about all players", "players", () =>
            {
                foreach (PlayerHandler p in game.players)
                {
                    Console.WriteLine(p.GetInfo());
                }
            });
            ROLL = new Command("roll", "rolls one or two dice", "roll", () =>
            {
                RollData data = game.CurrentPlayer.Roll(game.CurrentPlayer.HasTrain);
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
                game.EvaluateRoll(rollNum, data.doubles);
            });
            FORCEROLL = new Command<int>("froll", "rolls with a predetermined number", "froll <number>", (x) =>
            {
                Console.WriteLine($"Forced a {x}");
                game.EvaluateRoll(x, false);
            });
            ROLLONE = new Command("rollone", "rolls only one die", "rollone", () =>
            {
                RollData data = game.CurrentPlayer.Roll(false);
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
                    Card newCard = new Card(est, game.CurrentPlayer, game);
                    game.CurrentPlayer.AddCard(newCard);
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
                if (Enum.TryParse(x, out Card.Establishments est))
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
                SYNTAX,
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
                SYNTAX,
                PLAY,
                COMPLAY,
                COMPUTERGAME,
                TESTMATCH,
                TESTGEN,
                DUMMYGAME,
                RULES,
                CLEAR,
                INFO,
                TESTGENOME

            };
            commandList = outCommands;
            while (true) //Read the console for commands
            {
                if(!allowCommands) { continue; }
                string input = Console.ReadLine();
                InterpretCommand(input);
            }
        }
        public void InterpretCommand(string cmd)
        {
            string[] args = cmd.Split(' ');
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
                    else if (commandList[i] as Command<int> != null && args.Length == 2)
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
        public void RunMatch(Genome[] genomes, bool isGen)
        {
            for (int i = 0; i < genomes.Length; i++)
            {
                //Run three games, then rotate order
                for (int ii = 0; ii < 3; ii++)
                {
                    game = new Game(genomes, this, false, true);
                }
                Genome[] temp = new Genome[genomes.Length];
                for (int j = 0; j < genomes.Length - 1; j++)
                {
                    temp[j + 1] = genomes[j];
                }
                temp[0] = genomes[genomes.Length - 1];
                genomes = temp;
            }
            Genome[] orderedGenomes = new Genome[genomes.Length];
            Array.Copy(genomes, orderedGenomes, genomes.Length);
            Genome tempGenome;
            for (int i = 0; i < orderedGenomes.Length; i++)
            {
                for (int j = i + 1; j < orderedGenomes.Length; j++)
                {
                    if (orderedGenomes[i].MatchPoints > orderedGenomes[j].MatchPoints)
                    {
                        tempGenome = orderedGenomes[i];
                        orderedGenomes[i] = orderedGenomes[j];
                        orderedGenomes[j] = tempGenome;
                    }
                }
            }
            if (!isGen)
            {

                for (int i = 0; i < matchResults.Count; i++)
                {
                    Console.WriteLine($"\r\nGame {i + 1}");
                    foreach (PlayerHandler p in matchResults[i])
                    {
                        Console.Write($"\r\n{p.parentComputer.genome}, with {p.NumLandmarks} landmarks and {p.NumCoins} coins");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"\r\nGenome {orderedGenomes[0]} is the winner!\r\nFull results:\r\n");
            }
            foreach (Genome g in orderedGenomes)
            {
                if (!isGen)
                {
                    Console.WriteLine($"{g}, with {g.MatchPoints}");
                    g.ChangeScore(0); //Resets the genome's match score
                }

            }
            matchResults.Clear();
            if (isGen)
            {
                genResults.Add(orderedGenomes);
            }

        }
        public Genome[][] RunGen(Genome[][] genomes, int targetNumber)
        {
            Console.WriteLine($"Starting generation {genNumber}");
            List<Genome> nextGen = new List<Genome>();
            List<Genome> survivorPool = new List<Genome>();
            for (int i = 0; i < genomes.Length; i++)
            {
                RunMatch(genomes[i], true);
            }
            int playersPerGame = genResults[0].Length;
            Console.WriteLine("Generation over!");
            Console.WriteLine("These genomes will move on and breed (less than 24 points):\r\n");
            for (int i = 0; i < genResults.Count; i++)
            {
                for (int j = 0; j < playersPerGame; j++)
                {
                    Genome currentGenome = genResults[i][j];
                    if (currentGenome.MatchPoints > 30 && j != 0) { continue; }
                    Console.WriteLine($"{currentGenome} with {currentGenome.MatchPoints} points - " +
                        $"{currentGenome.Wins} lifetime wins ({Math.Round(currentGenome.Wins / (playersPerGame * 3.0) * 100, 2)}%)");
                    nextGen.Add(currentGenome);
                    survivorPool.Add(currentGenome);
                    currentGenome.ChangeScore(0);
                }
                Console.WriteLine(new string('-', 40));
            }
            //Breed the genomes!
            List<int> sectionLengths = new List<int>();
            int totalNum = 0;
            while (totalNum < 25)
            {
                int nextLength = random.Next(1, 4);
                if (totalNum + nextLength >= 25)
                {
                    sectionLengths.Add(24 - totalNum);
                    break;
                }
                sectionLengths.Add(nextLength);
                totalNum += nextLength;
            }
            survivorPool.Shuffle();
            while (survivorPool.Count > 1 && nextGen.Count < playersPerGame * 10)
            {
                int[] child1;
                int[] child2;
                int rand1 = random.Next(survivorPool.Count);
                child1 = survivorPool[rand1].ToIntArray();
                survivorPool.RemoveAt(rand1);
                if (survivorPool.Count == 0)
                {
                    break;
                }
                int rand2 = random.Next(survivorPool.Count);
                child2 = survivorPool[rand2].ToIntArray();
                survivorPool.RemoveAt(rand2);

                int globalIndex = 0;
                for (int i = 0; i < sectionLengths.Count; i++)
                {
                    for (int j = 0; j < sectionLengths[i]; j++)
                    {
                        if (i % 2 != 0)
                        {
                            child1[globalIndex] = child2[globalIndex];
                            child2[globalIndex] = child1[globalIndex];
                        }
                        nextGen.Add(new Genome(child1));
                        if (nextGen.Count == playersPerGame * 10) { break; }
                        nextGen.Add(new Genome(child2));
                        globalIndex++;
                    }
                }
            }
            while (nextGen.Count < playersPerGame * 10)
            {
                nextGen.Add(new Genome());
            }
            genResults.Clear();
            nextGen.Shuffle();
            genNumber++;
            Genome[][] nextSetup = new Genome[10][];
            for (int i = 0; i < 10; i++)
            {
                Genome[] table = new Genome[playersPerGame];
                for (int j = 0; j < playersPerGame; j++)
                {
                    table[j] = nextGen[0];
                    nextGen.RemoveAt(0);
                }
                nextSetup[i] = table;
            }
            return nextSetup;
            /*Query for continuation
            Console.WriteLine("Generation has finished, type 'C' to continue");
            string str = Console.ReadLine();
            if(str.ToLower().Equals("c"))
            {
                Genome[][] nextSetup = new Genome[10][];
                for(int i = 0; i < 10; i++)
                {
                    Genome[] table = new Genome[playersPerGame];
                    for(int j = 0; j < playersPerGame; j++)
                    {
                        table[j] = nextGen[0];
                        nextGen.RemoveAt(0);
                    }
                    nextSetup[i] = table;
                }
                RunGen(nextSetup);
            }
            */

        }
        public void AddMatchResults(PlayerHandler[] orderedResults)
        {
            for (int i = 0; i < orderedResults.Length; i++)
            {
                orderedResults[i].parentComputer.genome.ChangeScore(i + 1);
            }
            game = null;
            matchResults.Add(orderedResults);
        }
        public void EndGame(PlayerHandler[] orderedResults)
        {
            if (orderedResults != null)
            {
                Console.WriteLine($"{orderedResults[0]} has collected all four landmark cards, they win!\r\nFull results:");
                foreach (PlayerHandler p in orderedResults)
                {
                    Console.Write($"\r\n{p}, with {p.NumLandmarks} landmarks and {p.NumCoins} coins");
                    if (p.parentComputer != null)
                    {
                        Console.Write($" - {p.parentComputer.genome}");
                    }
                }

            }
            Console.WriteLine("\r\nGame ended");
            game = null;
            commandList = outCommands;
        }
    }

}
