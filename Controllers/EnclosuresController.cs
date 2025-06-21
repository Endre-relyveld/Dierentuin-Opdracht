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
    public class EnclosuresController : Controller
    {
        private readonly ZooContext _context;

        public EnclosuresController(ZooContext context)
        {
            _context = context;
        }

        // GET: Enclosures
        public async Task<IActionResult> Index()
        {
            return View(await _context.Enclosures.ToListAsync());
        }

        // GET: Enclosures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enclosure == null)
            {
                return NotFound();
            }

            return View(enclosure);
        }

        // GET: Enclosures/Create
        public IActionResult Create()
        {
            ViewBag.ClimateOptions = new SelectList(new List<string>
            {
                "Tropical",
                "Temperate",
                "Arctic",
                "Desert",
                "Aquatic"
            });

                    ViewBag.SecurityLevelOptions = new SelectList(new List<int>
            {
                1, 2, 3, 4, 5
            });

            return View();
        }

        // POST: Enclosures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Climate,HabitatType,SecurityLevel,Size")] Enclosure enclosure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enclosure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(enclosure);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }

            ViewBag.ClimateOptions = new SelectList(new List<string>
            {
                "Tropical",
                "Temperate",
                "Arctic",
                "Desert",
                "Aquatic"
            }, selectedValue: enclosure.Climate);

                    ViewBag.SecurityLevelOptions = new SelectList(new List<int>
            {
                1, 2, 3, 4, 5
            }, selectedValue: enclosure.SecurityLevel);

            return View(enclosure);
        }

        // POST: Enclosures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Climate,HabitatType,SecurityLevel,Size")] Enclosure enclosure)
        {
            if (id != enclosure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enclosure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnclosureExists(enclosure.Id))
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
            return View(enclosure);
        }

        // GET: Enclosures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enclosure == null)
            {
                return NotFound();
            }

            return View(enclosure);
        }

        // POST: Enclosures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure != null)
            {
                _context.Enclosures.Remove(enclosure);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Sunrise: Controleer alle dieren in het verblijf bij zonsopgang
        public IActionResult Sunrise(int id)
        {
            var enclosure = _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefault(e => e.Id == id);

            if (enclosure == null) return NotFound();

            var results = new List<string>();
            foreach (var animal in enclosure.Animals)
            {
                var status = animal.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => "wakker",
                    ActivityPattern.Nocturnal => "slaap",
                    ActivityPattern.Cathemeral => "actief",
                    _ => "onbekend"
                };
                results.Add($"{animal.Name} is nu {status}");
            }

            return Ok(results);
        }

        // Sunset: Controleer alle dieren in het verblijf bij zonsondergang
        public IActionResult Sunset(int id)
        {
            var enclosure = _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefault(e => e.Id == id);

            if (enclosure == null) return NotFound();

            var results = new List<string>();
            foreach (var animal in enclosure.Animals)
            {
                var status = animal.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => "slaap",
                    ActivityPattern.Nocturnal => "wakker",
                    ActivityPattern.Cathemeral => "actief",
                    _ => "onbekend"
                };
                results.Add($"{animal.Name} is nu {status}");
            }

            return Ok(results);
        }

        // FeedingTime: Toon voedertijd informatie voor alle dieren in het verblijf
        public IActionResult FeedingTime(int id)
        {
            var enclosure = _context.Enclosures
                .Include(e => e.Animals)
                    .ThenInclude(a => a.Prey)
                .Include(e => e.Animals)
                    .ThenInclude(a => a.Category)
                .FirstOrDefault(e => e.Id == id);

            if (enclosure == null) return NotFound();

            var results = new List<string>();
            foreach (var animal in enclosure.Animals)
            {
                if (animal.Prey != null && animal.Prey.Any())
                {
                    var preyNames = string.Join(", ", animal.Prey.Select(p => p.Name));
                    results.Add($"{animal.Name} eet: {preyNames}");
                }
                else
                {
                    results.Add($"{animal.Name} eet: {animal.DietaryClass} dieet");
                }
            }

            return Ok(results);
        }



        




        // CheckConstraints: Controleer of alle dieren in het verblijf voldoen aan de eisen
        public IActionResult CheckConstraints(int id)
        {
            var enclosure = _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefault(e => e.Id == id);

            if (enclosure == null) return NotFound();

            var issues = new List<string>();

            foreach (var animal in enclosure.Animals)
            {
                if (enclosure.Size < animal.SpaceRequirement)
                {
                    issues.Add($"{animal.Name} heeft meer ruimte nodig ({animal.SpaceRequirement}m², beschikbaar: {enclosure.Size}m²)");
                }

                if (enclosure.SecurityLevel < animal.SecurityRequirement)
                {
                    issues.Add($"{animal.Name} heeft hogere beveiliging nodig (niveau {animal.SecurityRequirement}, huidig: {enclosure.SecurityLevel})");
                }
            }

            if (issues.Any())
            {
                return BadRequest(new
                {
                    Enclosure = enclosure.Name,
                    Issues = issues
                });
            }

            return Ok(new
            {
                Enclosure = enclosure.Name,
                Message = "Alle dieren voldoen aan de verblijfseisen"
            });
        }

        private bool EnclosureExists(int id)
        {
            return _context.Enclosures.Any(e => e.Id == id);
        }
    }
}