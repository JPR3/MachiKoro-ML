using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class ComputerBase
    {
        public PlayerHandler Handler { get; private set; }
        public Card.Establishments Target { get; private set; }
        public readonly Genome genome;
        public int GenomeIndex { get; private set; } = 0;
        public ComputerBase(Card.Establishments target) //Dumb computer
        {
            this.Target = target;
        }
        public ComputerBase(Genome g) //Smart computer
        {
            genome = g;
            Target = (Card.Establishments)genome.targetList[0];
        }
        public void SetHandler(PlayerHandler p)
        {
            Handler = p;
        }
        public void IncGenome()
        {
            if(genome == null) { return; }
            GenomeIndex++;
            if(GenomeIndex < genome.targetList.Length)
            {
                Target = (Card.Establishments)genome.targetList[GenomeIndex];
            }
        }
        public string GetTargetName()
        {
            return Target.ToString();
        }
        public void TakeTurn()
        {
            //Roll-------------------------------------------------------------------------------
            if (Handler.shouldLog)
            {
                Console.WriteLine($"{Handler.name} is rolling...");
            }
            
            RollData data = Handler.Roll(Handler.HasTrain); //Will always roll 2 dice if possible
            int rollNum = data.rollVal1 + data.rollVal2;
            if (Handler.shouldLog)
            {
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
            }
            Handler.game.EvaluateRoll(rollNum, data.doubles);
        }
    }
}
