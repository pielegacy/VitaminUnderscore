using System;
using System.Collections.Generic;

namespace VitaminUnderscore
{
    /*
        Animal class
            The base for all species in VitaminUnderscore
            Anything that is an animal can be tested on
    */
    public abstract class Animal : NamedObject
    {
        public Animal(string name, int age, Dictionary<Trait, int> attributes) : base(name)
        {
            _age = age;
            _attributes = attributes;
        }
        private Dictionary<Trait, int> _attributes;
        public Dictionary<Trait, int> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        private readonly int _age;
        public int Age
        {
            get { return _age; }
        }
        private bool _living = true;
        public bool Living
        {
            get { return _living; }
            set { _living = value; }
        }

        public virtual void Consume(IConsumable consumable)
        {

        }
    }
    /*
        Human class
            Humans are species which extend animals, they possess a biography and are
            seen as the more ethical subjects
    */
    public class Human : Animal
    {
        public Human(string name, int age, string bio, Dictionary<Trait, int> attributes, List<Ingredient> allergies) : base(name, age, attributes)
        {
            _bio = bio;
            _allergies = allergies;
        }
        public Human(string name, int age) : this (name, age, "A human", GenerateAttributes(), new List<Ingredient>())
        {
        }
        private string _bio;
        public string Biography
        {
            get { return _bio;}
            set { _bio = value;}
        }
        private readonly List<Ingredient> _allergies;
        public List<Ingredient> Allergies
        {
            get { return _allergies;}
        }
        public bool AddAllergy(Ingredient allergy)
        {
            if (_allergies.Contains(allergy) == false)
                _allergies.Add(allergy);
            return !_allergies.Contains(allergy);
        }
        // Generates a random set of attributes
        public static Dictionary<Trait, int> GenerateAttributes(){
            Random rand = new Random();
            Dictionary<Trait, int> temp = new Dictionary<Trait, int>();
            temp.Add(Trait.BrainPower, rand.Next(1, 11));
            temp.Add(Trait.Endurance, rand.Next(1, 11));
            temp.Add(Trait.Immunity, rand.Next(1, 11));
            temp.Add(Trait.Strength, rand.Next(1, 11));
            temp.Add(Trait.Wellbeing, rand.Next(1, 11));
            return temp;                        
        }
        public override void Consume(IConsumable consumable)
        {
            if (consumable.GetType() == typeof(Formulation)) // Handle formulation
            {
                Formulation formula = consumable as Formulation;
                formula.Ingredients.ForEach(i => {
                    i.Effects.ForEach(e => {
                        Attributes[e.Trait] += e.Amount;
                    });
                });
            }
            else {
                Ingredient Ingredient = consumable as Ingredient;
                Ingredient.Effects.ForEach(e => {
                    Attributes[e.Trait] += e.Amount;
                });
            }
        }
    }
    /*
        Subject class
            A subject is a human who has been assigned a formulation
            and so are more content with taking it
    */
    public class Subject  : Human
    {
        public Subject(string name, int age, string bio, Dictionary<Trait, int> attributes, Formulation drug) : base (name, age, bio, attributes, new List<Ingredient>())
        {
            _assignedFormulation = drug;
        }
        public Subject(string name, int age, Formulation drug) : this (name, age, "a poor human", GenerateAttributes(), drug)
        {
        }
        private Formulation _assignedFormulation;
        public Formulation AssignedFormulation
        {
            get { return _assignedFormulation;}
            set { _assignedFormulation = value;}
        }
    }
}