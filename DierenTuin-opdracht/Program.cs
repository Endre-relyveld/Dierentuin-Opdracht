using DierenTuin_opdracht.Data;
using DierenTuin_opdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace DierenTuin_opdracht
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Voeg de DbContext toe
            builder.Services.AddDbContext<ZooContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Voeg controllers en views toe
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configureer de HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");







            //using (var scope = app.Services.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<ZooContext>();
            //    context.Database.EnsureCreated(); // Zorg dat database bestaat

            //    // Voeg testdata toe als de database leeg is
            //    if (!context.Animals.Any())
            //    {
            //        // Voorbeeld: voeg 1 testcategorie en dier toe
            //        var category = new Category { Name = "Zoogdieren" };
            //        context.Categories.Add(category);

            //        var animal = new Animal
            //        {
            //            Name = "Leo",
            //            Species = "Leeuw",
            //            Size = Size.Large,
            //            DietaryClass = DietaryClass.Carnivore,
            //            Category = category
            //        };
            //        context.Animals.Add(animal);

            //        context.SaveChanges();
            //    }
            //}

            // In Program.cs
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ZooContext>();
                    DataSeeder.Initialize(context);
                    Console.WriteLine("Database seeding voltooid!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Fout tijdens database seeding");
                }
            }



            app.Run();
        }
    }
}
