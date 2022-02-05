using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class Genome
    {
        
        static int[] allIDs =
        {
            0,0,0,0,0,0,        //Wheat field
            1,1,1,1,1,1,        //Ranch
            2,2,2,2,2,2,        //Bakery
            3,3,3,3,3,3,        //Cafe
            4,4,4,4,4,4,        //Convenience store
            5,5,5,5,5,5,        //Forest
            6,                  //Stadium
            7,                  //TV station
            8,                  //Business center
            9,9,9,9,9,9,        //Cheese factory
            10,10,10,10,10,10,  //Furniture factory
            11,11,11,11,11,11,  //Mine
            12,12,12,12,12,12,  //Family restaurant
            13,13,13,13,13,13,  //Apple orchard
            14,14,14,14,14,14,  //Fruit & Vegetable market
        };
        int[] targetList;
        string name = "";
        public Genome()
        {

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
        public override string ToString()
        {
            return name;
        }
    }
}
