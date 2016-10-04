using System;

namespace VitaminUnderscore
{
    ///<summary>
    ///The base class for all interactable objects in the game
    ///</summary>
    public class NamedObject
    {
        ///<param name='name'>
        ///The publicly accessible, should be unique
        ///as the game works off the name being
        ///a 'key' for the NamedObject
        ///</param>
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