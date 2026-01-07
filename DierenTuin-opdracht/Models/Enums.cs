namespace DierenTuin_opdracht.Models
{
    public enum Size
    {
        Microscopic, VerySmall, Small, Medium, Large, VeryLarge
    }

    public enum DietaryClass
    {
        Carnivore, Herbivore, Omnivore, Insectivore, Piscivore
    }

    public enum ActivityPattern
    {
        Diurnal, Nocturnal, Cathemeral
    }

    public enum SecurityLevel
    {
        Low, Medium, High
    }

    [Flags]
    public enum HabitatType
    {
        None = 0,
        Forest = 1,
        Aquatic = 2,
        Desert = 4,
        Grassland = 8
    }

    public enum Climate
    {
        Tropical, Temperate, Arctic
    }
}
