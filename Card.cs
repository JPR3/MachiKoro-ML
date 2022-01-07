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
            wheat_field,
            ranch,
            bakery,
            cafe,
            convenience_store,
            forest,
            stadium,
            tv_station,
            business_center,
            cheese_factory,
            furniture_factory,
            mine,
            family_restaurant,
            apple_orchard,
            fruit_and_vegetable_market
        };


        readonly Establishments est;
        public readonly PlayerHandler owner;
        public int[] activationNums { get; private set; }
        public int cost { get; private set; }
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
                case Establishments.wheat_field:
                    activationNums = new int[1] { 1 };
                    cost = 1;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.ranch:
                    activationNums = new int[1] { 2 };
                    cost = 1;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.bakery:
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
                case Establishments.cafe:
                    activationNums = new int[1] { 3 };
                    cost = 2;
                    effect = (caller) =>
                    {
                        if(caller != owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            if(caller.ChangeCoins(-1))
                            {
                                owner.ChangeCoins(1);
                            }
                            else
                            {
                                Console.WriteLine("...but there was nothing to steal!");
                            }
                        }
                    };
                    break;
                case Establishments.convenience_store:
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
                case Establishments.forest:
                    activationNums = new int[1] { 5 };
                    cost = 3;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.stadium:
                    activationNums = new int[1] { 6 };
                    break;
                case Establishments.tv_station:
                    activationNums = new int[1] { 6 };
                    break;
                case Establishments.business_center:
                    activationNums = new int[1] { 6 };
                    break;
                case Establishments.cheese_factory:
                    activationNums = new int[1] { 7 };
                    break;
                case Establishments.furniture_factory:
                    activationNums = new int[1] { 8 };
                    break;
                case Establishments.mine:
                    activationNums = new int[1] { 9 };
                    break;
                case Establishments.family_restaurant:
                    activationNums = new int[2] { 9, 10 };
                    break;
                case Establishments.apple_orchard:
                    activationNums = new int[1] { 10 };
                    break;
                case Establishments.fruit_and_vegetable_market:
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
