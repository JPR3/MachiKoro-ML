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
        public enum Symbols
        {
            agriculture,
            ranch,
            nature,
            restaurant,
            store,
            factory,
            major,
            landmark
        }

        readonly Establishments est;
        public Symbols Symbol { get; private set; }
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
        public int[] ActivationNums { get; private set; }
        public int Cost { get; private set; }
        Action<PlayerHandler> effect;
        readonly Game game;

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
                    ActivationNums = new int[1] { 1 };
                    Cost = 1;
                    Symbol = Symbols.agriculture;
                    effect = (caller) =>
                    {
                        if(owner.shouldLog)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                        }
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.ranch:
                    ActivationNums = new int[1] { 2 };
                    Cost = 1;
                    Symbol = Symbols.ranch;
                    effect = (caller) =>
                    {
                        if (owner.shouldLog)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                        }
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.bakery:
                    ActivationNums = new int[2] { 2, 3 };
                    Cost = 1;
                    Symbol = Symbols.store;
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            int change = 1;
                            if (owner.HasMall) { change = 2; }
                            owner.ChangeCoins(change);
                        }
                    };
                    break;
                case Establishments.cafe:
                    ActivationNums = new int[1] { 3 };
                    Cost = 2;
                    Symbol = Symbols.restaurant;
                    effect = (caller) =>
                    {
                        if(caller != owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            int targetSteal = 1; //Set to 2 if has mall
                            if(owner.HasMall) { targetSteal = 2; }
                            int stealNum = caller.GetMaxSteal(targetSteal);
                            if(stealNum != 0)
                            {
                                caller.ChangeCoins(-stealNum);
                                owner.ChangeCoins(stealNum);
                                if (owner.shouldLog)
                                {
                                    Console.WriteLine($"Stole {stealNum} from {caller}");
                                }
                            }
                            else if (owner.shouldLog)
                            {
                                Console.WriteLine("...but there was nothing to steal!");
                            }
                        }
                    };
                    break;
                case Establishments.convenience_store:
                    ActivationNums = new int[1] { 4 };
                    Cost = 2;
                    Symbol = Symbols.store;
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            int change = 3;
                            if (owner.HasMall) { change = 4; }
                            owner.ChangeCoins(change);
                        }
                    };
                    break;
                case Establishments.forest:
                    ActivationNums = new int[1] { 5 };
                    Cost = 3;
                    Symbol = Symbols.nature;
                    effect = (caller) =>
                    {
                        if (owner.shouldLog)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                        }
                        owner.ChangeCoins(1);
                    };
                    break;
                case Establishments.stadium:
                    ActivationNums = new int[1] { 6 };
                    Cost = 6;
                    Symbol = Symbols.major;
                    owner.AddStadium();
                    effect = (caller) =>
                    {
                        if(caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            foreach (PlayerHandler player in game.players)
                            {
                                if(player == owner) { continue; }
                                int stealNum = player.GetMaxSteal(2);
                                if (stealNum != 0)
                                {
                                    player.ChangeCoins(-stealNum);
                                    owner.ChangeCoins(stealNum);
                                    if (owner.shouldLog)
                                    {
                                        Console.WriteLine($"Stole {stealNum} from {player}");
                                    }
                                }
                                else if(owner.shouldLog)
                                {
                                    Console.WriteLine("...but there was nothing to steal!");
                                }
                            }
                        }
                    };
                    break;
                case Establishments.tv_station:
                    ActivationNums = new int[1] { 6 };
                    Cost = 7;
                    Symbol = Symbols.major;
                    owner.AddStation();
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            PlayerHandler target = null;
                            if(owner.parentComputer == null)
                            {
                                //Prompt to pick another player
                                Console.WriteLine($"{owner}, pick another player to steal up to 5 coins from");
                                game.PrintBalances();

                                while (target == null)
                                {
                                    string selection = Console.ReadLine();
                                    foreach (PlayerHandler player in game.players)
                                    {
                                        if (selection.ToLower().Equals(player.ToString().ToLower()))
                                        {
                                            target = player;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Computer picks opponent with most money
                                target = game.players[0];
                                foreach(PlayerHandler p in game.players)
                                {
                                    if(p != owner && p.NumCoins > target.NumCoins)
                                    {
                                        target = p;
                                    }
                                }
                            }
                            //Steal up to 5 coins from target
                            int stealNum = target.GetMaxSteal(5);
                            target.ChangeCoins(-stealNum);
                            owner.ChangeCoins(stealNum);
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"\r\nStole {stealNum} from {target}");
                            }


                        }
                    };
                    break;
                case Establishments.business_center:
                    ActivationNums = new int[1] { 6 };
                    Cost = 8;
                    Symbol = Symbols.major;
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
                                        if(c.Symbol != Symbols.major && c.Symbol != Symbols.landmark)
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
                                    if (card.Symbol != Symbols.major && card.Symbol != Symbols.landmark && selection.ToLower().Equals(card.ToString()))
                                    {
                                        targetCard = card;
                                    }
                                }
                            }
                            //Prompt to pick a card from self
                            Console.WriteLine("Pick a card to give away");
                            foreach (Card c in owner.GetCardsAsArray())
                            {
                                if(c.Symbol != Symbols.major && c.Symbol != Symbols.landmark)
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
                                    if (card.Symbol != Symbols.major && card.Symbol != Symbols.landmark && selection.ToLower().Equals(card.ToString()))
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
                    ActivationNums = new int[1] { 7 };
                    Cost = 5;
                    Symbol = Symbols.factory;
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            owner.ChangeCoins(3 * owner.NumOfSymbol(Symbols.ranch));
                        }
                    };
                    break;
                case Establishments.furniture_factory:
                    ActivationNums = new int[1] { 8 };
                    Cost = 3;
                    Symbol = Symbols.factory;
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            owner.ChangeCoins(3 * owner.NumOfSymbol(Symbols.nature));
                        }
                    };
                    break;
                case Establishments.mine:
                    ActivationNums = new int[1] { 9 };
                    Cost = 6;
                    Symbol = Symbols.nature;
                    effect = (caller) =>
                    {
                        if (owner.shouldLog)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                        }
                        owner.ChangeCoins(5);
                    };
                    break;
                case Establishments.family_restaurant:
                    ActivationNums = new int[2] { 9, 10 };
                    Cost = 3;
                    Symbol = Symbols.restaurant;
                    effect = (caller) =>
                    {
                        if (caller != owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            int targetSteal = 2; //Set to 3 if has mall
                            if (owner.HasMall) { targetSteal = 3; }
                            int stealNum = caller.GetMaxSteal(targetSteal);
                            if (stealNum != 0)
                            {
                                caller.ChangeCoins(-stealNum);
                                owner.ChangeCoins(stealNum);
                                if (owner.shouldLog)
                                {
                                    Console.WriteLine($"Stole {stealNum} from {caller}");
                                }
                            }
                            else if (owner.shouldLog)
                            {
                                Console.WriteLine("...but there was nothing to steal!");
                            }
                        }
                    };
                    break;
                case Establishments.apple_orchard:
                    ActivationNums = new int[1] { 10 };
                    Cost = 3;
                    Symbol = Symbols.agriculture;
                    effect = (caller) =>
                    {
                        if (owner.shouldLog)
                        {
                            Console.WriteLine($"{owner}'s {this} activated!");
                        }
                        owner.ChangeCoins(3);
                    };
                    break;
                case Establishments.fruit_and_vegetable_market:
                    ActivationNums = new int[2] { 11, 12 };
                    Cost = 2;
                    Symbol = Symbols.factory;
                    effect = (caller) =>
                    {
                        if (caller == owner)
                        {
                            if (owner.shouldLog)
                            {
                                Console.WriteLine($"{owner}'s {this} activated!");
                            }
                            owner.ChangeCoins(2 * owner.NumOfSymbol(Symbols.agriculture));
                        }
                    };
                    break;
                case Establishments.train_station:
                    ActivationNums = null;
                    Cost = 4;
                    Symbol = Symbols.landmark;
                    effect = (x) =>
                    {
                        owner.AddTrain();
                    };
                    break;
                case Establishments.shopping_mall:
                    ActivationNums = null;
                    Cost = 10;
                    Symbol = Symbols.landmark;
                    effect = (x) =>
                    {
                        owner.AddMall();
                    };
                    break;
                case Establishments.amusement_park:
                    ActivationNums = null;
                    Cost = 16;
                    Symbol = Symbols.landmark;
                    effect = (x) =>
                    {
                        owner.AddPark();
                    };
                    break;
                case Establishments.radio_tower:
                    ActivationNums = null;
                    Cost = 22;
                    Symbol = Symbols.landmark;
                    effect = (x) =>
                    {
                        owner.AddRadio();
                    };
                    break;
            }
        }

        public static string GetPurchasableEstablishments(PlayerHandler player)
        {
            int coins = player.NumCoins;
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
            if(!player.HasTrain)
            {
                str += $"{GetEstDesc(Establishments.train_station)}\r\n";
            }
            if(coins < 5) { return str; }
            //Five costs
            str += $"{GetEstDesc(Establishments.cheese_factory)}\r\n";
            if(coins < 6) { return str; }
            //Six costs
            if(!player.HasStadium)
            {
                str += $"{GetEstDesc(Establishments.stadium)}\r\n{GetEstDesc(Establishments.mine)}\r\n";
            }
            if(coins < 7) { return str; }
            //Seven costs
            if(!player.HasStation)
            {
                str += $"{GetEstDesc(Establishments.tv_station)}\r\n";
            }
            if(coins < 8) { return str; }
            //Eight costs
            if(!player.HasCenter)
            {
                str += $"{GetEstDesc(Establishments.business_center)}\r\n";
            }
            if(coins < 10) { return str; }
            //Ten costs
            if(!player.HasMall)
            {
                str += $"{GetEstDesc(Establishments.shopping_mall)}\r\n";
            }
            if(coins < 16) { return str; }
            //Sixteen costs
            if (!player.HasPark)
            {
                str += $"{GetEstDesc(Establishments.amusement_park)}\r\n";
            }
            if(coins < 22) { return str; }
            //Twenty-two costs
            if (!player.HasRadio)
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
