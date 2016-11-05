using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace VitaminUnderscore
{
    public class GameRegistry
    {
        /*
            Default Configurations for the JSON files
        */
        private const string EffectsRepo = @"https://api.myjson.com/bins/4nsu8";
        private const string IngredientsRepo = @"https://api.myjson.com/bins/2c7kw";
        // Every new game is a fresh save
        private const bool newGameDefault = false;
        // Adds individual ingredients
        public GameRegistry()
        {
            if (newGameDefault && Directory.Exists("SaveData"))
            {
                List<string> fileNames = Directory.EnumerateFiles("SaveData").ToList();
                fileNames.ForEach(f => File.Delete(f));
                Directory.Delete("SaveData");
            }
        }
        public List<Ingredient> Ingredients = new List<Ingredient>();
        public List<Effect> Effects = new List<Effect>()
        {
        };
        public List<Formulation> CreatedFormulations = new List<Formulation>();
        public List<Subject> Subjects = new List<Subject>();
        public List<Scientist> Pharmacists = new List<Scientist>();
        public List<Effect> RetEffects(string[] names)
        {
            List<Effect> results = new List<Effect>();
            for (int i = 0; i < names.Length; i++)
                if (Effects.Find(e => e.Name.ToLower() == names[i].ToLower()) != null)
                    results.Add(Effects.Find(e => e.Name.ToLower() == names[i].ToLower()));
            return results;
        }
        public Ingredient RetIngredient(string name)
        {
            return Ingredients.Find(i => i.Name.ToLower() == name.ToLower());
        }
        public List<Ingredient> RetIngredientList(string[] name)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            foreach (string n in name)
                ingredients.Add(RetIngredient(n));
            return ingredients;
        }
        // Load and save game data from JSON
        public void JsonSave()
        {
            if (!Directory.Exists("SaveData"))
                Directory.CreateDirectory("SaveData");
            if (Ingredients.Count <= 0 || Effects.Count <= 0)
                JsonLoad(false, true);
            File.WriteAllText("SaveData/ingredients.json", JsonConvert.SerializeObject(Ingredients));
            File.WriteAllText("SaveData/effects.json", JsonConvert.SerializeObject(Effects));
            File.WriteAllText("SaveData/pharmacists.json", JsonConvert.SerializeObject(Pharmacists));
            if (CreatedFormulations.Count > 0)
                File.WriteAllText("SaveData/formulations.json", JsonConvert.SerializeObject(CreatedFormulations));
            if (Subjects.Count > 0)
                File.WriteAllText("SaveData/subjects.json", JsonConvert.SerializeObject(Subjects));
        }
        public async void JsonLoad(bool verbose = false, bool forceDownload = false, string custEff = "", string custIng = "")
        {
            if (Directory.Exists("SaveData") && !forceDownload)
            {
                Ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(File.ReadAllText("SaveData/ingredients.json"));
                Effects = JsonConvert.DeserializeObject<List<Effect>>(File.ReadAllText("SaveData/effects.json"));
                if (File.Exists("SaveData/formulations.json"))
                    CreatedFormulations = JsonConvert.DeserializeObject<List<Formulation>>(File.ReadAllText("SaveData/formulations.json"));
                if (File.Exists("SaveData/subjects.json"))
                    Subjects = JsonConvert.DeserializeObject<List<Subject>>(File.ReadAllText("SaveData/subjects.json"));
                if (File.Exists("SaveData/pharmacists.json"))
                    Pharmacists = JsonConvert.DeserializeObject<List<Scientist>>(File.ReadAllText("SaveData/pharmacists.json"));
                if (verbose)
                    Dialog.MessageDone();
                if (Ingredients.Count == 0 || Effects.Count == 0)
                    JsonLoad(false, true);
            }
            else // Download default data from the internet if there isn't any 
            {
                Directory.CreateDirectory("SaveData");
                List<Effect> downloadedEffects = await DownloadEffects(custEff);
                if (downloadedEffects != null)
                    Effects = downloadedEffects;
                List<Ingredient> downloadedIngredients = await DownloadIngredients(custIng);
                if (downloadedIngredients != null)
                    Ingredients = downloadedIngredients;

            }
        }
        // Download JSON for default effects list from the internet
        static async Task<List<Effect>> DownloadEffects(string custEffect = "")
        {
            List<Effect> effectsTemp = null;
            string url = EffectsRepo;
            while (effectsTemp == null)
            {
                using (HttpClient web = new HttpClient())
                {
                    HttpResponseMessage response;
                    if (custEffect != "")
                        url = custEffect;
                    response = await web.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                        effectsTemp = JsonConvert.DeserializeObject<List<Effect>>(await response.Content.ReadAsStringAsync());
                };
            }
            return effectsTemp;
        }
        // Do the same for ingredients
        static async Task<List<Ingredient>> DownloadIngredients(string custIng = "")
        {
            List<Ingredient> ingredientsTemp = null;
            string url = IngredientsRepo;
            while (ingredientsTemp == null)
            {
                using (HttpClient web = new HttpClient())
                {
                    HttpResponseMessage response;
                    if (custIng != "")
                        url = custIng;
                    response = await web.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                        ingredientsTemp = JsonConvert.DeserializeObject<List<Ingredient>>(await response.Content.ReadAsStringAsync());
                }
            }
            return ingredientsTemp;
        }
        // Property for Main Player
        public Scientist Player
        {
            get
            {
                return Pharmacists[0];
            }
        }
        // For use with the entity framework
        public void DatabaseLoad()
        {
            Effects = TransferralClass.EffectFromDb();
            Console.WriteLine("Loaded effects");
            Ingredients = TransferralClass.IngredientsFromDb();
        }
        public void DatabaseSave()
        {
            Console.WriteLine("Adding...");
            TransferralClass.EffectsToDb(Effects);
            TransferralClass.IngredientsToDb(Ingredients);
        }
        ///<summary>
        ///Clear the Save Data of the program
        ///</summary>
        public void SaveDataClear()
        {
            while (Directory.GetFiles("SaveData").Count() > 1)
            {
                Directory.GetFiles("SaveData").ToList().ForEach(f =>
                {
                    File.Delete(f);
                });
            }
            Directory.Delete("SaveData");
        }
        public List<string> PositiveDescriptions = new List<string>(){
            "impeccable",
            "great",
            "excellent",
            "remarkable"
        };
        public List<string> NegativeDescriptions = new List<string>()
        {
            "terrible",
            "unhealthly",
            "shitty",
            "awful"
        };
        public string DescGood
        {
            get
            {
                Random rand = new Random();
                return PositiveDescriptions[rand.Next(0, PositiveDescriptions.Count)];
            }
        }
        public string DescBad
        {
            get
            {
                Random rand = new Random();
                return NegativeDescriptions[rand.Next(0, NegativeDescriptions.Count)];
                
            }
        }
    }
}