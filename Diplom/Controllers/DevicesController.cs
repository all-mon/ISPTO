using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplom.Data;
using Diplom.Models;
using System.Xml.Linq;
using Diplom.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Diplom.Controllers
{
    public class DevicesController : Controller
    {
        private readonly DiplomContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DevicesController(DiplomContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //Главная GET: Devices
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder)? "name_desc" : "";
            var wwwrootPath = _webHostEnvironment.WebRootPath;
            ViewData["Test"] = wwwrootPath;


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var devices = from d in _context.Device select d;

            //поиск по имени или описанию
            if (!String.IsNullOrEmpty(searchString))
            {
                devices = devices.Where(d => d.Name!.Contains(searchString) || d.Description.Contains(searchString));
            }
            //сортировка по имети
            switch (sortOrder) 
            {
                case "name_desc":
                    devices = devices.OrderByDescending( d => d.Name);
                    break;
                default:
                    devices = devices.OrderBy(d => d.Name);
                    break;
            }
            //количество записей на странице
            int pageSize = 10;
            return View(await PaginatedList<Device>.CreateAsync(devices.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        //Подробнее GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Device == null)
            {
                return NotFound();
            }

            /*var device = await _context.Device
                .FirstOrDefaultAsync(m => m.ID == id);*/
            var device = await _context.Device.Include(d => d.DevicePlacements)!.ThenInclude(dp => dp.Placement).AsNoTracking().
                FirstOrDefaultAsync(p => p.ID == id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        //Создать GET: Devices/Create
        public IActionResult Create()
        {
            //создать модель оборудования
            var model = new Device();
            //Загрузить список возможных аналогов(подгружаем все устройства, для дальнейшего выбора)
            model.Analogues = _context.Device.ToList();
            //Подгрузить все возможные места установки(для дальнейшего выбора)
            PopulateAllPlacementData();
            return View(model);
        }

        //Создать POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Device device, string[] selectedPlacements)
        {
            // var errors = ModelState.Values.SelectMany(v => v.Errors);
            try
            {
                if (ModelState.IsValid)
                {
                    // Загрузка изображения
                    if (device.ImageFile != null && device.ImageFile.Length > 0)
                    {
                        var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(device.ImageFile.FileName);
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", imageFileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await device.ImageFile.CopyToAsync(stream);
                        }

                        device.ImagePath = "/images/" + imageFileName;
                    }

                    // Загрузка документации
                    if (device.DocumentationFile != null && device.DocumentationFile.Length > 0)
                    {
                        var docFileName = Guid.NewGuid().ToString() + Path.GetExtension(device.DocumentationFile.FileName);
                        var docPath = Path.Combine(_webHostEnvironment.WebRootPath, "docs", docFileName);

                        using (var stream = new FileStream(docPath, FileMode.Create))
                        {
                            await device.DocumentationFile.CopyToAsync(stream);
                        }

                        device.DocumentPath = "/docs/" + docFileName;
                    }
                    
                    // Логика сохранения оборудования в базе данных
                    _context.Add(device);
                    UpdateDevicePlacements(selectedPlacements, device);
                    PopulateAssignedPlacementData(device);

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
            //сброс отмеченных аналогов
            device.Analogues = _context.Device.ToList();
            return View(device);
        }

        //Подгрузить все возможные места установки(для дальнейшего выбора)
        private void PopulateAllPlacementData()
        {
            var AllPlacement = _context.Placement;
            var viewModel = new List<AssignedPlacementData>();
            foreach (var place in AllPlacement)
            {
                viewModel.Add(new AssignedPlacementData
                {
                    PlacementID = place.ID,
                    Title = place.Name,
                    Assigned = false
                });
            }
            ViewData["AllPlacements"] = viewModel;
        }

        //Изменить
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Device == null)
            {
                return NotFound();
            }

            var device = await _context.Device.Include(d => d.DevicePlacements).ThenInclude(i => i.Placement).AsNoTracking().FirstOrDefaultAsync(s=>s.ID == id);  
            if (device == null)
            {
                return NotFound();
            }
            PopulateAssignedPlacementData(device);
            return View(device);
        }

        //Подгрузка выбранных мест установки, уже выбранные отмечены(поле Assigned)
        private void PopulateAssignedPlacementData(Device device)
        {
            var AllPlacement = _context.Placement;
            var devicePlacements = new HashSet<int>(device.DevicePlacements.Select(p => p.PlacementID));
            var viewModel = new List<AssignedPlacementData>();
            foreach (var place in AllPlacement)
            {
                viewModel.Add(new AssignedPlacementData
                {
                    PlacementID = place.ID,
                    Title = place.Name,
                    Assigned = devicePlacements.Contains(place.ID)
                });
            }
            ViewData["Placements"] = viewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedPlacements)
        {
            if (id == null)
            {
                return NotFound();
            }
            var deviceToUpdate = await _context.Device.Include(d => d.DevicePlacements)
                .ThenInclude(p=>p.Placement).FirstOrDefaultAsync(d=>d.ID == id);

            if (await TryUpdateModelAsync<Device>(
                deviceToUpdate! ,"", d => d.Name, d => d.Description, d => d.ImagePath, d => d.DocumentPath))
            {
                UpdateDevicePlacements(selectedPlacements,deviceToUpdate!);
                PopulateAssignedPlacementData(deviceToUpdate!);

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
            UpdateDevicePlacements (selectedPlacements, deviceToUpdate!);
            PopulateAssignedPlacementData (deviceToUpdate!);
            return View(deviceToUpdate);
        }
        private void SavePlacementForNewDevice(string[] selectedPlacements, Device device)
        {

        }
        private void UpdateDevicePlacements(string[] selectedPlacements, Device deviceToUpdate)
        {
            if (selectedPlacements == null)
            {
                deviceToUpdate!.DevicePlacements = new List<DevicePlacement>();
                return;
            }

            var selectedPlacementsHS = new HashSet<string>(selectedPlacements);
            var devicePlacementsHS = new HashSet<int>(deviceToUpdate.DevicePlacements.Select(p=>p.Placement.ID));

            foreach (var place in _context.Placement)
            {
                if (selectedPlacementsHS.Contains(place.ID.ToString()))
                {
                    if (!devicePlacementsHS.Contains(place.ID))
                    {
                        deviceToUpdate.DevicePlacements!.Add(new DevicePlacement
                        {
                            PlacementID = place.ID,
                            DeviceID = deviceToUpdate.ID
                        });
                    }
                }
                else
                {
                    if (devicePlacementsHS.Contains(place.ID))
                    {
                        DevicePlacement dpToRemove = deviceToUpdate.DevicePlacements!.FirstOrDefault(i => i.PlacementID == place.ID)!;
                        _context.Remove(dpToRemove!);
                    }
                }
            }

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