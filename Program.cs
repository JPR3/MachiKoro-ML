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
    class Program
    {
        Command HELP;
        List<object> commandList;

        public void ReadCommands()
        {
            HELP = new Command("help", "shows a list of commands", "help", () =>
            {
                for(int i = 0; i < commandList.Count; i++)
                {
                    CommandBase commandBase = commandList[i] as CommandBase;
                    string output = $"{commandBase.commandId} - {commandBase.commandDescription}";
                    Console.WriteLine(output);
                }
            });

            commandList = new List<object>
            {
                HELP

            };
            //Change to only while it should be accepting input
            while (true)
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
                    }
                }
            }
        }

    }
    
}
