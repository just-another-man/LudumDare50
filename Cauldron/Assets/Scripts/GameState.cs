
using System;
using System.Collections.Generic;

namespace CauldronCodebase
{
    //Class holds the game state and can be used for saving and loading
    public class GameState
    {
        private int fame;
        private int fear;
        private int money;
        public int Fame
        {
            get => fame;
            set => Set(Statustype.Fame, value);
        }
        public int Fear
        {
            get => fear;
            set => Set(Statustype.Fear, value);
        }
        public int Money
        {
            get => money;
            set => Set(Statustype.Money, value);
        }
        public Phase phase;
        public int currentDay = 1;
        public int cardsDrawnToday;
        public Encounter currentCard;
        public List<Potions> potionsTotal;
        public int wrongPotionsCount;

        public IEncounterDeck currentDeck;
        public NightEventProvider currentEvents;

        private int statusMax;
        
        public event System.Action StatusChanged;

        public GameState(int max, int startValue, IEncounterDeck deck, NightEventProvider events)
        {
            potionsTotal = new List<Potions>(15);
            
            statusMax = max;
            fear = startValue;
            fame = startValue;
            currentDeck = deck;
            currentEvents = events;
        }
        
        public int Get(Statustype type)
        {
            switch (type)
            {
                case Statustype.Money:
                    return money;
                case Statustype.Fear:
                    return fear;
                case Statustype.Fame:
                    return fame;
            }
            return -1000;
        }

        private int Set(Statustype type, int newValue)
        {
            int statValue = Get(type);
            int num = newValue - statValue;
            if (num == 0)
                return statValue;
            if (type == Statustype.Money && num < 0)
                return statValue;
            statValue += num;
            if (statValue > statusMax)
                statValue = statusMax;
            else if (statValue < 0)
                statValue = 0;
            switch (type)
            {
                case Statustype.Money:
                    money = statValue;
                    break;
                case Statustype.Fear:
                    fear = statValue;
                    break;
                case Statustype.Fame:
                    fame = statValue;
                    break;
            }
            StatusChanged?.Invoke();
            return statValue;
        }

        public int Add(Statustype type, int value)
        {
            return Set(type, value + Get(type));
        }
    }

    public enum Phase
    {
        Night,
        DayNoCustomer,
        DayCustomer
    }
}