using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
        public static void AddIngredient(GameRegistry reg, bool dump = false)
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
                effects.RemoveAll(e => e.Name == "");
                Dialog.HelpMessage("Is this a 1) Vitamin 2) Mineral 3) Other ?");
                IngredientType type = (IngredientType)Convert.ToInt32(Console.ReadLine() as string) - 1;
                result = new Ingredient(name, type, effects);
                Dialog.Describe(result);
                Console.WriteLine("Is this correct? y/n");
                string check = "--";
                while (check.ToLower() != "y" && check.ToLower() != "n")
                    check = Console.ReadLine();
                complete = check.ToLower() == "y";
            }
            reg.Ingredients.Add(result);
            reg.JsonSave();
            if (dump)
            {
                Directory.CreateDirectory("ObjectDumps");
                File.WriteAllText($"ObjectDumps/{result.Name}.json", JsonConvert.SerializeObject(result));
                Console.WriteLine($"{result.Name} dumped to file in ObjectDumps");
            }
                
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
        public static void AddIngredientFromFile(GameRegistry reg, string location)
        {
            Ingredient newIngredient = JsonConvert.DeserializeObject<Ingredient>(File.ReadAllText(location));
            newIngredient.Effects.ForEach(e => {
                if (!reg.Effects.Contains(e))
                    reg.Effects.Add(e);
            });
            reg.Ingredients.Add(newIngredient);
            Console.WriteLine($"{newIngredient.Name} added from file");
            reg.JsonSave();
        }
        public static void AddIngredientFromText(GameRegistry reg, string json)
        {
            Ingredient newIngredient = JsonConvert.DeserializeObject<Ingredient>(json);
            newIngredient.Effects.ForEach(e => {
                if (!reg.Effects.Contains(e))
                    reg.Effects.Add(e);
            });
            reg.Ingredients.Add(newIngredient);
            Console.WriteLine($"{newIngredient.Name} added from file");
            reg.JsonSave();
        }
        public static void MonitorVitals(GameRegistry reg, int id)
        {
            Animal testee = reg.Subjects[id] as Animal;
            Console.WriteLine($"-- {testee.Name} --");
            for (int i = 0; i < testee.Attributes.Count; i++)
            {
                Trait t = (Trait)i;
                Console.WriteLine($"{t} : {testee.Attributes[t]}");
            }
            Console.ReadKey();
        }
    }
}