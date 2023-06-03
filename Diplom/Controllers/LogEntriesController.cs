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
    [Authorize(Roles = "Employee, Administrator")]
    public class LogEntriesController : Controller
    {
        private readonly DiplomContext _context;

        public LogEntriesController(DiplomContext context)
        {
            _context = context;
        }

        // GET: LogEntries
        public async Task<IActionResult> Index()
        {
              return _context.LogEntry != null ? 
                          View(await _context.LogEntry.ToListAsync()) :
                          Problem("Entity set 'DiplomContext.LogEntry'  is null.");
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,CreatedDate,Date,Executor")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
