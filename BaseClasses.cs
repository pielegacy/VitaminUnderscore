using System;

namespace VitaminUnderscore
{
    public class NamedObject
    {
        public NamedObject(string name)
        {
            _name = name;
        }
        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

    }
    public enum IngredientType
    {
        Vitamin = 0,
        Mineral,
        Other
    }
    public enum Trait
    {
        Strength = 0,
        Endurance,
        Immunity,
        BrainPower,
        Wellbeing
    }
    public interface IConsumable
    {
        IngredientType Type {get;}
    }
}