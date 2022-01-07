using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class Card
    {
        public enum Establishments
        {
            Wheat_Field,
            Ranch,
            Bakery,
            Cafe,
            Convenience_Store,
            Forest,
            Stadium,
            TV_Station,
            Business_Center,
            Cheese_Factory,
            Furniture_Factory,
            Mine,
            Family_Restaurant,
            Apple_Orchard,
            Fruit_And_Vegetable_Market
        };


        readonly Establishments est;
        public readonly PlayerHandler owner;
        public int[] activationNums { get; private set; }
        int cost;
        Action<PlayerHandler> effect;

        public Card(Establishments estType, PlayerHandler owner)
        {
            this.owner = owner;
            est = estType;
            SetValues(est); //Sets the activationNums, cost and effect based on establishment type
        }

        public void Invoke(PlayerHandler caller)
        {
            effect.Invoke(caller);
        }

        void SetValues(Establishments type)
        {
            switch (type)
            {
                case Establishments.Wheat_Field:
                    activationNums = new int[1] { 1 };
                    cost = 1;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.Ranch:
                    activationNums = new int[1] { 2 };
                    cost = 1;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.Bakery:
                    activationNums = new int[2] { 2, 3 };
                    cost = 1;
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            owner.ChangeCoins(1);
                        }
                    };
                    break;
                case Establishments.Cafe:
                    activationNums = new int[1] { 3 };
                    cost = 2;
                    effect = (caller) =>
                    {
                        if(caller != owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            caller.ChangeCoins(-1);
                            owner.ChangeCoins(1);
                        }
                    };
                    break;
                case Establishments.Convenience_Store:
                    activationNums = new int[1] { 4 };
                    cost = 2;
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            owner.ChangeCoins(3);
                        }
                    };
                    break;
                case Establishments.Forest:
                    activationNums = new int[1] { 5 };
                    cost = 3;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.Stadium:
                    activationNums = new int[1] { 6 };
                    break;
                case Establishments.TV_Station:
                    activationNums = new int[1] { 6 };
                    break;
                case Establishments.Business_Center:
                    activationNums = new int[1] { 6 };
                    break;
                case Establishments.Cheese_Factory:
                    activationNums = new int[1] { 7 };
                    break;
                case Establishments.Furniture_Factory:
                    activationNums = new int[1] { 8 };
                    break;
                case Establishments.Mine:
                    activationNums = new int[1] { 9 };
                    break;
                case Establishments.Family_Restaurant:
                    activationNums = new int[2] { 9, 10 };
                    break;
                case Establishments.Apple_Orchard:
                    activationNums = new int[1] { 10 };
                    break;
                case Establishments.Fruit_And_Vegetable_Market:
                    activationNums = new int[2] { 11, 12 };
                    break;
            }
        }

        override public string ToString()
        {
            return est.ToString();
        }
    }
}
