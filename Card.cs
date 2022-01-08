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

        public static string GetPurchasableEstablishments(int coins)
        {
            string str = "";
            //One costs
            str += $"{GetEstDesc(Establishments.wheat_field)}\r\n{GetEstDesc(Establishments.ranch)}\r\n{GetEstDesc(Establishments.bakery)}\r\n";
            if(coins < 2) { return str; }
            //Two costs
            str += $"{GetEstDesc(Establishments.fruit_and_vegetable_market)}\r\n";
            if(coins < 3) { return str; }
            //Three costs
            str += $"{GetEstDesc(Establishments.cafe)}\r\n{GetEstDesc(Establishments.convenience_store)}\r\n{GetEstDesc(Establishments.forest)}" +
                $"\r\n{GetEstDesc(Establishments.furniture_factory)}\r\n{GetEstDesc(Establishments.family_restaurant)}\r\n" +
                $"{GetEstDesc(Establishments.apple_orchard)}\r\n";
            if(coins < 5) { return str; }
            //Five costs
            str += $"{GetEstDesc(Establishments.cheese_factory)}\r\n";
            if(coins < 6) { return str; }
            //Six costs
            str += $"{GetEstDesc(Establishments.stadium)}\r\n{GetEstDesc(Establishments.mine)}\r\n";
            if(coins < 7) { return str; }
            //Seven costs
            str += $"{GetEstDesc(Establishments.tv_station)}\r\n";
            if(coins < 8) { return str; }
            //Eight costs
            str += $"{GetEstDesc(Establishments.business_center)}\r\n";
            return str; 
        }
        static string GetEstDesc(Establishments est)
        {
            switch(est)
            {
                case Establishments.wheat_field:
                    return "Wheat field: 1 coin - activates on 1\r\n\tGet 1 coin from the bank, on anyone's turn";
                case Establishments.ranch:
                    return "Ranch: 1 coin - activates on 2\r\n\tGet 1 coin from the bank, on anyone's turn";
                case Establishments.bakery:
                    return "Bakery: 1 coin - activates on 2-3\r\n\tGet 1 coin from the bank, on your turn only";
                case Establishments.cafe:
                    return "Cafe: 2 coins - activates on 3\r\n\tGet 1 coin from the player who rolled the dice";
                case Establishments.convenience_store:
                    return "Convenience store: 2 coins - activates on 4\r\n\tGet 3 coins from the bank, on your turn only";
                case Establishments.forest:
                    return "Forest: 3 coins - activates on 5\r\n\tGet 1 coin from the bank, on anyone's turn";
                case Establishments.stadium:
                    return "Stadium: 6 coins - activates on 6\r\n\tGet 2 coins from all players, on your turn only";
                case Establishments.tv_station:
                    return "TV station: 7 coins - activates on 6\r\n\tTake 5 coins from any one player, on your turn only";
                case Establishments.business_center:
                    return "Business center: 8 coins - activates on 6\r\n\tTrade one establishment with another player, on your turn only";
                case Establishments.cheese_factory:
                    return "Cheese factory: 5 coins - activates on 7\r\n\tGet three coins from the bank for each ranch you own, on your turn only";
                case Establishments.furniture_factory:
                    return "Furniture factory: 3 coins - activates on 8\r\n\tGet 3 coins from the bank for each forest or mine you own, on your turn only";
                case Establishments.mine:
                    return "Mine: 6 coins - activates on 9\r\n\tGet 5 coins from the bank, on anyone's turn";
                case Establishments.family_restaurant:
                    return "Family resturant: 3 coins - activates on 9-10\r\n\tGet 2 coins from the player who rolled the dice";
                case Establishments.apple_orchard:
                    return "Apple orchard: 3 coins - activates on 10\r\n\tGet 3 coins from the bank, on anyone's turn";
                case Establishments.fruit_and_vegetable_market:
                    return "Fruit and vegetable market: 2 coins - activates on 11-12\r\n\tGet 2 coins from the bank for each wheat field or apple orchard you own, on your turn only";
                    
            }

            return "";
        }
        override public string ToString()
        {
            return est.ToString();
        }
    }
}
