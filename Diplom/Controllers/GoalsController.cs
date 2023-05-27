using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplom.Data;
using Diplom.Models;

namespace Diplom.Controllers
{
    public class GoalsController : Controller
    {
        private readonly DiplomContext _context;

        public GoalsController(DiplomContext context)
        {
            _context = context;
        }

        // GET: Goals
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            /*
                        ViewData["CurrentSort"] = sortOrder;
                        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                        ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
                        ViewData["IsCompletedSortParm"] = sortOrder == "CompletedTrue" ? "CompletedFalse" : "CompletedTrue";*/


            ViewData["CurrentSort"] = sortOrder ?? "";

            ViewData["NameSortParm"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["DateSortParm"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["IsCompletedParm"] = sortOrder == "completed_true" ? "completed_false" : "completed_true";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var tasks = from t in _context.Task select t;

            //поиск по имени или описанию
            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(d => d.Name!.Contains(searchString) || d.Description!.Contains(searchString));
            }
            //сортировка по имети/приоритету
            /*switch (sortOrder)
            {
                case "name_desc":
                    tasks = tasks.OrderByDescending(t => t.Name);
                    break;
                case "Date":
                    tasks = tasks.OrderBy(t => t.TaskDate);
                    break;
                case "date_desc":
                    tasks = tasks.OrderByDescending(t => t.TaskDate);
                    break;
                case "CompletedTrue":
                    tasks = tasks.Where(t => t.IsCompleted);
                    break;
                case "CompletedFalse":
                    tasks = tasks.Where(t => !t.IsCompleted);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.Name);
                    break;
            }*/
            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.Contains("name"))
                {
                    tasks = sortOrder.EndsWith("_asc")
                        ? tasks.OrderBy(t => t.Name)
                        : tasks.OrderByDescending(t => t.Name);
                }
                if (sortOrder.Contains("date"))
                {
                    tasks = sortOrder.EndsWith("_asc")
                        ? tasks.OrderBy(t => t.TaskDate)
                        : tasks.OrderByDescending(t => t.TaskDate);
                }
                if (sortOrder.Contains("completed"))
                {
                    bool isCompleted = sortOrder.Contains("true");
                    tasks = tasks.Where(t => t.IsCompleted == isCompleted);
                }
            }
            else
            {
                tasks = tasks.OrderBy(t => t.Name);
            }


            //количество записей на странице
            int pageSize = 12;
            return View(await PaginatedList<Goal>.CreateAsync(tasks.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Goals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var goal = await _context.Task
                .FirstOrDefaultAsync(m => m.ID == id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // GET: Goals/Create
        public IActionResult Create()
        {
           return View();
        }

        // POST: Goals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,TaskDate,Priority")] Goal goal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(goal);
        }

        // GET: Goals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var goal = await _context.Task.FindAsync(id);
            if (goal == null)
            {
                return NotFound();
            }
            return View(goal);
        }

        // POST: Goals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,TaskDate,Priority,IsCompleted")] Goal goal)
        {
            if (id != goal.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoalExists(goal.ID))
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
            return View(goal);
        }

        // GET: Goals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var goal = await _context.Task
                .FirstOrDefaultAsync(m => m.ID == id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Task == null)
            {
                return Problem("Entity set 'DiplomContext.Task'  is null.");
            }
            var goal = await _context.Task.FindAsync(id);
            if (goal != null)
            {
                _context.Task.Remove(goal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoalExists(int id)
        {
          return (_context.Task?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
