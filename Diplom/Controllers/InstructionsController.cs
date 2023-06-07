using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Diplom.Data;
using Diplom.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Diplom.Controllers
{
    public class InstructionsController : Controller
    {
        private readonly DiplomContext _context;

        public InstructionsController(DiplomContext context)
        {
            _context = context;
        }

        // GET: Instructions
        [Authorize(Roles = "Employee, Administrator")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder ?? "";
            ViewData["DateSortParm"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var instructions = from i in _context.Instruction select i;

            //поиск по имени или описанию
            if (!String.IsNullOrEmpty(searchString))
            {
                instructions = instructions.Where(i => i.Name!.Contains(searchString));
            }

            //сортировка по параметрам
            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.Contains("date"))
                {
                    instructions = sortOrder.EndsWith("_asc")
                        ? instructions.OrderBy(t => t.CreatedDate)
                        : instructions.OrderByDescending(t => t.CreatedDate);
                }
            }
            else
            {
                instructions = instructions.OrderBy(t => t.Name);
            }

            //количество записей на странице
            int pageSize = 10;

            return _context.Instruction != null ?
                          View(await PaginatedList<Instruction>.CreateAsync(instructions.AsNoTracking(), pageNumber ?? 1, pageSize)) :
                          Problem("Entity set 'DiplomContext.Instruction'  is null.");
        }

        // GET: Instructions/Details/5
        [Authorize(Roles = "Employee, Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Instruction == null)
            {
                return NotFound();
            }

            var instruction = await _context.Instruction
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instruction == null)
            {
                return NotFound();
            }

            return View(instruction);
        }

        // GET: Instructions/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Content")] Instruction instruction)
        {
            if (ModelState.IsValid)
            {
                instruction.CreatedDate = DateTime.Now;
                _context.Add(instruction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instruction);
        }

        // GET: Instructions/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Instruction == null)
            {
                return NotFound();
            }

            var instruction = await _context.Instruction.FindAsync(id);
            if (instruction == null)
            {
                return NotFound();
            }
            return View(instruction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Content")] Instruction instruction)
        {
            if (id != instruction.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instruction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructionExists(instruction.ID))
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
            return View(instruction);
        }

        // GET: Instructions/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Instruction == null)
            {
                return NotFound();
            }

            var instruction = await _context.Instruction
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instruction == null)
            {
                return NotFound();
            }

            return View(instruction);
        }

        // POST: Instructions/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Instruction == null)
            {
                return Problem("Entity set 'DiplomContext.Instruction'  is null.");
            }
            var instruction = await _context.Instruction.FindAsync(id);
            if (instruction != null)
            {
                _context.Instruction.Remove(instruction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructionExists(int id)
        {
          return (_context.Instruction?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
