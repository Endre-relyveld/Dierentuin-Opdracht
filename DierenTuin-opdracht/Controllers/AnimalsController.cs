using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DierenTuin_opdracht.Data;
using DierenTuin_opdracht.Models;

namespace DierenTuin_opdracht.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ZooContext _context;

        public AnimalsController(ZooContext context)
        {
            _context = context;
        }

        // Sunset: vergelijkbaar met Sunrise maar omgekeerde logica
        public IActionResult Sunset(int id)
        {
            var animal = _context.Animals.Include(a => a.Enclosure).FirstOrDefault(a => a.Id == id);
            if (animal == null) return NotFound();

            var status = animal.ActivityPattern switch
            {
                ActivityPattern.Diurnal => "slaap",
                ActivityPattern.Nocturnal => "wakker",
                ActivityPattern.Cathemeral => "actief",
                _ => "onbekend"
            };

            return Ok($"{animal.Name} is nu {status}.");
        }

        // CheckConstraints: controleer of het dier in het juiste verblijf zit
        public IActionResult CheckConstraints(int id)
        {
            var animal = _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefault(a => a.Id == id);

            if (animal == null) return NotFound();

            var issues = new List<string>();

            // Controleer ruimtevereiste
            if (animal.Enclosure?.Size < animal.SpaceRequirement)
            {
                issues.Add($"Verblijf is te klein (heeft {animal.Enclosure.Size}m², nodig {animal.SpaceRequirement}m²)");
            }

            // Controleer veiligheidseisen
            if (animal.SecurityRequirement > (animal.Enclosure?.SecurityLevel ?? 0))
            {
                issues.Add($"Verblijf heeft onvoldoende beveiliging (niveau {animal.Enclosure?.SecurityLevel}, nodig niveau {animal.SecurityRequirement})");
            }

            // Controleer dieetcompatibiliteit met verblijf
            if (animal.Enclosure != null && animal.Enclosure.DietaryRestrictions != null)
            {
                if (animal.Enclosure.DietaryRestrictions.Contains(animal.DietaryClass.ToString()))
                {
                    issues.Add($"Dieet ({animal.DietaryClass}) is niet toegestaan in dit verblijf");
                }
            }

            if (issues.Any())
            {
                return BadRequest(new
                {
                    Animal = animal.Name,
                    Issues = issues
                });
            }

            return Ok(new
            {
                Animal = animal.Name,
                Message = "Voldoet aan alle eisen"
            });
        }


        private static string ToFirstUpper(string? input)
        {
            var s = (input ?? "").Trim().ToLowerInvariant();
            if (s.Length == 0) return "";
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private List<Animal> GetUniquePreyOptionsBySpecies(int? excludeAnimalId = null)
        {
            return _context.Animals
                .AsNoTracking()
                .Where(a => excludeAnimalId == null || a.Id != excludeAnimalId)
                .ToList()
                .Where(a => !string.IsNullOrWhiteSpace(a.Species)) // geen lege soort
                .GroupBy(a => a.Species!.Trim().ToLowerInvariant()) // ✅ dedupe op soort
                .Select(g =>
                {
                    var first = g.First(); // kiest 1 record als “representant” van die soort
                    return new Animal
                    {
                        Id = first.Id,
                        // we zetten Name op de nette soort-tekst zodat je view (@p.Name) het goed toont
                        Name = ToFirstUpper(first.Species),
                        Species = first.Species
                    };
                })
                .OrderBy(a => a.Name)
                .ToList();
        }



        // Sunrise: geeft aan of het dier wakker wordt of gaat slapen
        public IActionResult Sunrise(int id)
        {
            var animal = _context.Animals.Include(a => a.Enclosure).FirstOrDefault(a => a.Id == id);
            if (animal == null) return NotFound();

            var status = animal.ActivityPattern switch
            {
                ActivityPattern.Diurnal => "wakker",
                ActivityPattern.Nocturnal => "slaap",
                ActivityPattern.Cathemeral => "actief",
                _ => "onbekend"
            };

            return Ok($"{animal.Name} is nu {status}.");
        }

        // FeedingTime: wat eet het dier
        public IActionResult FeedingTime(int id)
        {
            var animal = _context.Animals
                .Include(a => a.Prey)
                .FirstOrDefault(a => a.Id == id);

            if (animal == null) return NotFound();

            if (animal.Prey != null && animal.Prey.Any())
            {
                var preyNames = string.Join(", ", animal.Prey.Select(p => p.Name));
                return Ok($"{animal.Name} eet: {preyNames}.");
            }

            return Ok($"{animal.Name} eet: {animal.DietaryClass} dieet.");
        }



        // GET: Animals
        public async Task<IActionResult> Index(
            string? name,
            string? species,
            int? categoryId,
            Size? size,
            DietaryClass? dietaryClass,
            ActivityPattern? activityPattern,
            string? enclosure,
            double? minSpace,
            string? preySpecies,
            SecurityLevel? securityRequirement
        )
        {
            var query = _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .AsQueryable();

            // Naam
            if (!string.IsNullOrWhiteSpace(name))
            {
                var n = name.Trim().ToLower();
                query = query.Where(a => a.Name != null && a.Name.ToLower().Contains(n));
            }

            // Soort
            if (!string.IsNullOrWhiteSpace(species))
            {
                var s = species.Trim().ToLower();
                query = query.Where(a => a.Species != null && a.Species.ToLower().Contains(s));
            }

            // Categorie
            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            // Grootte
            if (size.HasValue)
                query = query.Where(a => a.Size == size.Value);

            // Voedingsklasse
            if (dietaryClass.HasValue)
                query = query.Where(a => a.DietaryClass == dietaryClass.Value);

            // Activiteitspatroon
            if (activityPattern.HasValue)
                query = query.Where(a => a.ActivityPattern == activityPattern.Value);

            // Verblijf (op naam)
            if (!string.IsNullOrWhiteSpace(enclosure))
            {
                var e = enclosure.Trim().ToLower();
                query = query.Where(a => a.Enclosure != null && a.Enclosure.Name.ToLower().Contains(e));
            }

            // Ruimtebehoefte (min)
            if (minSpace.HasValue)
                query = query.Where(a => a.SpaceRequirement >= minSpace.Value);

            // Beveiligingsniveau
            if (securityRequirement.HasValue)
                query = query.Where(a => a.SecurityRequirement == securityRequirement.Value);

            // Eerst server-side resultaat ophalen (met prey erbij voor view + prey-filter)
            var animals = await query
                .Include(a => a.Prey)
                .ToListAsync();

            // Prooi filter (client-side, omdat EF dit bij jou niet kan vertalen)
            if (!string.IsNullOrWhiteSpace(preySpecies))
            {
                var p = preySpecies.Trim();

                animals = animals
                    .Where(a => a.Prey != null && a.Prey.Any(pr =>
                        !string.IsNullOrWhiteSpace(pr.Species) &&
                        pr.Species.Contains(p, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Dropdown data (nodig voor view)
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", categoryId);

            // (optioneel) als je dit in je view gebruikt:
            ViewData["SelectedSize"] = size;
            ViewData["SelectedDietaryClass"] = dietaryClass;
            ViewData["SelectedActivityPattern"] = activityPattern;
            ViewData["SelectedSecurityRequirement"] = securityRequirement;

            return View(animals);
        }




        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .Include(a => a.Prey)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name");

            ViewData["PreyOptions"] = GetUniquePreyOptionsBySpecies();


            return View();
        }




        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Species,CategoryId,Size,DietaryClass,ActivityPattern,EnclosureId,SpaceRequirement,SecurityRequirement")] Animal animal,
            List<int> SelectedPreyIds
        )


        {
            if (ModelState.IsValid)
            {
                if (SelectedPreyIds != null && SelectedPreyIds.Any())
                {
                    animal.Prey = await _context.Animals
                        .Where(a => SelectedPreyIds.Contains(a.Id))
                        .ToListAsync();
                }

                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Id", animal.EnclosureId);

            ViewData["PreyOptions"] = GetUniquePreyOptionsBySpecies();


            return View(animal);
        }

        // GET: Animals/Edit/5
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
            .Include(a => a.Prey)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            // Verplaats deze reges NA de null-check
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);

            ViewData["PreyOptions"] = GetUniquePreyOptionsBySpecies(animal.Id);




            ViewData["SelectedPreyIds"] = animal.Prey?
                .Select(p => p.Id)
                .ToList() ?? new List<int>();

            return View(animal);

        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Species,CategoryId,Size,DietaryClass,ActivityPattern,EnclosureId,SpaceRequirement,SecurityRequirement")] Animal animal,
            List<int> SelectedPreyIds
        )

        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var animalDb = await _context.Animals
                        .Include(a => a.Prey)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (animalDb == null) return NotFound();

                    animalDb.Name = animal.Name;
                    animalDb.Species = animal.Species;
                    animalDb.CategoryId = animal.CategoryId;
                    animalDb.Size = animal.Size;
                    animalDb.DietaryClass = animal.DietaryClass;
                    animalDb.ActivityPattern = animal.ActivityPattern;
                    animalDb.EnclosureId = animal.EnclosureId;
                    animalDb.SpaceRequirement = animal.SpaceRequirement;
                    animalDb.SecurityRequirement = animal.SecurityRequirement;

                    animalDb.Prey ??= new List<Animal>();
                    animalDb.Prey.Clear();

                    if (SelectedPreyIds != null && SelectedPreyIds.Any())
                    {
                        var prey = await _context.Animals
                            .Where(a => SelectedPreyIds.Contains(a.Id) && a.Id != id)
                            .ToListAsync();

                        foreach (var p in prey)
                            animalDb.Prey.Add(p);
                    }

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Id", animal.EnclosureId);

            ViewData["PreyOptions"] = GetUniquePreyOptionsBySpecies(id);




            return View(animal);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Category)
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }
    }
}
