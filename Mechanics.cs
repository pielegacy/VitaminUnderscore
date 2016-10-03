using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

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
                Console.WriteLine($"-- {described.Name} --\nAge: {described.Age}\nDescription: {described.Age}\nAssigned Formulation : {described.AssignedFormulation.Name}\n");
            }
        }
        // Main Menu Dialog Option
        public static bool MainMenu(GameRegistry reg, bool firstTime = false)
        {
            Console.Clear();
            bool exit = false;
            if (!firstTime)
                firstTime = Directory.Exists("SaveData") == false;
            ColouredMessage("-- VitaSys Alternative Medicine OS --\nWhat would you like to do today?", ConsoleColor.Yellow);
            if (firstTime)
            {
                Dialog.ColouredMessage("Loading First Time Startup...", ConsoleColor.Cyan);
                reg.JsonLoad();
                System.Threading.Thread.Sleep(10);
                Dialog.ColouredMessage("Loaded!", ConsoleColor.Green);
            }
            else
                reg.JsonLoad();
            if (!firstTime)
            {            
            HelpOptions.ForEach(h => Console.WriteLine(h.ToString()));
            string commandString = Console.ReadLine();
            switch (commandString.Split(' ')[0].ToLower())
            {
                case "1":
                    reg.CreatedFormulations.Add(CreateFormulation(reg));
                    reg.JsonSave();
                    break;
                case "2":
                    Console.Clear();
                    Dialog.ColouredMessage("-- Your Forumalae --", ConsoleColor.Cyan);
                    reg.CreatedFormulations.ForEach(f => Describe(f));
                    Console.ReadKey();
                    break;
                case "3":
                    reg.Subjects.Add(CreateSubject(reg));
                    reg.JsonSave();
                    break;
                case "4":
                    Console.Clear();
                    Dialog.ColouredMessage("-- Subjects --", ConsoleColor.Yellow);
                    reg.Subjects.ForEach(s => Describe(s));
                    Console.ReadKey();
                    break;
                case "5":
                    Dialog.TestSubject(reg);
                    reg.JsonSave();
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
                case "#dea":
                    if (DevMode)
                        DeveloperDialog.AddEffect(reg);
                    break;
                case "dev_ingredient_add":
                case "#dia":
                    if (DevMode)
                        if (commandString.Split(' ')[1].ToLower() == "dump")
                            DeveloperDialog.AddIngredient(reg, true);
                        else
                            DeveloperDialog.AddIngredient(reg);
                    break;
                case "dev_list":
                case "#dl":
                    DeveloperDialog.MonitorList(reg, commandString.Split(' ')[1].ToLower());
                    break;
                case "50":
                case "dev_load":
                case "#load":
                    Console.WriteLine("-- File location? --");
                    string loc = Console.ReadLine();
                    DeveloperDialog.AddIngredientFromFile(reg, loc);
                break;
                case "dev_monitor":
                case "#dm":
                    DeveloperDialog.MonitorVitals(reg, Int32.Parse(commandString.Split(' ')[1]));
                    break;
                default:
                    break;
            }
            }
            else 
            {
                Console.WriteLine("Welcome to Vitamin _, a vitamin development simulation.\nTo start let's test your typing, type the word vitamin");
                string initAnswer = Console.ReadLine();
                while (initAnswer.ToLower() != "vitamin")
                {
                    Console.WriteLine("Come on now! You can do it! 'Vitamin', without the quotes of course");
                    initAnswer = Console.ReadLine();
                }
                Console.WriteLine("PERFECT! Let's save your progress, press any key to begin the game.");
                reg.JsonSave();
                Console.ReadKey();
                    
            }
            return exit;
        }
        private static List<string> HelpOptions = new List<string>()
        {
            "1) Create New Formulation",
            "2) View Formulations",
            "3) Recruit New Subject",
            "4) View Subjects",
            "5) Test Formulation",
            "50) Load Ingredient From File",
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
                type = (IngredientType)Convert.ToInt16(currentType) - 1;
                newForm = new Formulation(name, ingredients, type);
                Describe(newForm);
                complete = Dialog.YesOrNo();
            }
            return newForm;
        }
        public static bool YesOrNo(string prompt = "Is this correct?")
        {
            Console.WriteLine($"{prompt} (y/n)");
            string choice = "";
                while (choice.ToLower().Split(' ')[0] != "y" && choice.ToLower().Split(' ')[0] != "n")
                    choice = Console.ReadLine();
            return choice.ToLower().Split(' ')[0] == "y";
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
                complete = Dialog.YesOrNo();
            }
            return newSubject;
        }
        public static void TestSubject(GameRegistry reg)
        {
            bool complete = false;
            Formulation d = null;
            Subject testee = null;
            while (!complete)
            {
                Console.Clear();
                ColouredMessage("-- Subject Testing Framework v1.420 --\nPlease select a Subject by index:", ConsoleColor.Yellow);
                for (int i = 0; i < reg.Subjects.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {reg.Subjects[i].Name}");
                }
                int indexInt = Convert.ToInt32(Console.ReadLine()); // TODO : Check for errors
                testee = reg.Subjects[indexInt - 1];
                Console.WriteLine($"What formulation would you like to test on {testee.Name}?");
                if (reg.CreatedFormulations.Count > 0)
                    d = Pickers<Formulation>.ChooseByName(reg.CreatedFormulations);
                else
                {
                    d = Dialog.CreateFormulation(reg);
                    reg.CreatedFormulations.Add(d);
                    reg.JsonSave();
                }
                Console.Clear();
                Random rand = new Random();
                Dialog.ColouredMessage($"-- PATIENT TESTING REPORT BEGIN--\nTesting Instance : {rand.Next(0,100).GetHashCode()}\nSubject Report :", ConsoleColor.Green);
                Describe(testee);
                Dialog.ColouredMessage($"Formulation Report : ", ConsoleColor.Green);
                Describe(d);
                if (testee.AssignedFormulation.Name == d.Name)
                    Dialog.ColouredMessage($"Board Approved : {testee.DrugApproval()}", ConsoleColor.Green);        
                else
                    Dialog.ColouredMessage($"Warning : Pharmaceutical Board has not approved patient to consume this formulation, proceed with caution", ConsoleColor.Red);                            
                complete = Dialog.YesOrNo("Do you agree to this test?");
            }
            testee.Consume(d);
            Console.ReadKey();
        }
    }
    // TODO : Fix
    public static class Pickers<T>
    {
        public static int NumberInRange(int min, int max)
        {
            int result = min - 1;
            try
            {
                result = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine(result.ToString());
            }
            finally
            {
                Console.WriteLine($"Must be a numerical value between {min} and {max}");
            }
            return result;
        }
        public static T ChooseByName(List<T> options)
        {
            T result = default(T);
            bool found = false;
            if (typeof(NamedObject).IsAssignableFrom(typeof(T)))
            {
                options.ForEach(o => Console.WriteLine((o as NamedObject).Name));
                while (found == false)
                {
                    string choice = Console.ReadLine();
                    found = options.Where(f => (f as NamedObject).Name.ToLower() == choice.ToLower()).Count() > 0;
                    if (found)
                        result = options.Find(f => (f as NamedObject).Name.ToLower() == choice.ToLower());
                }
            }
            return result;
        }
    }
}