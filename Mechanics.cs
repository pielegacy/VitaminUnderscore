using System;
using System.Collections.Generic;

namespace VitaminUnderscore
{
    /*
    * Dialog Class
    *   Used for handling in game messages, descriptions and forms
    */
    public static class Dialog
    {
        public static void MessageLoad()
        {
            Console.WriteLine("Loading...");
        }
        public static void MessageDone()
        {
            Console.WriteLine("Done!");
        }
        // Display text in color that isn't white
        public static void ColouredMessage(string message, ConsoleColor textColor)
        {
            Console.ForegroundColor = textColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void HelpMessage(string message)
        {
            ColouredMessage(message, ConsoleColor.Cyan);
        }
        public static void WarningMessage(string message)
        {
            ColouredMessage(message, ConsoleColor.Red);
        }
        // Universal Class used to Describe the games various objects
        public static void Describe(NamedObject obj)
        {
            if (obj.GetType() == typeof(Ingredient))
            {
                Ingredient described = obj as Ingredient;
                Console.WriteLine($"-- {described.Name} --\nType: {described.Type}\nEffects: ");
                described.Effects.ForEach(e => Console.WriteLine($" - {e.Name}"));
            }
            if (obj.GetType() == typeof(Formulation))
            {
                Formulation described = obj as Formulation;
                Console.WriteLine($"-- {described.Name} --\nType: {described.Type}\nIngredients: ");
                described.Ingredients.ForEach(i => Console.WriteLine($" - {i.Name}"));
            }
            if (obj.GetType() == typeof(Effect))
            {
                Effect described = obj as Effect;
                Console.WriteLine($"-- {described.Name} --\nTrait: {described.Trait}\nMagnitude: {described.Amount}");
            }
        }
        // Main Menu Dialog Option
        public static bool MainMenu(IngredientsRegistry reg, bool firstTime = false)
        {
            Console.Clear();
            bool exit = false;
            ColouredMessage("-- VitaSys Alternative Medicine OS --\nWhat would you like to do today?", ConsoleColor.Yellow);
            if (firstTime)
            {
                Dialog.ColouredMessage("Loading First Time Startup...", ConsoleColor.Cyan);
                System.Threading.Thread.Sleep(10);
                Dialog.ColouredMessage("Loaded!", ConsoleColor.Green);
            }
            HelpOptions.ForEach(h => Console.WriteLine(h.ToString()));
            string commandString = Console.ReadLine();
            switch (commandString.Split(' ')[0])
            {
                case "1":
                    reg.CreatedFormulations.Add(CreateFormulation(reg));
                break;
                case "2":
                    Console.Clear();
                    Dialog.ColouredMessage("-- Your Forumalae --", ConsoleColor.Cyan);
                    reg.CreatedFormulations.ForEach(f => Describe(f));
                    Console.ReadKey();
                break;
                case "0":
                    Console.WriteLine("Shutting down...");
                    exit = true;
                break;
                case "100":
                    Console.WriteLine(reg.CreatedFormulations.Count.ToString());
                    reg.JsonSave();
                break;
                case "101":
                    reg.JsonLoad();
                break;
                default:
                break;
            }
            return exit;
        }
        private static List<string> HelpOptions = new List<string>()
        {
            "1) Create New Formulation",
            "2) View Formulations",
            //"420) Enter Dev Mode",
            "99) View This Menu",
            "0) Close Game",
            "100) Save Game",
            "101) Load Game"
        };
        // Create a formulation from the command line
        public static Formulation CreateFormulation(IngredientsRegistry reg)
        {
            Formulation newForm = null;
            bool complete = false;
            while (!complete)
            {
                Console.Clear();
                ColouredMessage("-- Formulation Builder v1.3 --", ConsoleColor.Green);
                HelpMessage("Name?");
                string name = Console.ReadLine();
                string currentIngredient = "";
                List<Ingredient> ingredients = new List<Ingredient>();
                HelpMessage("Ingredients?");
                while (currentIngredient.ToLower() != "done")
                {
                    currentIngredient = currentIngredient.ToLower();
                    if (currentIngredient != "list")
                    {
                        if (currentIngredient == "")
                            HelpMessage("--Type the name of the Ingredient to add it to the formulation, type done when you are finished--");
                        if (reg.RetIngredient(currentIngredient) != null)
                            ingredients.Add(reg.RetIngredient(currentIngredient));
                        else
                            WarningMessage("Invalid ingredient name, type list to see available ingredients");
                    }
                    else
                        reg.Ingredients.ForEach(r => Console.WriteLine($"- {r.Name}"));
                    currentIngredient = Console.ReadLine();
                }
                string currentType = "";
                HelpMessage("Is this a compound of 1) Vitamins 2) Minerals 3) Both ?");
                while (currentType != "1" && currentType != "2" && currentType != "3")
                    currentType = Console.ReadLine();
                IngredientType type = IngredientType.Other;
                switch (Convert.ToInt16(currentType))
                {
                    case 1:
                        type = IngredientType.Vitamin;
                        break;
                    case 2:
                        type = IngredientType.Mineral;
                        break;
                }
                newForm = new Formulation(name, ingredients, type);
                Describe(newForm);
                Console.WriteLine("Is this correct? y/n");
                string choice = "";
                while (choice.ToLower() != "y" && choice.ToLower() != "n")
                    choice = Console.ReadLine();
                complete = choice.ToLower() == "y";
            }
            return newForm;
        }
    }
}