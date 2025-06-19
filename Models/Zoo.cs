using DierenTuin_opdracht.Models;

namespace DierenTuin_opdracht.Models
{
    public class Zoo
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Mijn Dierentuin";

        public List<Enclosure>? Enclosures { get; set; } = new();
    }
}
