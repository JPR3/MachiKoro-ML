using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class ComputerBase
    {
        public PlayerHandler handler { get; private set; }
        public Card.Establishments target { get; private set; }
        public readonly Genome genome;
        public int genomeIndex { get; private set; } = 0;
        public ComputerBase(Card.Establishments target) //Dumb computer
        {
            this.target = target;
        }
        public ComputerBase(Genome g) //Smart computer
        {
            genome = g;
            target = (Card.Establishments)genome.targetList[0];
        }
        public void SetHandler(PlayerHandler p)
        {
            handler = p;
        }
        public void IncGenome()
        {
            if(genome == null) { return; }
            genomeIndex++;
            if(genomeIndex < genome.targetList.Length)
            {
                target = (Card.Establishments)genome.targetList[genomeIndex];
            }
        }
        public string GetTargetName()
        {
            return target.ToString();
        }
        public void TakeTurn()
        {
            //Roll-------------------------------------------------------------------------------
            if (handler.shouldLog)
            {
                Console.WriteLine($"{handler.name} is rolling...");
            }
            
            RollData data = handler.Roll(handler.hasTrain); //Will always roll 2 dice if possible
            int rollNum = data.rollVal1 + data.rollVal2;
            if (handler.shouldLog)
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
            handler.game.EvaluateRoll(rollNum, data.doubles);
        }
    }
}
