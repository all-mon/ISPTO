﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplom.Data;
using Diplom.Models;
using System.Xml.Linq;

namespace Diplom.Controllers
{
    public class DevicesController : Controller
    {
        private readonly DiplomContext _context;

        public DevicesController(DiplomContext context)
        {
            _context = context;
        }

        // GET: Devices
        public async Task<IActionResult> Index()
        {
              return _context.Device != null ? 
                          View(await _context.Device.ToListAsync()) :
                          Problem("Entity set 'DiplomContext.Device'  is null.");
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Device == null)
            {
                return NotFound();
            }

            /*var device = await _context.Device
                .FirstOrDefaultAsync(m => m.ID == id);*/
            var device = await _context.Device.Include(d => d.DevicePlacements).ThenInclude(dp => dp.Placement).AsNoTracking().
                FirstOrDefaultAsync(p => p.ID == id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImagePath,DocumentPath")] Device device)
        {
            // var errors = ModelState.Values.SelectMany(v => v.Errors);
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(device);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {

                // Log the error(uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
            "Try again, and if the problem persists " +
            "see your system administrator.");
            }

            
            return View(device);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Device == null)
            {
                return NotFound();
            }

            var device = await _context.Device.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       /* [HttpPost]
        [ValidateAntiForgeryToken]*/
        /*public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,ImagePath,DocumentPath")] Device device)
        {
            if (id != device.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.ID))
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
            return View(device);
        }*/
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var deviceToUpdate = await _context.Device.FirstOrDefaultAsync(s=>s.ID == id);

            if (await TryUpdateModelAsync<Device>(
                deviceToUpdate ,"", d => d.Name, d => d.Description, d => d.ImagePath, d => d.DocumentPath))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            return View(deviceToUpdate);
        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null || _context.Device == null)
            {
                return NotFound();
            }

            var device = await _context.Device.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (device == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(device);
        }

        /* // POST: Devices/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(int id)
         {
             if (_context.Device == null)
             {
                 return Problem("Entity set 'DiplomContext.Device'  is null.");
             }
             var device = await _context.Device.FindAsync(id);
             if (device != null)
             {
                 _context.Device.Remove(device);
             }

             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
         }*/

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Device.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Device.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool DeviceExists(int id)
        {
          return (_context.Device?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
