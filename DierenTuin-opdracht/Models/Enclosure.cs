using DierenTuin_opdracht.Models;

namespace DierenTuin_opdracht.Models
{
    public class Enclosure
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Animal>? Animals { get; set; } = new();

        public Climate Climate { get; set; }
        public HabitatType HabitatType { get; set; }

        public string DietaryRestrictions { get; set; } = string.Empty;




        public SecurityLevel SecurityLevel { get; set; }
        public double Size { get; set; }
    }
}
