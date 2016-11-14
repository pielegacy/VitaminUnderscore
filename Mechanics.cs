using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace VitaminUnderscore
{
    ///<summary>
    ///Dialog Class
    /// Contains definitions for functions that are used by the game for
    /// - General conversations
    /// - Major Gameplay Elements
    // - Presentation functions
    ///</summary>
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
        // Format double into $00.00
        public static string Dollar(double money)
        {
            return $"${money:F}";
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
                Console.WriteLine($"-- {described.Name} --\nType: {described.Type}\nDevelopment Cost: ${described.Cost}\nIngredients: ");
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
                Console.WriteLine($"-- {described.Name} --\nAge: {described.Age}\nCurrent Wealth : {Dialog.Dollar(described.Money)}\nDescription: {described.Biography}\nAssigned Formulation : {described.AssignedFormulation.Name}\n");
            }
            if (obj.GetType() == typeof(Scientist))
            {
                Scientist described = obj as Scientist;
                Console.WriteLine($"-- {described.Name} --\nAge: {described.Age}\nCurrent Wealth : {Dialog.Dollar(described.Money)}\nDescription: {described.Biography}\n");
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
                if (reg.Player != null && reg.Player.Living)
                {
                    ColouredMessage($"Net Worth : ${reg.Pharmacists.Sum(p => p.Money):0.00}", ConsoleColor.Green);
                    HelpOptions.ForEach(h => Console.WriteLine(h.ToString()));
                    string commandString = Console.ReadLine();
                    switch (commandString.Split(' ')[0].ToLower())
                    {
                        case "custom":
                            Console.Clear();
                            LoadCustomCampaign(reg);
                            break;
                        case "1":
                            Formulation created = CreateFormulation(reg);
                            if (created != null)
                                reg.CreatedFormulations.Add(created);
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
                        case "6":
                            if (Dialog.RemoveFormulation(reg))
                            {
                                Console.WriteLine("Successfully Removed Forumalae");
                                reg.JsonSave();
                                Console.ReadKey();
                            }
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
                                if (commandString.Split(' ').Length > 1 && commandString.Split(' ')[1].ToLower() == "dump")
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
                        case "51":
                        case "#text":
                            Console.WriteLine("Paste JSON");
                            string jsonText = Console.ReadLine();
                            DeveloperDialog.AddIngredientFromText(reg, jsonText);
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
                    WarningMessage("You died, how unfortunate.\nPress any key to finish your game");
                    exit = true;
                    Console.ReadKey();
                    reg.SaveDataClear();
                }
            }
            else
            {
                Console.WriteLine("-- No employees detected, running employment tool now --");
                reg.Pharmacists.Add(Dialog.CreateScientist(true));
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
            "6) Remove Formulation",
            "0) Close Game",
            "100) Save Game",
            "101) Load Game"
        };
        ///<summary>
        ///Load Custom game files from internet using a JSON hosting service
        ///</summary>
        ///<param name="reg">
        ///The registry to which the game will save the new campaign data
        ///</param>
        public static void LoadCustomCampaign(GameRegistry reg)
        {
            Console.WriteLine("Please enter the url for an effects.json");
            string effURL = Console.ReadLine();
            Console.WriteLine("Please enter the url for an ingredients.json");
            string ingURL = Console.ReadLine();
            reg.JsonLoad(false, true, effURL, ingURL);
            Console.WriteLine("Custom campaign loaded, ensure you run the save command");
            Console.ReadKey();
        }
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
                            HelpMessage("--Type the name of the Ingredient to add it to the formulation, type list to see ingredients, type done when you are finished--");
                        if (reg.RetIngredient(currentIngredient) != null)
                            ingredients.Add(reg.RetIngredient(currentIngredient));
                        else
                            if (currentIngredient != "")
                            WarningMessage("Invalid ingredient name, type list to see available ingredients");
                    }
                    else
                        reg.Ingredients.ForEach(r => Console.WriteLine($"- {r.Name}"));
                    currentIngredient = Console.ReadLine();
                }
                string currentType = "";
                if (ingredients.Count == 0)
                    Console.WriteLine("Well would you look at that, it's got nothing in it");
                HelpMessage("Is this a compound of 1) Vitamins 2) Minerals 3) Both ?");
                while (currentType != "1" && currentType != "2" && currentType != "3")
                    currentType = Console.ReadLine();
                IngredientType type = IngredientType.Other;
                type = (IngredientType)Convert.ToInt16(currentType) - 1;
                newForm = new Formulation(name, ingredients, type);
                Describe(newForm);
                if (newForm.Cost > reg.Player.Money)
                {
                    WarningMessage("You do not have enough money to create this formulation");
                    newForm = null;
                    complete = Dialog.YesOrNo("Continue making a formulae?");
                }
                else
                {
                    complete = Dialog.YesOrNo();
                    if (complete)
                        reg.Player.Money -= newForm.Cost;
                }
            }
            return newForm;
        }
        public static bool RemoveFormulation(GameRegistry reg)
        {
            bool result = false;
            Console.Clear();
            Dialog.ColouredMessage("-- Official Request for Pharmaceutical Recall --\nCreated Forumalae:", ConsoleColor.Red);
            Dialog.ColouredMessage("-- Please write the name of the formulation to be recalled --", ConsoleColor.Red);
            Formulation recalled = Pickers<Formulation>.ChooseByName(reg.CreatedFormulations);
            Dialog.Describe(recalled);
            result = Dialog.YesOrNo($"Are you sure you want to remove {recalled.Name} from the market?");
            if (result)
            {
                reg.CreatedFormulations.Remove(recalled);
            }
            return result;
        }
        public static bool YesOrNo(string prompt = "Is this correct?")
        {
            Console.WriteLine($"{prompt} (y/n)");
            string choice = Console.ReadLine();
            while (choice.ToLower().ToCharArray()[0] != 'y' && choice.ToLower().ToCharArray()[0] != 'n')
            {
                Console.WriteLine("--Please answer Yes or No--");
                choice = Console.ReadLine();
            }
            return choice.ToLower().ToCharArray()[0] == 'y';
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
                newSubject = new Subject(formulation);
                Describe(newSubject);
                Random rand = new Random();
                complete = Dialog.YesOrNo();
            }
            return newSubject;
        }
        public static Scientist CreateScientist(bool isMain = false)
        {
            Scientist sci = null;
            bool done = false;
            while (!done)
            {
                Dialog.ColouredMessage("-- Vitasys Employment System --\nWelcome to the Vitasys Alternative Pharmaceutical Company", ConsoleColor.Cyan);
                Dialog.ColouredMessage("Please enter you full name:", ConsoleColor.Cyan);
                string name = Console.ReadLine();
                while (name == "")
                {
                    // What an easter egg tho
                    Random rand = new Random();
                    int chance = rand.Next(0, 100);
                    if (chance > 80)
                        Dialog.WarningMessage("You have no name? That's cooked as aye");
                    name = Console.ReadLine();
                }
                Dialog.ColouredMessage("How old are you?:", ConsoleColor.Cyan);
                string ageString = Console.ReadLine();
                int age = 0;
                if (!Int32.TryParse(ageString, out age))
                {
                    Console.WriteLine("Oh you think you're smart aye? That's it, you can be a big baby.");
                    Dialog.ColouredMessage("-- Age set to 3 years old --", ConsoleColor.Green);
                    age = 3;
                }
                sci = new Scientist(name, age);
                sci.Biography = "A clever person obviously";
                Dialog.Describe(sci);
                done = Dialog.YesOrNo();
            }
            return sci;
        }
        ///<summary>
        /// Display decision dialog for testing subjects
        ///</summary>
        public static void TestSubject(GameRegistry reg)
        {
            bool complete = false;
            Formulation d = null;
            Animal testee = null;
            Random rand = new Random();
            while (!complete)
            {
                Console.Clear();
                if (reg.Subjects.Count > 0)
                {
                    ColouredMessage("-- Subject Testing Framework v1.420 --\nPlease select a Subject by index:", ConsoleColor.Yellow);
                    for (int i = 0; i < reg.Subjects.Count; i++)
                    {
                        if (reg.Subjects[i].Living)
                            Console.WriteLine($"{i + 1}) {reg.Subjects[i].Name}");
                    }
                    int indexInt = 0;
                    try
                    {
                        indexInt = Convert.ToInt32(Console.ReadLine());
                        if (reg.Subjects[0] != null)
                            testee = reg.Subjects[indexInt - 1] as Subject;
                    }
                    // Invalid index will result in the first testee being used, if
                    // this is not possible. The main player will be used
                    catch (System.Exception)
                    {
                        Console.WriteLine("Invalid index, using default testee");
                        if (reg.Subjects[0] != null)
                            testee = reg.Subjects[0];
                    }
                }
                else
                {
                    ColouredMessage("Oh, it appears you do not have any subjects available for testing?", ConsoleColor.Cyan);
                    if (YesOrNo("Would you like to test a formulation on yourself?") == false)
                        break;
                    else
                        testee = reg.Player;
                }
                if (testee != null)
                {
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
                    Dialog.ColouredMessage($"-- PATIENT TESTING REPORT BEGIN--\nTesting Instance : {rand.Next(0, 100).GetHashCode()}\nSubject Report :", ConsoleColor.Green);
                    Describe(testee);
                    Dialog.ColouredMessage($"Formulation Report : ", ConsoleColor.Green);
                    Describe(d);
                    if (testee.GetType() == typeof(Subject))
                    {
                        Subject subj = testee as Subject;
                        if (subj.AssignedFormulation.Name == d.Name)
                            Dialog.ColouredMessage($"Board Approved : {subj.DrugApproval()}", ConsoleColor.Green);
                        else
                            Dialog.ColouredMessage($"Warning : Pharmaceutical Board has not approved patient to consume this formulation, proceed with caution", ConsoleColor.Red);
                    }

                    complete = Dialog.YesOrNo("Do you agree to this test?");
                }
                if (complete && testee != null && d != null)
                {
                    testee.Consume(d);
                    if (testee.Living == false && testee.GetType() == typeof(Subject))
                    {
                        reg.Pharmacists.First().Money += (testee as Subject).Money;
                        Console.WriteLine($"Oh, looks like {testee.Name} passed away. They left you behind {Dialog.Dollar((testee as Subject).Money)} thought.");
                        reg.Subjects.Remove(testee as Subject);
                    }
                    else
                    {
                        if (testee.GetType() == typeof(Subject))
                        {
                        double reward = rand.Next(1, 4) * (testee as Subject).Money;
                        Dialog.ColouredMessage($"Testing successful, the Board has taken note of this success and supplied you a cut of ${reward} for your efforts", ConsoleColor.Green);
                        reg.Player.Money += reward;
                        }
                        else
                            Dialog.ColouredMessage($"Testing successful", ConsoleColor.Green);
                    }
                }
                Console.ReadKey();
            }
        }
        ///<summary>
        /// Charge all Pharmacists for a purchase
        ///</summary>
        public static bool Charge(GameRegistry reg, double amount)
        {
            double amt = amount;
            bool success = amount >= reg.Pharmacists.Sum(p => p.Money);
            if (success)
            {
                List<Scientist> PharmacistOrdered = reg.Pharmacists.OrderBy(p => p.Money).ToList();
                PharmacistOrdered.ForEach(p =>
                {
                    amt -= p.Money;
                    p.Money = 0;
                    if (amt < 0)
                    {
                        p.Money = amt * -1;
                        amt = 0;
                    }
                });
            }
            return success;
        }
    }
    ///<summary>
    ///Used for isntances when a player has to choose something
    ///</summary>
    public static class Pickers<T>
    {
        ///<summary>
        ///Provides the necessary dialog and then asks a player
        ///to an option from the list by its name
        ///</summary>
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
                    else
                        Dialog.WarningMessage("Invalid Choice, please type the name of your selection");
                }
            }
            return result;
        }
    }
}