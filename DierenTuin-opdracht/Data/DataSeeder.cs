using DierenTuin_opdracht.Models;
using Bogus;
using System.Linq;

namespace DierenTuin_opdracht.Data
{
    public static class DataSeeder
    {
        public static void Initialize(ZooContext context)
        {
            // Voeg 5 categorieën toe
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Zoogdieren" },
                    new Category { Name = "Vogels" },
                    new Category { Name = "Reptielen" },
                    new Category { Name = "Amfibieën" },
                    new Category { Name = "Vissen" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Voeg 7 verblijven toe
            if (!context.Enclosures.Any())
            {
                var enclosures = new[]
                {
                    new Enclosure { Name = "Savanne", Size = 500, Climate = Climate.Tropical },
                    new Enclosure { Name = "Aquarium", Size = 200, Climate = Climate.Tropical },
                    new Enclosure { Name = "IJsberenverblijf", Size = 300, Climate = Climate.Arctic },
                    new Enclosure { Name = "Vogelvolière", Size = 150, Climate = Climate.Temperate },
                    new Enclosure { Name = "Reptielenhuis", Size = 100, Climate = Climate.Tropical },
                    new Enclosure { Name = "Apenrots", Size = 400, Climate = Climate.Tropical },
                    new Enclosure { Name = "Woestijn", Size = 250, Climate = Climate.Temperate }
                };
                context.Enclosures.AddRange(enclosures);
                context.SaveChanges();
            }

            // Voeg 7 dieren toe
            if (!context.Animals.Any())
            {
                var animalFaker = new Faker<Animal>()
                    .RuleFor(a => a.Name, f => f.Name.FirstName())
                    .RuleFor(a => a.Species, f => f.PickRandom("Leeuw", "Tijger", "Olifant", "Pinguïn", "Krokodil", "Adelaar", "Gorilla"))
                    .RuleFor(a => a.Size, f => f.PickRandom<Size>())
                    .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
                    .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
                    .RuleFor(a => a.SpaceRequirement, f => f.Random.Double(5, 50))
                    .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
                    .RuleFor(a => a.CategoryId, f => f.Random.Int(1, 5))
                    .RuleFor(a => a.EnclosureId, f => f.Random.Int(1, 7));

                var animals = animalFaker.Generate(7);
                context.Animals.AddRange(animals);
                context.SaveChanges();
            }
        }
    }
}