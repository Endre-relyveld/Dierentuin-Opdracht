using Microsoft.EntityFrameworkCore;
using DierenTuin_opdracht.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DierenTuin_opdracht.Data
{
    public class ZooContext : DbContext
    {
        public ZooContext(DbContextOptions<ZooContext> options) : base(options) { }

        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enclosure> Enclosures => Set<Enclosure>();
        public DbSet<Zoo> Zoos => Set<Zoo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Zoo>()
                .HasMany(z => z.Enclosures)
                .WithOne(e => e.Zoo)
                .HasForeignKey(e => e.ZooId);


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .HasMany(a => a.Prey)
                .WithMany()
                .UsingEntity(j => j.ToTable("AnimalPrey"));
        }
    }
}
