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
    public class ZoosController : Controller
    {
        private readonly ZooContext _context;

        public ZoosController(ZooContext context)
        {
            _context = context;
        }

        // GET: Zoos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Zoos.ToListAsync());
        }

        // GET: Zoos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // GET: Zoos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Zoos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Zoo zoo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zoo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zoo);
        }

        // GET: Zoos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos.FindAsync(id);
            if (zoo == null)
            {
                return NotFound();
            }
            return View(zoo);
        }

        // POST: Zoos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Zoo zoo)
        {
            if (id != zoo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zoo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZooExists(zoo.Id))
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
            return View(zoo);
        }

        // GET: Zoos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoo = await _context.Zoos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // POST: Zoos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zoo = await _context.Zoos.FindAsync(id);
            if (zoo != null)
            {
                _context.Zoos.Remove(zoo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Sunrise: Behandel zonsopgang voor de hele dierentuin
        public IActionResult Sunrise()
        {
            var animals = _context.Animals
                .Include(a => a.Enclosure)
                .ToList();

            var results = new List<string>();
            foreach (var animal in animals)
            {
                var status = animal.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => "wakker",
                    ActivityPattern.Nocturnal => "slaap",
                    ActivityPattern.Cathemeral => "actief",
                    _ => "onbekend"
                };
                results.Add($"{animal.Name} in {animal.Enclosure?.Name ?? "geen verblijf"} is nu {status}");
            }

            return Ok(results);
        }

        // Sunset: Behandel zonsondergang voor de hele dierentuin
        public IActionResult Sunset()
        {
            var animals = _context.Animals
                .Include(a => a.Enclosure)
                .ToList();

            var results = new List<string>();
            foreach (var animal in animals)
            {
                var status = animal.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => "slaap",
                    ActivityPattern.Nocturnal => "wakker",
                    ActivityPattern.Cathemeral => "actief",
                    _ => "onbekend"
                };
                results.Add($"{animal.Name} in {animal.Enclosure?.Name ?? "geen verblijf"} is nu {status}");
            }

            return Ok(results);
        }

        // FeedingTime: Behandel voedertijd voor de hele dierentuin
        public IActionResult FeedingTime()
        {
            var animals = _context.Animals
                .Include(a => a.Enclosure)
                .Include(a => a.Prey)
                .Include(a => a.Category)
                .ToList();

            var results = new List<string>();
            foreach (var animal in animals)
            {
                var foodInfo = animal.Prey != null && animal.Prey.Any()
                    ? $"eet: {string.Join(", ", animal.Prey.Select(p => p.Name))}"
                    : $"eet: {animal.DietaryClass} dieet";

                results.Add($"{animal.Name} in {animal.Enclosure?.Name ?? "geen verblijf"} {foodInfo}");
            }

            return Ok(results);
        }

        // CheckConstraints: Controleer alle beperkingen in de dierentuin
        public IActionResult CheckConstraints()
        {
            var issues = new List<string>();
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            foreach (var enclosure in enclosures)
            {
                foreach (var animal in enclosure.Animals)
                {
                    if (enclosure.Size < animal.SpaceRequirement)
                    {
                        issues.Add($"{animal.Name} heeft meer ruimte nodig in {enclosure.Name} ({animal.SpaceRequirement}m² nodig, {enclosure.Size}m² beschikbaar)");
                    }

                    if (enclosure.SecurityLevel < animal.SecurityRequirement)
                    {
                        issues.Add($"{animal.Name} heeft hogere beveiliging nodig in {enclosure.Name} (niveau {animal.SecurityRequirement} nodig, niveau {enclosure.SecurityLevel} beschikbaar)");
                    }
                }
            }

            return issues.Any()
                ? BadRequest(issues)
                : Ok("Alle dieren voldoen aan de verblijfseisen");
        }

        // AutoAssign: Automatisch dieren toewijzen aan verblijven
        public IActionResult AutoAssign(bool removeAll = false)
        {
            try
            {
                if (removeAll)
                {
                    // Reset alle dieren naar geen verblijf
                    var allAnimals = _context.Animals.Where(a => a.EnclosureId != null);
                    foreach (var animal in allAnimals)
                    {
                        animal.EnclosureId = null;
                    }
                }

                // Eenvoudige toewijzingslogica (pas dit aan naar je behoeften)
                var unassignedAnimals = _context.Animals
                    .Where(a => a.EnclosureId == null)
                    .ToList();

                var availableEnclosures = _context.Enclosures
                    .Include(e => e.Animals)
                    .ToList();

                foreach (var animal in unassignedAnimals)
                {
                    var suitableEnclosure = availableEnclosures
                        .FirstOrDefault(e => e.Size >= animal.SpaceRequirement &&
                                           e.SecurityLevel >= animal.SecurityRequirement);

                    if (suitableEnclosure != null)
                    {
                        animal.EnclosureId = suitableEnclosure.Id;
                        suitableEnclosure.Size -= animal.SpaceRequirement; // Update resterende ruimte
                    }
                }

                _context.SaveChanges();

                return Ok(removeAll
                    ? "Alle dieren zijn uit verblijven verwijderd en opnieuw toegewezen"
                    : "Niet-toegewezen dieren zijn toegewezen aan geschikte verblijven");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij toewijzen: {ex.Message}");
            }
        }

        private bool ZooExists(int id)
        {
            return _context.Zoos.Any(e => e.Id == id);
        }
    }
}