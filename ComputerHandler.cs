﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class DumbComputer
    {
        public PlayerHandler handler { get; private set; }
        public Card.Establishments target { get; private set; }
        public DumbComputer(Card.Establishments target)
        {
            this.target = target;
        }
        public void SetHandler(PlayerHandler p)
        {
            handler = p;
        }
        public void TakeTurn()
        {
            //Roll-------------------------------------------------------------------------------
            Console.WriteLine($"{handler.name} is rolling...");
            RollData data = handler.Roll(handler.hasTrain); //Will always roll 2 dice if possible
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
            handler.game.EvaluateRoll(rollNum, data.doubles);
        }
    }
}