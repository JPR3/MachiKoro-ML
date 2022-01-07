using System;
using System.Collections.Generic;

namespace MachiKoro_ML
{
    class Initialize
    {
        /*
         *  TODO:
         *  Add balance command
         *  Check function of cafe
         *  Implement remaining cards
         *  Win condition
         *  Improve card buying process
         */
        public static void Main(String[] args)
        {
            Console.WriteLine("Welcome to Machi Koro!\r\nType 'help' for a list of commands");
            Program prog = new Program();
            prog.ReadCommands();
        }
    }
    class Program //Rename at some point
    {
        Game game;
        Command HELP;
        Command<int> PLAY;
        Command PLAYER;
        Command ROLL;
        Command<int> FORCEROLL;
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
                if(x > 4 || x < 1) 
                {
                    Console.WriteLine("Must specify between 1 and 4 players");
                    return; 
                }
                Console.WriteLine("Starting a game!");
                game = new Game(x);
                commandList = playingCommands;
            });
            PLAYER = new Command("player", "shows info about the current player", "player", () =>
            {
                Console.WriteLine(game.currentPlayer.GetInfo());
            });
            ROLL = new Command("roll", "rolls one or two dice", "roll", () =>
            {
                int rollNum = game.currentPlayer.Roll();
                Console.WriteLine($"Rolled a {rollNum}\r\n");
                game.EvaluateRoll(rollNum);
            });
            FORCEROLL = new Command<int>("froll", "rolls with a predetermined number", "froll <number>", (x) =>
            {
                Console.WriteLine($"Forced a {x}\r\n");
                game.EvaluateRoll(x);
            });

            playingCommands = new List<object>
            {
                HELP,
                PLAYER,
                ROLL,
                FORCEROLL
            };
            outCommands = new List<object>
            {
                HELP,
                PLAY

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
                    }
                    
                }
            }
        }

    }
    
}
