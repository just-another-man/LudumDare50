using System.Collections.Generic;
using System.IO;
using EasyLoc;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New night event", menuName = "Night event/Basic", order = 8)]
    public class NightEvent : LocalizableSO
    {
        public string title;
        [TextArea(8,8)]
        public string flavourText;
        public Sprite picture;
        public float moneyCoef, fearCoef, fameCoef;
        public Encounter bonusCard;

        public void ApplyModifiers(GameState game, MainSettings settings)
        {
            game.Fame += CalculateModifier(Statustype.Fame, settings);
            game.Fear += CalculateModifier(Statustype.Fear, settings);
            game.Money += CalculateModifier(Statustype.Money, settings);
        }

        public int CalculateModifier(Statustype type, MainSettings settings)
        {
            switch (type)
            {
                case Statustype.Money:
                    return Mathf.FloorToInt(moneyCoef * settings.gameplay.defaultMoneyChangeEvent);
                case Statustype.Fear:
                    return Mathf.FloorToInt(fearCoef * settings.gameplay.defaultStatChange);
                case Statustype.Fame:
                    return Mathf.FloorToInt(fameCoef * settings.gameplay.defaultStatChange);
            }

            return 0;
        }
        
        public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            //cache??
            string[] lines = localizationCSV.text.Split('\n');
            List<int> requiredColumns = new List<int>();
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language.ToString()))
                {
                    requiredColumns.Add(i);
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data[0] == name)
                {
                    if (requiredColumns[0] >= data.Length)
                        return false;
                    flavourText = data[requiredColumns[0]];
                    return true;
                }
            }
            return false;
        }
        
        [ContextMenu("Export All Events to csv")]
        public void ExportAllNightEvents()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Events.csv");
            file.WriteLine("id;description_RU;description_EN");
            var events = Resources.FindObjectsOfTypeAll<NightEvent>();
            foreach (var nightEvent in events)
            {
                file.WriteLine($"{nightEvent.name};{nightEvent.flavourText}");
            }
            file.Close();
        }
    }
    
    
}