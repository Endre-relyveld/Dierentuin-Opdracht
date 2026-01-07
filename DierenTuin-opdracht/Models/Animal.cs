using DierenTuin_opdracht.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DierenTuin_opdracht.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Size Size { get; set; }
        public DietaryClass DietaryClass { get; set; }
        public ActivityPattern ActivityPattern { get; set; }

        public int? EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }

        public double SpaceRequirement { get; set; }

        public SecurityLevel SecurityRequirement { get; set; }

        public List<Animal>? Prey { get; set; } = new();
    }
}
