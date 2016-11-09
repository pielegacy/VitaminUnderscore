
using System.Collections.Generic;
using System.Linq;

namespace VitaminUnderscore
{
    // Used for transferring objects to EF compatible models
    public static class TransferralClass
    {
        // // Initialize database context
        // static VitaminDb db = new VitaminDb();
        // // Adds a list of ingredients to the database
        // public static void IngredientsToDb(List<Ingredient> ingredients)
        // {
        //     ingredients.ForEach(i => {
        //         IngredientData ing = new IngredientData();
        //         ing.Name = i.Name;
        //         ing.Type = i.Type;
        //         ing.Effects = DataFromList(i.Effects);
        //         db.Ingredients.Add(ing);
        //     });
        //     db.SaveChanges();
        // }
        // public static List<Ingredient> IngredientsFromDb()
        // {
        //     List<Ingredient> result = new List<Ingredient>();
        //     db.Ingredients.ToList().ForEach(i => {
        //         result.Add(new Ingredient(i.Name, i.Type, EffectsFromList(i.Effects)));
        //     });
        //     return result;
        // }
        // public static void EffectsToDb(List<Effect> effects)
        // {
        //     effects.ForEach(e => {
        //         EffectData eff = new EffectData();
        //         eff.Name = e.Name;
        //         eff.Trait = e.Trait;
        //         eff.Amount = e.Amount;
        //         db.Effects.Add(eff);
        //     });
        //     db.SaveChanges();
        // }
        // public static List<Effect> EffectFromDb()
        // {
        //     List<Effect> result = new List<Effect>();
        //     db.Effects.ToList().ForEach(e => {
        //         result.Add(new Effect(e.Name, e.Trait, e.Amount));
        //     });
        //     return result;
        // }
        // public static List<EffectData> DataFromList(List<Effect> effects)
        // {
        //     List<EffectData> result = new List<EffectData>();
        //     effects.ForEach(e => {
        //         result.Add(db.Effects.ToList().Find(d => d.Name == e.Name));
        //     });
        //     return result;
        // }
        // public static List<Effect> EffectsFromList(List<EffectData> effects)
        // {
        //     List<Effect> result = new List<Effect>();
        //     effects.ForEach(e => {
        //         result.Add(new Effect(e.Name, e.Trait, e.Amount));
        //     });
        //     return result;
        // }
        // private static List<Effect> RetEffectsDb(string[] names)
        // {
        //     List<Effect> results = new List<Effect>();
        //     List<Effect> data = EffectsFromList(db.Effects.ToList());
        //     for (int i = 0; i < names.Length; i++)
        //         results.Add(data.Find(e => e.Name.ToLower() == names[i].ToLower()));
        //     return results;
        // }
    }
}