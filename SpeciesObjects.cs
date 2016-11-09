using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VitaminUnderscore
{
    ///<summary>
    ///Animals are objects which can consume any object
    ///that inherits IConsumable.
    ///They have a set of traits with associated values
    ///which dictate their health and can be easily extended.
    ///</summary>
    public abstract class Animal : NamedObject
    {
        ///<param name="age">
        ///Used to define how well the animal takes consuming a
        ///a formulation if it is harmful to them.
        ///</param>
        ///<param name="attributes">
        ///Is a dictionary, each key a possible value from the Trait
        ///enumeration. The integer value is the magnitude of that trait.
        ///Note that having a negative value for any traits increases a species
        ///chance of dying from ingestion of bad formulae.`
        ///</param>
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
        ///<summary>
        ///Consuming iterates through an animals traits and 
        ///Applies the associated values to them depending on
        ///the formulation
        ///</summary>
        ///<param name="consumable">
        ///May be either a single ingredient or a formulation
        ///</param>
        public virtual void Consume(IConsumable consumable)
        {
            if (consumable.GetType() == typeof(Formulation)) // Handle formulation
            {
                Formulation formula = consumable as Formulation;
                formula.Ingredients.ForEach(i =>
                {
                    i.Effects.ForEach(e =>
                    {
                        int og = Attributes[e.Trait];
                        Attributes[e.Trait] = og + e.Amount;
                    });
                });
            }
            else
            {
                Ingredient Ingredient = consumable as Ingredient;
                Ingredient.Effects.ForEach(e =>
                {
                    Attributes[e.Trait] += e.Amount;
                });
            }
            ProcessAttributes();
        }
        protected virtual void ProcessAttributes()
        {
            Random rand = new Random();
            int affect = 0;
            File.WriteAllText("testing_output.txt", JsonConvert.SerializeObject(Attributes));
            for (int i = 0; i < Attributes.Count; i++)
            {
                Trait t = (Trait)i;
                if (Attributes[t] <= 0)
                {
                    Console.WriteLine($"That had a serious impact on {Name}'s {Attributes[t]}");
                    affect += Attributes[t];
                }
            }
            File.AppendAllText("testing_output.txt", JsonConvert.SerializeObject(Attributes));
            decimal chance = rand.Next(0, 5) * affect * Math.Floor(Convert.ToDecimal(Age) / rand.Next(1, 11));
            Living = chance > rand.Next(-120, -90);
        }
    }
    ///<summary>
    ///Humans are animals which have a background story (bio)
    ///and are generally considered more 'ethical' to test on 😉
    ///</summary>
    public class Human : Animal
    {
        ///<param name="name">
        ///The name of the entity
        ///</param>
        ///<param name="age">
        ///The age of the subject, may affect medicine consumption and health
        ///</param>
        ///<param name="bio">
        ///Short description of the subject
        ///Displayed in their summary
        ///</param>
        ///<param name="attributes">
        ///Attributes are used by the system to
        ///calculate a subjects weaknesses and such.
        ///Can be generated using GenerateAttributes()
        ///</param>
        public Human(string name, int age, string bio, Dictionary<Trait, int> attributes, List<Ingredient> allergies) : base(name, age, attributes)
        {
            _bio = bio;
            _allergies = allergies;
            GenerateFinances();
        }
        public Human(string name, int age) : this(name, age, "", GenerateAttributes(), new List<Ingredient>())
        {
            Biography = GenerateBiography();
        }
        private string _bio;
        public string Biography
        {
            get { return _bio; }
            set { _bio = value; }
        }
        private double _money;
        public double Money
        {
            get { return _money; }
            set { _money = value; }
        }
        private readonly List<Ingredient> _allergies;
        public List<Ingredient> Allergies
        {
            get { return _allergies; }
        }
        public bool AddAllergy(Ingredient allergy)
        {
            if (_allergies.Contains(allergy) == false)
                _allergies.Add(allergy);
            return !_allergies.Contains(allergy);
        }
        // Generates a random set of attributes
        public static Dictionary<Trait, int> GenerateAttributes(bool moneyInclusive = false)
        {
            Random rand = new Random();
            Dictionary<Trait, int> temp = new Dictionary<Trait, int>();
            temp.Add(Trait.BrainPower, rand.Next(1, 11));
            temp.Add(Trait.Endurance, rand.Next(1, 11));
            temp.Add(Trait.Immunity, rand.Next(1, 11));
            temp.Add(Trait.Strength, rand.Next(1, 11));
            temp.Add(Trait.Wellbeing, rand.Next(1, 11));
            return temp;
        }
        protected void GenerateFinances()
        {
            Random rand = new Random();
            double total = 0.00;
            for (int i = 0; i < 5; i++)
            {
                Trait t = (Trait)i;
                if (Attributes[t] > 0)
                    total += rand.Next(1, 4) * Attributes[t];
            }
            Money += total;
        }
        // Generate Biography based on attributes
        public string GenerateBiography()
        {
            Random rand = new Random();
            List<string> PositiveDescriptions = new List<string>(){
            "impeccable",
            "great",
            "excellent",
            "remarkable"
        };
            List<string> NegativeDescriptions = new List<string>()
        {
            "terrible",
            "unhealthly",
            "shitty",
            "awful"
        };
            string _res = "A human. ";
            int _greatestValue = Attributes.Values.Max();
            Trait? _greatestTrait = null;
            int _worstValue = Attributes.Values.Min();
            Trait? _worstTrait = null;
            Attributes.Keys.ToList().ForEach(a =>
            {
                if (Attributes[a] == _greatestValue)
                    _greatestTrait = a;
                if (Attributes[a] == _worstValue)
                    _worstTrait = a;
            });
            if (_greatestTrait == null || _worstTrait == null)
                _res += "Somehow not great at much";
            else
                _res += $"Has {PositiveDescriptions[rand.Next(0, PositiveDescriptions.Count)]} {_greatestTrait} however {NegativeDescriptions[rand.Next(0, NegativeDescriptions.Count)]} {_worstTrait}.";
            return _res;
        }
        ///<summary>
        ///Consume is the method which is called whenever an Animal or its children classes
        ///takes one of the formulae presented. It Can be extended upon for additional functionality
        ///however its base functionality basically checks for allergies and applies the affects
        ///of the vitamin.
        ///</summary>
        public override void Consume(IConsumable consumable)
        {
            if (consumable.GetType() == typeof(Formulation)) // Handle formulation
            {
                Formulation formula = consumable as Formulation;
                formula.Ingredients.ForEach(i =>
                {
                    AllergicReaction(i);
                    i.Effects.ForEach(e =>
                    {
                        int og = Attributes[e.Trait];
                        Attributes[e.Trait] = og + e.Amount;
                    });
                });
            }
            else
            {
                Ingredient Ingredient = consumable as Ingredient;
                Ingredient.Effects.ForEach(e =>
                {
                    Attributes[e.Trait] += e.Amount;
                });
            }
            ProcessAttributes();
        }
        // Handle allergies
        protected virtual bool AllergicReaction(Ingredient substance)
        {
            bool res = false;
            if (Allergies.Contains(substance))
            {
                Dialog.WarningMessage($"It appears that {Name} is having an allergic reaction to {substance.Name}");
                res = true;
            }
            return res;
        }
    }

    ///<summary>
    ///A subject is a human who has been assigned a formulation
    ///and so are more content with taking it
    ///</summary>
    public class Subject : Human
    {
        [JsonConstructorAttribute]
        public Subject(string name, int age, string bio, Dictionary<Trait, int> attributes, Formulation drug) : base(name, age, bio, attributes, new List<Ingredient>())
        {
            _assignedFormulation = drug;
            Biography = GenerateBiography();
        }
        public Subject(string name, int age, Formulation drug) : this(name, age, "a poor human", GenerateAttributes(), drug)
        {
        }
        ///<param name="drug">
        ///Their assigned Formulation
        ///</param>
        public Subject(Formulation drug) : this(GetRandomName().Result, GetRandomAge(), "", GenerateAttributes(), drug)
        {
        }
        private List<string> _notes = new List<string>();
        private void AddNote(string note)
        {
            if (!_notes.Contains(note))
                _notes.Add(note);
        }
        private Formulation _assignedFormulation;
        public Formulation AssignedFormulation
        {
            get { return _assignedFormulation; }
            set { _assignedFormulation = value; }
        }
        protected override bool AllergicReaction(Ingredient substance)
        {
            bool res = base.AllergicReaction(substance);
            if (res == true)
                _notes.Add($"Had a severe reaction to {substance.Name}");
            return res;
        }
        /// <summary>
        /// Checks to see what the possible harm on the subject would be if they took their
        /// Prescribed Formulation
        /// </summary>
        public bool DrugApproval()
        {
            Subject projection = this;
            projection.Consume(AssignedFormulation);
            return projection.Attributes[Trait.BrainPower] > 0 && projection.Attributes[Trait.Endurance] > 0 && projection.Attributes[Trait.Immunity] > 0 && projection.Attributes[Trait.Strength] > 0 && projection.Attributes[Trait.Wellbeing] > 0;
        }
        // Create a random name using the namey name database
        // Can't figure out how to get a surname from the database so I chuck
        // a random suffix on the end
        static async Task<string> GetRandomName()
        {
            Random rand = new Random();
            List<string> surnameSuffixes = new List<string>()
            {
                "son",
                "vic",
                "ton",
                "en",
                "er",
                "ly",
                "ge",
                "and"
            };
            using (HttpClient web = new HttpClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<string[]>(await web.GetStringAsync("http://namey.muffinlabs.com/name.json"))[0] + " " + JsonConvert.DeserializeObject<string[]>(await web.GetStringAsync("http://namey.muffinlabs.com/name.json"))[0] + surnameSuffixes[rand.Next(0, surnameSuffixes.Count - 1)];
                }
                catch
                {
                    return "Not Connected" + surnameSuffixes[rand.Next(0, surnameSuffixes.Count - 1)];
                }
            }
        }
        static int GetRandomAge()
        {
            Random rand = new Random();
            return rand.Next(18, 100);
        }
    }
    ///<summary>
    ///A scientist is required for the game to work, without one you lose
    ///</summary>
    public class Scientist : Human
    {
        public Scientist(string name, int age, bool main = false) : base(name, age)
        {
            _isMain = main;
            Money = 1000.00;
        }
        private bool _isMain;
        public bool MainPharmacist
        {
            get { return _isMain; }
        }
    }
}