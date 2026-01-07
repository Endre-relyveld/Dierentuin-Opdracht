using DierenTuin_opdracht.Models;

namespace DierenTuin_opdracht.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Animal>? Animals { get; set; } = new();
    }
}
