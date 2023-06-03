using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplom.Data;
using Diplom.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Diplom.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PlacementsController : Controller
    {
        private readonly DiplomContext _context;

        public PlacementsController(DiplomContext context)
        {
            _context = context;
        }

        // GET: Placements
        public async Task<IActionResult> Index()
        {
              return _context.Placement != null ? 
                          View(await _context.Placement.ToListAsync()) :
                          Problem("Entity set 'DiplomContext.Placement'  is null.");
        }

        // GET: Placements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Placement == null)
            {
                return NotFound();
            }

            var placement = await _context.Placement
                .FirstOrDefaultAsync(m => m.ID == id);
            if (placement == null)
            {
                return NotFound();
            }

            return View(placement);
        }

        // GET: Placements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Placements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description")] Placement placement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(placement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(placement);
        }

        // GET: Placements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Placement == null)
            {
                return NotFound();
            }

            var placement = await _context.Placement.FindAsync(id);
            if (placement == null)
            {
                return NotFound();
            }
            return View(placement);
        }

        // POST: Placements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description")] Placement placement)
        {
            if (id != placement.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(placement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlacementExists(placement.ID))
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
            return View(placement);
        }

        // GET: Placements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Placement == null)
            {
                return NotFound();
            }

            var placement = await _context.Placement
                .FirstOrDefaultAsync(m => m.ID == id);
            if (placement == null)
            {
                return NotFound();
            }

            return View(placement);
        }

        // POST: Placements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Placement == null)
            {
                return Problem("Entity set 'DiplomContext.Placement'  is null.");
            }
            var placement = await _context.Placement.FindAsync(id);
            if (placement != null)
            {
                _context.Placement.Remove(placement);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlacementExists(int id)
        {
          return (_context.Placement?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
