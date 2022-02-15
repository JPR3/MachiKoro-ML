using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class Genome
    {
        
        static readonly int[] allIDs =
        {
            0,0,0,0,0,0,        //Wheat field
            1,1,1,1,1,1,        //Ranch
            2,2,2,2,2,2,        //Bakery
            3,3,3,3,3,3,        //Cafe
            4,4,4,4,4,4,        //Convenience store
            5,5,5,5,5,5,        //Forest
            6,                  //Stadium
            7,                  //TV station
          //8,                  //Business centers are currently disabled
            9,9,9,9,9,9,        //Cheese factory
            10,10,10,10,10,10,  //Furniture factory
            11,11,11,11,11,11,  //Mine
            12,12,12,12,12,12,  //Family restaurant
            13,13,13,13,13,13,  //Apple orchard
            14,14,14,14,14,14,  //Fruit & Vegetable market
        };
        public readonly int[] targetList;
        readonly string name = "";
        public int Wins { get; private set; }
        public int MatchPoints { get; private set; }
        public Genome()
        {
            Wins = 0;
            Random rand = new Random();
            targetList = new int[25];
            List<int> availableIDs = allIDs.ToList();
            //Add landmarks at randomized indecies
            List<int> landmarkInds = new List<int>();
            do
            {
                int number = rand.Next(25);
                if (!landmarkInds.Contains(number))
                {
                    landmarkInds.Add(number);
                }
            } while (landmarkInds.Count < 4);
            for (int i = 0; i < 4; i++)
            {
                targetList[landmarkInds[i]] = 15 + i;
            }
            //Populate rest of genome
            for (int i = 0; i < targetList.Length; i++)
            {
                if (targetList[i] == 0)
                {
                    int randInd = rand.Next(availableIDs.Count);
                    targetList[i] = availableIDs[randInd];
                    availableIDs.RemoveAt(randInd);
                }
            }
            //Set name
            foreach (int num in targetList)
            {
                if (num < 10)
                {
                    name += num;
                }
                else
                {
                    //Convert to a letter: 10 = A, 11 = B, etc.
                    name += (char)(num + 55);
                }
            }
        }
        public Genome(int[] targets)
        {
            Wins = 0;
            targetList = targets;
            //Set name
            foreach (int num in targetList)
            {
                if (num < 10)
                {
                    name += num;
                }
                else
                {
                    //Convert to a letter: 10 = A, 11 = B, etc.
                    name += (char)(num + 55);
                }
            }
        }
        public int[] ToIntArray()
        {
            return targetList;
        }
        public void ChangeScore(int position)
        {
            if(position == 0)
            { 
                MatchPoints = 0;
                return; 
            }
            MatchPoints += position;
            if(position == 1)
            {
                Wins++;
            }
        }
        public override string ToString()
        {
            return name;
        }
    }
}
