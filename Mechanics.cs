using System;
using System.Collections.Generic;
using System.IO;

namespace VitaminUnderscore
{
    /*
    * Dialog Class
    *   Used for handling in game messages, descriptions and forms
    */
    public static class Dialog
    {
        public const bool DevMode = true;        
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
            if (obj.GetType() == typeof(Subject))
            {
                Subject described = obj as Subject;
                Console.WriteLine($"-- {described.Name} --\nAge: {described.Age}\nDescription: {described.Age}");
            }
        }
        // Main Menu Dialog Option
        public static bool MainMenu(GameRegistry reg, bool firstTime = false)
        {
            Console.Clear();
            bool exit = false;
            ColouredMessage("-- VitaSys Alternative Medicine OS --\nWhat would you like to do today?", ConsoleColor.Yellow);
            if (Directory.Exists("SaveData") == false)
            {
                Dialog.ColouredMessage("Loading First Time Startup...", ConsoleColor.Cyan);
                reg.JsonLoad();
                System.Threading.Thread.Sleep(10);
                reg.JsonSave();
                Dialog.ColouredMessage("Loaded!", ConsoleColor.Green);
            }
            HelpOptions.ForEach(h => Console.WriteLine(h.ToString()));
            string commandString = Console.ReadLine();
            switch (commandString.Split(' ')[0].ToLower())
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
                case "3":
                    reg.Subjects.Add(CreateSubject(reg));
                break;
                case "4":
                    Console.Clear();
                    Dialog.ColouredMessage("-- Subjects --", ConsoleColor.Yellow);
                    reg.Subjects.ForEach(s => Describe(s));
                    Console.ReadKey();
                break;
                case "0":
                    Console.WriteLine("Saving & Shutting down...");
                    reg.JsonSave();
                    exit = true;
                break;
                case "100":
                    Console.WriteLine(reg.CreatedFormulations.Count.ToString());
                    reg.JsonSave();
                break;
                case "101":
                    reg.JsonLoad();
                break;
                // Developer Specific Commands
                case "dev_effect_add":
                    if (DevMode)
                        DeveloperDialog.AddEffect(reg);
                break;
                case "dev_ingredient_add":
                    if (DevMode)
                        DeveloperDialog.AddIngredient(reg);
                break;
                case "dev_list":
                    DeveloperDialog.MonitorList(reg,commandString.Split(' ')[1].ToLower());
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
            "3) Recruit New Subject",
            "4) View Subjects",
            //"420) Enter Dev Mode",
            "99) View This Menu",
            "0) Close Game",
            "100) Save Game",
            "101) Load Game"
        };
        // Create a formulation from the command line
        public static Formulation CreateFormulation(GameRegistry reg)
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
                type = (IngredientType)Convert.ToInt16(currentType);
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
        // Add Test Subject
        public static Subject CreateSubject(GameRegistry reg)
        {
            Subject newSubject = null;
            if (reg.CreatedFormulations.Count == 0)
            {
                Console.WriteLine("No formulations available, press any key to create one.");
                Console.ReadKey();
                reg.CreatedFormulations.Add(CreateFormulation(reg));
            }
            bool complete = false;
            while (!complete)
            {
                Console.Clear();
                ColouredMessage("-- HR Department Subject Selector v1.2 --\nSelect Testing Formulation:", ConsoleColor.Green);
                reg.CreatedFormulations.ForEach(i => Console.WriteLine(i.Name));
                string form = "";
                Formulation formulation = null;
                while (formulation == null)
                {
                    form = Console.ReadLine();
                    formulation = reg.CreatedFormulations.Find(f => f.Name.ToLower() == form.ToLower());
                }                
                // Formulation formulation = reg.CreatedFormulations.Find(f => f.Name == form);
                newSubject = new Subject(formulation);
                Describe(newSubject);
                Console.WriteLine("Is this correct? y/n");                
                string choice = "";
                while (choice.ToLower() != "y" && choice.ToLower() != "n")
                    choice = Console.ReadLine();
                complete = choice.ToLower() == "y";
            }
            return newSubject;
        }
    }
}