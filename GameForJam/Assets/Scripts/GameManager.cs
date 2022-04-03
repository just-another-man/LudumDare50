using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public IngredientsData ingredientsBook;
        public EncounterDeck cardDeck;
        public Canvas textCanvas;
        public Text textField;
        
        public Status money, fear, fame;
        public float fameMoneyCoef = 0.1f;

        private int moneyUpdateTotal, fameUpdateTotal, fearUpdateTotal;

        private int currentDay = 1;
        private int cardsDrawnToday;
        public int cardsPerDay = 3;
        public int cardsDealtAtNight = 3;
        public float nightDelay = 4f;
        public float villagerDelay = 2f;
        
        //stats
        [Header("for info")]
        public Encounter currentCard;
        public List<Potions> rightPotionsTotal;
        public List<Potions> wrongPotionsTotal;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
            {
                Debug.LogError("double singleton:"+this.GetType().Name);
            }

            rightPotionsTotal = new List<Potions>(15);
            wrongPotionsTotal = new List<Potions>(15);

            money = new Status();
            fame = new Status();
            fear = new Status();
            
            HideText();
        }

        public void ShowText(string text)
        {
            textCanvas.gameObject.SetActive(true);
            textField.text = text;
        }

        public void HideText()
        {
            textCanvas.gameObject.SetActive(false);
        }

        private void Start()
        {
            cardDeck.Init();
            DrawCard();
        }

        public void DrawCard()
        {
            currentCard = cardDeck.GetTopCard();
            if (currentCard == null)
            {
                Debug.LogError("Ran out of cards!!!");
                return;
            }
            currentCard.Init();
            cardsDrawnToday ++;
            Debug.Log(currentCard.text);
            ShowText(currentCard.text);
            //new villager enters
        }

        public void EndEncounter(Potions potion)
        {
            //HideText();
            //visual effects
            //potion popup
            
            //Debug.Log(money.Value + defaultMoneyBonus);
            //status update
            if (currentCard.actualVillager != null) 
            {
                money.Add(currentCard.actualVillager.moneyBonus + Mathf.FloorToInt(fame.Value() * fameMoneyCoef));
                fame.Add(currentCard.actualVillager.fameBonus);
                fear.Add(currentCard.actualVillager.fearBonus);
            }
            //Debug.Log(money.Value);
            
            //compare potion and save bonuses for later
            if (potion == currentCard.requiredPotion)
            {
                moneyUpdateTotal += currentCard.moneyBonus;
                fearUpdateTotal += currentCard.fearBonus;
                fameUpdateTotal += currentCard.fameBonus;
                rightPotionsTotal.Add(potion);
                if (currentCard.bonusCard != null)
                    cardDeck.AddCardToPool(currentCard.bonusCard);
            }
            else
            {
                moneyUpdateTotal += currentCard.moneyPenalty;
                fearUpdateTotal += currentCard.fearPenalty;
                fameUpdateTotal += currentCard.famePenalty;
                wrongPotionsTotal.Add(potion);
                if (currentCard.penaltyCard != null)
                    cardDeck.AddCardToPool(currentCard.penaltyCard);
            }
            
            //villager exits
            
            //draw new card
            if (cardsDrawnToday >= cardsPerDay)
            {
                StartCoroutine(StartNewDay());
            }
            else
            {
                StartCoroutine(DrawCardWithDelay());
            }

        }

        private IEnumerator DrawCardWithDelay()
        {
            yield return new WaitForSeconds(villagerDelay);
            DrawCard();
        }

        private IEnumerator StartNewDay()
        {
            yield return new WaitForSeconds(villagerDelay);
            
            Witch.instance.Hide();

            //text message
            Debug.Log("New day!");
            ShowText("Новый день!");

            yield return new WaitForSeconds(nightDelay/2);
            
            //deck update
            cardDeck.NewDayPool(currentDay);
            cardDeck.DealCards(cardsDealtAtNight);
            currentDay++;
            cardsDrawnToday = 0;
            
            //status update
            money.Add(moneyUpdateTotal);
            moneyUpdateTotal = 0;
            fear.Add(fearUpdateTotal);
            fearUpdateTotal = 0;
            fame.Add(fameUpdateTotal);
            fameUpdateTotal = 0;
            
            yield return new WaitForSeconds(nightDelay/2);
            
            HideText();
            Witch.instance.Wake();
            //in case the player had put some ingredients in the pot during the night - clear the pot
            Cauldron.instance.Clear();
            DrawCard();
        }

    }
}