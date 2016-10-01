using System;
using System.Collections.Generic;
using System.Linq;
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
            while (Dialog.MainMenu(reg) == false)
                Console.Clear();
        }
    }
}
