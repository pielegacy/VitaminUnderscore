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
        public static void AddEffect(IngredientsRegistry reg)
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
    }
}