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
            fruit_and_vegetable_market,
            train_station,
            shopping_mall,
            amusement_park,
            radio_tower
        };


        readonly Establishments est;
        public bool isTradable { get; private set; }
        PlayerHandler owner;
        static readonly string[] descriptions =
        {
            "Wheat_field: 1 coin - activates on 1\r\n\tGet 1 coin from the bank, on anyone's turn",
            "Ranch: 1 coin - activates on 2\r\n\tGet 1 coin from the bank, on anyone's turn",
            "Bakery: 1 coin - activates on 2-3\r\n\tGet 1 coin from the bank, on your turn only",
            "Cafe: 2 coins - activates on 3\r\n\tGet 1 coin from the player who rolled the dice",
            "Convenience_store: 2 coins - activates on 4\r\n\tGet 3 coins from the bank, on your turn only",
            "Forest: 3 coins - activates on 5\r\n\tGet 1 coin from the bank, on anyone's turn",
            "Stadium: 6 coins - activates on 6\r\n\tGet 2 coins from all players, on your turn only",
            "TV_station: 7 coins - activates on 6\r\n\tTake 5 coins from any one player, on your turn only",
            "Business_center: 8 coins - activates on 6\r\n\tTrade one establishment with another player, on your turn only",
            "Cheese_factory: 5 coins - activates on 7\r\n\tGet three coins from the bank for each ranch you own, on your turn only",
            "Furniture_factory: 3 coins - activates on 8\r\n\tGet 3 coins from the bank for each forest or mine you own, on your turn only",
            "Mine: 6 coins - activates on 9\r\n\tGet 5 coins from the bank, on anyone's turn",
            "Family_resturant: 3 coins - activates on 9-10\r\n\tGet 2 coins from the player who rolled the dice",
            "Apple_orchard: 3 coins - activates on 10\r\n\tGet 3 coins from the bank, on anyone's turn",
            "Fruit_and_vegetable_market: 2 coins - activates on 11-12\r\n\tGet 2 coins from the bank for each wheat field or apple orchard you own, on your turn only",
            "Train_station: 4 coins - LANDMARK CARD\r\n\tYou may roll 1 or 2 dice",
            "Shopping_mall: 10 coins - LANDMARK CARD\r\n\tYour Bakeries, Cafes, Convenience stores, and Family restaurants earn/steal an extra coin",
            "Amusement_park: 16 coins - LANDMARK CARD\r\n\tIf you roll doubles, take another turn",
            "Radio_tower: 22 coins - LANDMARK CARD\r\n\tOnce per turn, you can choose to reroll your dice"
        };
        public int[] activationNums { get; private set; }
        public int cost { get; private set; }
        Action<PlayerHandler> effect;
        Game game;

        public Card(Establishments estType, PlayerHandler owner, Game game)
        {
            this.game = game;
            this.owner = owner;
            est = estType;
            SetValues(est); //Sets the activationNums, cost and effect based on establishment type
        }

        public void Invoke(PlayerHandler caller)
        {
            effect.Invoke(caller);
        }
        public void SetNewOwner(PlayerHandler newOwner)
        {
            owner = newOwner;
        }

        void SetValues(Establishments type)
        {
            switch (type)
            {
                case Establishments.wheat_field:
                    activationNums = new int[1] { 1 };
                    cost = 1;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                        owner.numAgriculture++;
                    };
                    break;
                case Establishments.ranch:
                    activationNums = new int[1] { 2 };
                    cost = 1;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                        owner.numRanches++;
                    };
                    break;
                case Establishments.bakery:
                    activationNums = new int[2] { 2, 3 };
                    cost = 1;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            int change = 1;
                            if (owner.hasMall) { change = 2; }
                            owner.ChangeCoins(change);
                        }
                    };
                    break;
                case Establishments.cafe:
                    activationNums = new int[1] { 3 };
                    cost = 2;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if(caller != owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            int targetSteal = 1; //Set to 2 if has mall
                            if(owner.hasMall) { targetSteal = 2; }
                            int stealNum = caller.GetMaxSteal(targetSteal);
                            if(stealNum != 0)
                            {
                                caller.ChangeCoins(-stealNum);
                                owner.ChangeCoins(stealNum);
                                Console.WriteLine($"Stole {stealNum} from {caller}");
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
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            int change = 3;
                            if (owner.hasMall) { change = 4; }
                            owner.ChangeCoins(change);
                        }
                    };
                    break;
                case Establishments.forest:
                    activationNums = new int[1] { 5 };
                    cost = 3;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(1);
                        owner.numNature++;
                    };
                    break;
                case Establishments.stadium:
                    activationNums = new int[1] { 6 };
                    cost = 6;
                    isTradable = false;
                    owner.AddStadium();
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            foreach (PlayerHandler player in game.players)
                            {
                                if(player == owner) { continue; }
                                int stealNum = player.GetMaxSteal(2);
                                if (stealNum != 0)
                                {
                                    player.ChangeCoins(-stealNum);
                                    owner.ChangeCoins(stealNum);
                                    Console.WriteLine($"Stole {stealNum} from {player}");
                                }
                                else
                                {
                                    Console.WriteLine("...but there was nothing to steal!");
                                }
                            }
                        }
                    };
                    break;
                case Establishments.tv_station:
                    activationNums = new int[1] { 6 };
                    cost = 7;
                    isTradable = false;
                    owner.AddStation();
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            //Prompt to pick another player
                            Console.WriteLine($"{owner}, pick another player to steal up to 5 coins from");
                            game.PrintBalances();
                            PlayerHandler target = null;
                            while(target == null)
                            {
                                string selection = Console.ReadLine();
                                foreach (PlayerHandler player in game.players)
                                {
                                    if(selection.ToLower().Equals(player.ToString().ToLower()))
                                    {
                                        target = player;
                                    }
                                }
                            }
                            //Steal up to 5 coins from target
                            int stealNum = target.GetMaxSteal(5);
                            target.ChangeCoins(-stealNum);
                            owner.ChangeCoins(stealNum);
                            Console.WriteLine($"\r\nStole {stealNum} from {target}");


                        }
                    };
                    break;
                case Establishments.business_center:
                    activationNums = new int[1] { 6 };
                    cost = 8;
                    isTradable = false;
                    owner.AddCenter();
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            //Prompt to pick a player
                            Console.WriteLine($"{owner}, pick another player to trade a card with");
                            foreach(PlayerHandler player in game.players)
                            {
                                if(player != owner)
                                {
                                    Console.WriteLine($"\r\n{player}");
                                    foreach (Card c in player.GetCardsAsArray())
                                    {
                                        if(c.isTradable)
                                        {
                                            Console.WriteLine($"\t{c}");
                                        }
                                        
                                    }

                                }
                            }
                            PlayerHandler targetPlayer = null;
                            while (targetPlayer == null)
                            {
                                string selection = Console.ReadLine();
                                foreach (PlayerHandler player in game.players)
                                {
                                    if (selection.ToLower().Equals(player.ToString().ToLower()))
                                    {
                                        targetPlayer = player;
                                    }
                                }
                            }
                            //Prompt to pick a card from player
                            Console.WriteLine("Pick a card to take");
                            Card targetCard = null;
                            while (targetCard == null)
                            {
                                string selection = Console.ReadLine();
                                Card[] cards = targetPlayer.GetCardsAsArray();
                                foreach (Card card in cards)
                                {
                                    if (card.isTradable && selection.ToLower().Equals(card.ToString()))
                                    {
                                        targetCard = card;
                                    }
                                }
                            }
                            //Prompt to pick a card from self
                            Console.WriteLine("Pick a card to give away");
                            foreach (Card c in owner.GetCardsAsArray())
                            {
                                if(c.isTradable)
                                {
                                    Console.WriteLine($"\t{c}");
                                }
                            }
                            Card ownerCard = null;
                            while(ownerCard == null)
                            {
                                string selection = Console.ReadLine();
                                Card[] cards = owner.GetCardsAsArray();
                                foreach (Card card in cards)
                                {
                                    if (card.isTradable && selection.ToLower().Equals(card.ToString()))
                                    {
                                        ownerCard = card;
                                    }
                                }
                            }
                            //Make the trade
                            Console.WriteLine($"Traded {owner}'s {ownerCard} for {targetPlayer}'s {targetCard}");
                            owner.ReplaceCard(ownerCard, targetCard);
                            targetPlayer.ReplaceCard(targetCard, ownerCard);

                        }
                    };
                    break;
                case Establishments.cheese_factory:
                    activationNums = new int[1] { 7 };
                    cost = 5;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            owner.ChangeCoins(3 * owner.numRanches);
                        }
                    };
                    break;
                case Establishments.furniture_factory:
                    activationNums = new int[1] { 8 };
                    cost = 3;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            owner.ChangeCoins(3 * owner.numNature);
                        }
                    };
                    break;
                case Establishments.mine:
                    activationNums = new int[1] { 9 };
                    cost = 6;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(5);
                    };
                    break;
                case Establishments.family_restaurant:
                    activationNums = new int[2] { 9, 10 };
                    cost = 3;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if (caller != owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            int targetSteal = 2; //Set to 3 if has mall
                            if (owner.hasMall) { targetSteal = 3; }
                            int stealNum = caller.GetMaxSteal(targetSteal);
                            if (stealNum != 0)
                            {
                                caller.ChangeCoins(-stealNum);
                                owner.ChangeCoins(stealNum);
                                Console.WriteLine($"Stole {stealNum} from {caller}");
                            }
                            else
                            {
                                Console.WriteLine("...but there was nothing to steal!");
                            }
                        }
                    };
                    break;
                case Establishments.apple_orchard:
                    activationNums = new int[1] { 10 };
                    cost = 3;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        Console.WriteLine($"{owner}'s {this} activated!");
                        owner.ChangeCoins(3);
                    };
                    break;
                case Establishments.fruit_and_vegetable_market:
                    activationNums = new int[2] { 11, 12 };
                    cost = 2;
                    isTradable = true;
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                            owner.ChangeCoins(2 * owner.numAgriculture);
                        }
                    };
                    break;
                case Establishments.train_station:
                    activationNums = null;
                    cost = 4;
                    isTradable = false;
                    effect = null;
                    //Change owner's values to represent abilities of new card
                    owner.AddTrain();
                    break;
                case Establishments.shopping_mall:
                    activationNums = null;
                    cost = 10;
                    isTradable = false;
                    effect = null;
                    //Change owner values
                    owner.AddMall();
                    break;
                case Establishments.amusement_park:
                    activationNums = null;
                    cost = 16;
                    isTradable = false;
                    effect = null;
                    //Change owner values
                    owner.AddPark();
                    break;
                case Establishments.radio_tower:
                    activationNums = null;
                    cost = 22;
                    isTradable = false;
                    effect = null;
                    //Change owner values
                    owner.AddRadio();
                    break;
            }
        }

        public static string GetPurchasableEstablishments(PlayerHandler player)
        {
            int coins = player.numCoins;
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
            if(coins < 4) { return str; }
            //Four costs
            if(!player.hasTrain)
            {
                str += $"{GetEstDesc(Establishments.train_station)}\r\n";
            }
            if(coins < 5) { return str; }
            //Five costs
            str += $"{GetEstDesc(Establishments.cheese_factory)}\r\n";
            if(coins < 6) { return str; }
            //Six costs
            if(!player.hasStadium)
            {
                str += $"{GetEstDesc(Establishments.stadium)}\r\n{GetEstDesc(Establishments.mine)}\r\n";
            }
            if(coins < 7) { return str; }
            //Seven costs
            if(!player.hasStation)
            {
                str += $"{GetEstDesc(Establishments.tv_station)}\r\n";
            }
            if(coins < 8) { return str; }
            //Eight costs
            if(!player.hasCenter)
            {
                str += $"{GetEstDesc(Establishments.business_center)}\r\n";
            }
            if(coins < 10) { return str; }
            //Ten costs
            if(!player.hasMall)
            {
                str += $"{GetEstDesc(Establishments.shopping_mall)}\r\n";
            }
            if(coins < 16) { return str; }
            //Sixteen costs
            if (!player.hasPark)
            {
                str += $"{GetEstDesc(Establishments.amusement_park)}\r\n";
            }
            if(coins < 22) { return str; }
            //Twenty-two costs
            if (!player.hasRadio)
            {
                str += $"{GetEstDesc(Establishments.radio_tower)}\r\n";
            }
            return str;
        }
        public static string GetEstDesc(Establishments est)
        {
            return descriptions[(int)est];
        }
        override public string ToString()
        {
            return est.ToString();
        }
    }
}
