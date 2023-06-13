using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Diplom.Data;
using Diplom.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Diplom.Controllers
{
    [Authorize(Roles = "Employee, Administrator")]
    public class LogEntriesController : Controller
    {
        private readonly DiplomContext _context;

        public LogEntriesController(DiplomContext context)
        {
            _context = context;
        }

        // GET: LogEntries
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

            var entries = from i in _context.LogEntry select i;

            //поиск по имени или описанию
            if (!String.IsNullOrEmpty(searchString))
            {
                entries = entries.Where(i => i.Name!.Contains(searchString));
            }

            //сортировка по параметрам
            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.Contains("date"))
                {
                    entries = sortOrder.EndsWith("_asc")
                        ? entries.OrderBy(t => t.Date)
                        : entries.OrderByDescending(t => t.Date);
                }
            }
            else
            {
                entries = entries.OrderBy(t => t.Name);
            }

            //количество записей на странице
            int pageSize = 10;

            return _context.Instruction != null ?
                          View(await PaginatedList<LogEntry>.CreateAsync(entries.AsNoTracking(), pageNumber ?? 1, pageSize)) :
                          Problem("Entity set 'DiplomContext.Instruction'  is null.");
        }

        // GET: LogEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LogEntry == null)
            {
                return NotFound();
            }

            var logEntry = await _context.LogEntry
                .FirstOrDefaultAsync(m => m.ID == id);
            if (logEntry == null)
            {
                return NotFound();
            }

            return View(logEntry);
        }

        // GET: LogEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LogEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Date,Executor")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
                logEntry.CreatedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute,0);
                                  
                
                _context.Add(logEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(logEntry);
        }

        // GET: LogEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LogEntry == null)
            {
                return NotFound();
            }

            var logEntry = await _context.LogEntry.FindAsync(id);
            if (logEntry == null)
            {
                return NotFound();
            }
            return View(logEntry);
        }

        // POST: LogEntries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,CreatedDate,Date,Executor")] LogEntry logEntry)
        {
            if (id != logEntry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogEntryExists(logEntry.ID))
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
            return View(logEntry);
        }

        // GET: LogEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LogEntry == null)
            {
                return NotFound();
            }

            var logEntry = await _context.LogEntry
                .FirstOrDefaultAsync(m => m.ID == id);
            if (logEntry == null)
            {
                return NotFound();
            }

            return View(logEntry);
        }

        // POST: LogEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LogEntry == null)
            {
                return Problem("Entity set 'DiplomContext.LogEntry'  is null.");
            }
            var logEntry = await _context.LogEntry.FindAsync(id);
            if (logEntry != null)
            {
                _context.LogEntry.Remove(logEntry);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogEntryExists(int id)
        {
          return (_context.LogEntry?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
