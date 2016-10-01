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
        private const string EffectsRepo = @"http://api.myjson.com/bins/2sd7s";
        private const string IngredientsRepo = @"https://api.myjson.com/bins/2b7xk";
        // Every new game is a fresh save
        private const bool newGameDefault = true;
        // Adds individual ingredients
        public GameRegistry()
        {
            if (newGameDefault && Directory.Exists("SaveData")){
                List<string> fileNames = Directory.EnumerateFiles("SaveData").ToList();
                fileNames.ForEach(f => File.Delete(f));
                Directory.Delete("SaveData");
            }
        }
        public List<Ingredient> Ingredients = new List<Ingredient>();
        public List<Effect> Effects = new List<Effect>(){
        };
        public List<Formulation> CreatedFormulations = new List<Formulation>();
        public List<Animal> Subjects = new List<Animal>();
        public List<Effect> RetEffects(string[] names)
        {
            List<Effect> results = new List<Effect>();
            for (int i = 0; i < names.Length; i++)
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
            File.WriteAllText("SaveData/ingredients.json", JsonConvert.SerializeObject(Ingredients));
            File.WriteAllText("SaveData/effects.json", JsonConvert.SerializeObject(Effects));
            if (CreatedFormulations.Count > 0)
                File.WriteAllText("SaveData/formulations.json", JsonConvert.SerializeObject(CreatedFormulations));
            if (Subjects.Count > 0)
                File.WriteAllText("SaveData/subjects.json", JsonConvert.SerializeObject(Subjects));
        }
        public async void JsonLoad(bool verbose = false)
        {
            if (Directory.Exists("SaveData"))
            {
                if (verbose)
                    Dialog.MessageLoad();
                Ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(File.ReadAllText("SaveData/ingredients.json"));
                Effects = JsonConvert.DeserializeObject<List<Effect>>(File.ReadAllText("SaveData/effects.json"));
                if (File.Exists("SaveData/formulations.json"))
                    CreatedFormulations = JsonConvert.DeserializeObject<List<Formulation>>(File.ReadAllText("SaveData/formulations.json"));
                if (File.Exists("SaveData/subjects.json"))
                    Subjects = JsonConvert.DeserializeObject<List<Animal>>(File.ReadAllText("SaveData/subjects.json"));
                if (verbose)
                    Dialog.MessageDone();
            }
            else // Download default data from the internet if there isn't any 
            {
                Directory.CreateDirectory("SaveData");
                List<Effect> downloadedEffects = await DownloadEffects();
                if (downloadedEffects != null)
                    Effects = downloadedEffects;
                List<Ingredient> downloadedIngredients = await DownloadIngredients();
                if (downloadedIngredients != null)
                    Ingredients = downloadedIngredients;
            }
        }
        // Download JSON for default effects list from the internet
        static async Task<List<Effect>> DownloadEffects()
        {
            List<Effect> effectsTemp = null;
            using (HttpClient web = new HttpClient())
            {
                HttpResponseMessage response = await web.GetAsync(EffectsRepo);
                if (response.IsSuccessStatusCode)
                    effectsTemp = JsonConvert.DeserializeObject<List<Effect>>(await response.Content.ReadAsStringAsync());
            };
            return effectsTemp;
        }
        // Do the same for ingredients
        static async Task<List<Ingredient>> DownloadIngredients()
        {
            List<Ingredient> ingredientsTemp = null;
            using (HttpClient web = new HttpClient())
            {
                HttpResponseMessage response = await web.GetAsync(IngredientsRepo);
                if (response.IsSuccessStatusCode)
                    ingredientsTemp = JsonConvert.DeserializeObject<List<Ingredient>>(await response.Content.ReadAsStringAsync());
            }
            return ingredientsTemp;
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
    }
}