using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        public Human(string name, int age) : this(name, age, "A human", GenerateAttributes(), new List<Ingredient>())
        {
        }
        private string _bio;
        public string Biography
        {
            get { return _bio; }
            set { _bio = value; }
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
        public static Dictionary<Trait, int> GenerateAttributes()
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
                        //Console.WriteLine($"{e.Trait} : {og} -> {Attributes[e.Trait]}");
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
        protected virtual void ProcessAttributes()
        {
            Random rand = new Random();
            int affect = 0;
            for (int i = 0; i < Attributes.Count; i++)
            {
                Trait t = (Trait)i;
                if (Attributes[t] <= 0)
                {
                    Console.WriteLine($"That had a serious impact on {Name}'s {Attributes[t]}");
                    affect += Attributes[t];
                }
            }
            decimal chance = rand.Next(0, 5) * affect * Math.Floor(Convert.ToDecimal(Age) / rand.Next(1,11));
            Console.WriteLine($"{chance}");
            Living = chance > rand.Next(-120, -90);
        }
    }

    ///<summary>
    ///A subject is a human who has been assigned a formulation
    ///and so are more content with taking it
    ///</summary>
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
    ///<param name="drug">
    ///Their assigned Formulation
    ///</param>
    public class Subject : Human
    {
        [JsonConstructorAttribute]
        public Subject(string name, int age, string bio, Dictionary<Trait, int> attributes, Formulation drug) : base(name, age, bio, attributes, new List<Ingredient>())
        {
            _assignedFormulation = drug;
        }
        public Subject(string name, int age, Formulation drug) : this(name, age, "a poor human", GenerateAttributes(), drug)
        {
        }
        public Subject(Formulation drug) : this(GetRandomName().Result, GetRandomAge(), "A poor test subject", GenerateAttributes(), drug)
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
                return JsonConvert.DeserializeObject<string[]>(await web.GetStringAsync("http://namey.muffinlabs.com/name.json"))[0] + " " + JsonConvert.DeserializeObject<string[]>(await web.GetStringAsync("http://namey.muffinlabs.com/name.json"))[0] + surnameSuffixes[rand.Next(0, surnameSuffixes.Count - 1)];
            }
        }
        static int GetRandomAge()
        {
            Random rand = new Random();
            return rand.Next(18, 100);
        }
    }
}