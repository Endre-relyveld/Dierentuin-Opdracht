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
        public async Task<IActionResult> Index()
        {
            var zooContext = _context.Animals.Include(a => a.Category).Include(a => a.Enclosure);
            return View(await zooContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        // GET: Animals/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");  // Toon Name i.p.v. Id
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name"); // Toon Name i.p.v. Id
            return View();
        }

        

        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Species,CategoryId,Size,DietaryClass,ActivityPattern,EnclosureId,SpaceRequirement,SecurityRequirement")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Id", animal.EnclosureId);
            return View(animal);
        }

        // GET: Animals/Edit/5
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            // Verplaats deze reges NA de null-check
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);

            return View(animal);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,CategoryId,Size,DietaryClass,ActivityPattern,EnclosureId,SpaceRequirement,SecurityRequirement")] Animal animal)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animal);
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
