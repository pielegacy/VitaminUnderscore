using System;
using System.Net.Http;
/*
Vitamin _ : a Vitamin Development Simulation
Created by Alex Billson for Object Oriented Programming
at Swinburne University of Technology
*/
namespace VitaminUnderscore
{
    public class Program
    {
        public static GameRegistry reg = new GameRegistry();

        public static void Main(string[] args)
        {
            Console.Title = "Vitamin _";
            //TODO: Actually implement this
            // using (HttpClient web = new HttpClient())
            // {
            //     try
            //     {
            //         web.GetAsync("http://bing.com");
            //     }
            //     catch
            //     {
            //         Dialog.WarningMessage("-- Warning : Internet Connectivity Required for Full Functionality, some functions may not work for this session --");
            //     }
            // }
            while (Dialog.MainMenu(reg) == false)
                Console.Clear();
        }
    }
}
