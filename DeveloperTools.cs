using System;
using System.Collections.Generic;
using System.Linq;

namespace VitaminUnderscore
{
    /*
        Dev Tools
            Note that proper error checking mechanisms and such are not in place as they are
            not required for me - Alex
    */
    public class DeveloperDialog
    {
        /*
            Add effect to save using dialogs
        */
        public static void AddEffect(GameRegistry reg)
        {
            Effect result = null;
            bool complete = false;
            while (!complete)
            {
                Console.WriteLine("-- Add Effect To Database --");
                Dialog.HelpMessage("Name?");
                string name = Console.ReadLine();
                Dialog.HelpMessage("Trait?");
                for (int i = 0; i < Enum.GetValues(typeof(Trait)).Length; i++)
                    Dialog.HelpMessage(i.ToString() + " - " + ((Trait)i).ToString());
                int choice = Convert.ToInt32(Console.ReadLine());
                Trait trait = (Trait)choice;
                Dialog.HelpMessage("Amount?");
                int amount = Convert.ToInt32(Console.ReadLine());
                result = new Effect(name, trait, amount);
                Dialog.Describe(result);
                Console.WriteLine("Is this correct? y/n");
                string check = "";
                while (check.ToLower() != "y" && check.ToLower() != "n")
                    check = Console.ReadLine();
                complete = check.ToLower() == "y";
            }
            reg.Effects.Add(result);
            reg.JsonSave(); // Save Changes
        }
        /*
            Add ingredient to save using dialogs
        */
        public static void AddIngredient(GameRegistry reg)
        {
            Ingredient result = null;
            bool complete = false;
            while (!complete)
            {
                Console.WriteLine("-- Add Ingredient To Database --");
                Dialog.HelpMessage("Name?");
                string name = Console.ReadLine();
                Dialog.HelpMessage("*List* its effects by name, type done when finished");
                string effectName = "";
                List<Effect> effects = new List<Effect>();
                List<string> effectNames = new List<string>();
                while (effectName.ToLower() != "done")
                {
                    if (effectName == "list")
                        reg.Effects.ForEach(e => Dialog.Describe(e));
                    else
                        effectNames.Add(effectName.ToLower());
                    effectName = Console.ReadLine();
                }
                effects = reg.RetEffects(effectNames.ToArray());
                Dialog.HelpMessage("Is this a 1) Vitamin 2) Mineral 3) Other ?");
                IngredientType type = (IngredientType)Convert.ToInt32(Console.ReadLine() as string);
                result = new Ingredient(name, type, effects);
                Dialog.Describe(result);
                Console.WriteLine("Is this correct? y/n");
                string check = "";
                while (check.ToLower() != "y" && check.ToLower() != "n")
                    check = Console.ReadLine();
                complete = check.ToLower() == "y";
            }
            reg.Ingredients.Add(result);
            reg.JsonSave();
        }
        public static void MonitorList(GameRegistry reg, string type)
        {
            while (true)
            {
                Console.Clear();
                reg.JsonLoad();
                switch (type)
                {
                    case "ingredients":
                    case "i":
                        reg.Ingredients.ForEach(i => Dialog.Describe(i));
                        break;
                    case "effects":
                    case "e":
                        reg.Effects.ForEach(i => Dialog.Describe(i));
                        break;
                    case "formulations":
                    case "f":
                        reg.CreatedFormulations.ForEach(i => Dialog.Describe(i));
                        break;
                }
                Console.ReadKey();
            }
        }
    }
}