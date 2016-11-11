using System;
using System.Collections.Generic;

namespace VitaminUnderscore
{
    /*
        Ingredient
            Ingredients are an aggrgation of effects, used to create a Formulation
    */
    public class Ingredient : NamedObject, IConsumable
    {
        public Ingredient(string name, IngredientType type, List<Effect> effects) : base(name)
        {
            _type = type;
            _effects = effects;
        }
        private readonly IngredientType _type;
        public IngredientType Type
        {
            get { return _type; }
        }
        private readonly List<Effect> _effects;
        public List<Effect> Effects
        {
            get { return _effects; }
        }


    }
    /*
        Effect
            Effects are tied to a trait of a species, will exert such
            effect when consumed
    */
    public class Effect : NamedObject
    {
        public Effect(string name, Trait trait, int amount) : base(name)
        {
            _trait = trait;
            _amount = amount;
        }
        private Trait _trait;
        public Trait Trait
        {
            get { return _trait; }
        }
        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
    }
    /*
        Formulation
            A formulation is a collection of ingredients,
            these are taken by subjects
    */
    public class Formulation : NamedObject, IConsumable
    {
        public Formulation(string name, List<Ingredient> ingredients, IngredientType type) : base(name)
        {
            _ingredients = ingredients;
            _type = type;
        }
        private List<Ingredient> _ingredients;
        public List<Ingredient> Ingredients
        {
            get { return _ingredients;}
        }
        private readonly IngredientType _type;
        public IngredientType Type
        {
            get { return _type;}
        }
        public double Cost
        {
            get {
                double res = 0.00;
                _ingredients.ForEach(i => {
                    double ingCost = 0.00;
                    i.Effects.ForEach(e => {
                        ingCost += e.Amount * 5 *  i.Effects.Count;
                    });
                    res += ingCost;
                });
                return res;
            }
        }
        
    }
}