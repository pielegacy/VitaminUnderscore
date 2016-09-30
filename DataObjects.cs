using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VitaminUnderscore
{
    /*
        TODO : Fix list of effect data in Ingredients
    */
    public class VitaminDb : DbContext
    {
        public DbSet<IngredientData> Ingredients { get; set; }
        public DbSet<EffectData> Effects { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./vitamin.db");
        }
    }

    // Classes used by Entity Framework to generate database contexts
    public class IngredientData
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public IngredientType Type { get; set; }
        [Required]
        public List<EffectData> Effects { get; set; }
    }
    public class EffectData
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Trait Trait { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}