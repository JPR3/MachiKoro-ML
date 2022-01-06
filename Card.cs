using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachiKoro_ML
{
    public class Card
    {
        public enum EstablishmentTypes
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
        EstablishmentTypes est;
        int[] activationNums;
        Action<PlayerHandler> effect;

        public Card(EstablishmentTypes estType)
        {
            est = estType;
            SetValues(est); //Sets the activationNums and effect variables based on establishment type
        }

        public void Invoke(PlayerHandler parent)
        {
            effect.Invoke(parent);
        }

        void SetValues(EstablishmentTypes type)
        {
            switch (type)
            {
                case EstablishmentTypes.Wheat_Field:
                    activationNums = new int[1] { 1 };
                    break;
                case EstablishmentTypes.Ranch:
                    activationNums = new int[1] { 2 };
                    break;
                case EstablishmentTypes.Bakery:
                    activationNums = new int[2] { 2, 3 };
                    break;
                case EstablishmentTypes.Cafe:
                    activationNums = new int[1] { 3 };
                    break;
                case EstablishmentTypes.Convenience_Store:
                    activationNums = new int[1] { 4 };
                    break;
                case EstablishmentTypes.Forest:
                    activationNums = new int[1] { 5 };
                    break;
                case EstablishmentTypes.Stadium:
                    activationNums = new int[1] { 6 };
                    break;
                case EstablishmentTypes.TV_Station:
                    activationNums = new int[1] { 6 };
                    break;
                case EstablishmentTypes.Business_Center:
                    activationNums = new int[1] { 6 };
                    break;
                case EstablishmentTypes.Cheese_Factory:
                    activationNums = new int[1] { 7 };
                    break;
                case EstablishmentTypes.Furniture_Factory:
                    activationNums = new int[1] { 8 };
                    break;
                case EstablishmentTypes.Mine:
                    activationNums = new int[1] { 9 };
                    break;
                case EstablishmentTypes.Family_Restaurant:
                    activationNums = new int[2] { 9, 10 };
                    break;
                case EstablishmentTypes.Apple_Orchard:
                    activationNums = new int[1] { 10 };
                    break;
                case EstablishmentTypes.Fruit_And_Vegetable_Market:
                    activationNums = new int[2] { 11, 12 };
                    break;
            }
        }
    }
}
