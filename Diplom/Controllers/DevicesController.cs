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

            
            var device = await _context.Device
                .Include(d  => d.AnalogDevice)
                    .ThenInclude(a => a.Analog)
                .Include(d => d.DevicePlacements)!
                    .ThenInclude(dp => dp.Placement)
                .AsNoTracking().
                FirstOrDefaultAsync(p => p.ID == id);
            
            if (device == null)
            {
                return NotFound();
            }

            //ViewData["deviceAnalogues"] = device.Analogues;

            return View(device);
        }

        //Создать GET: Devices/Create
        public IActionResult Create()
        {
            //создать модель оборудования
            //var model = new Device();

            PopulateAnalogDevicesDropDownList();

            //Подгрузить все возможные места установки(для дальнейшего выбора)
            PopulateAllPlacementData();
            return View();
        }
        //Селект со всеми устройсвами(для выбора аналогов)
        private void PopulateAnalogDevicesDropDownList()
        {
            var devices = _context.Device.OrderBy(d => d.Name).ToList();
            ViewBag.AnalogDevices = new SelectList(devices, "ID", "Name");
        }

        //Создать POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Device device, string[] selectedPlacements, int[] selectedAnalogDevices)
        {
            // var errors = ModelState.Values.SelectMany(v => v.Errors);
            try
            {
                if (ModelState.IsValid)
                {
                    //device.Analogues = _context.Device.Where(d => device.SelectedAnalogues.Contains(d.ID)).ToList();

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

                    if (selectedAnalogDevices != null)
                    {
                        foreach (int analogDeviceId in selectedAnalogDevices)
                        {
                            // Создаем связь "многие ко многим" и добавляем ее в контекст базы данных
                            AnalogDevice analogDevice = new AnalogDevice { DeviceId = device.ID, AnalogId = analogDeviceId };
                            _context.AnalogDevice.Add(analogDevice);
                        }
                        await _context.SaveChangesAsync();
                    }

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
           // device.Analogues = _context.Device.ToList();
            PopulateAllPlacementData();
            PopulateAnalogDevicesDropDownList();
            return View(device);
        }

        //Создает List<AssignedPlacementData>, где все места установки c флагом false(чистый список), добавляет его в ViewData
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

        //Изменить GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Device == null)
            {
                return NotFound();
            }

            var device = await _context.Device
            .Include(d => d.AnalogDevice)
                .ThenInclude(ad => ad.Analog)
            .Include(d => d.DevicePlacements)
                .ThenInclude(dp => dp.Placement)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ID == id);

            if (device == null)
            {
                return NotFound();
            }
            PopulateAssignedPlacementData(device);
            await PopulateAnalogDevicesDropDownListAsync(device);
            return View(device);
        }
        //Селект для метода Edit, с уже выбранными ранее местами установки
        private async Task PopulateAnalogDevicesDropDownListAsync(Device device)
        {
            var devices = await _context.Device.ToListAsync();
            var selectedAnalogDevices = device.AnalogDevice.Select(ad => ad.AnalogId).ToList();
            var selectList = new SelectList(devices, "ID", "Name");

            foreach (var item in selectList)
            {
                int itemID =  Int32.Parse(item.Value);
                if (selectedAnalogDevices.Contains(itemID))
                {
                    item.Selected = true;
                }
            }
            ViewBag.AnalogDevices = selectList;
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
        public async Task<IActionResult> Edit(int? id, string[] selectedPlacements,
            string[] selectedAnalogDevices,
            IFormFile? imageFile, IFormFile? documentationFile)
        {
            if (id == null)
            {
                return NotFound();
            }            
            var deviceToUpdate = await _context.Device
            .Include(d => d.AnalogDevice)
                .ThenInclude(ad => ad.Analog)
            .Include(d => d.DevicePlacements)
                .ThenInclude(dp => dp.Placement)
            .FirstOrDefaultAsync(d => d.ID == id);

            if (await TryUpdateModelAsync<Device>(
                deviceToUpdate! ,"", d => d.Name, d => d.Description, d=>d.QuantityInStock) && deviceToUpdate != null)
            {
                string oldImagePath = deviceToUpdate.ImagePath!;
                string oldDocPath = deviceToUpdate.DocumentPath!;

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Загрузка нового изображения и сохранение его на сервере
                    string newImagePath = SaveImage(imageFile);

                    // Обновление поля ImagePath в объекте Device
                    deviceToUpdate.ImagePath = newImagePath;

                    // Удаление старого изображения
                    DeleteOldImage(oldImagePath);
                }
                if(documentationFile != null && documentationFile.Length > 0)
                {
                    string newDocumentationPath = SaveDocumentation(documentationFile);
                    deviceToUpdate.DocumentPath = newDocumentationPath;
                    DeleteOldDocumentation(oldDocPath);
                }

                UpdateDevicePlacements(selectedPlacements,deviceToUpdate!);
                UpdateDeviceAnalogs(selectedAnalogDevices,deviceToUpdate!);


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

        private void DeleteOldDocumentation(string oldDocPath)
        {
            if (oldDocPath != null && oldDocPath != "/docs/default_item_pdf.pdf")
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, oldDocPath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        private string SaveDocumentation(IFormFile documentationFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "docs");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + documentationFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                documentationFile.CopyTo(fileStream);
            }
            return "/docs/" + uniqueFileName;
        }

        private void DeleteOldImage(string imagePath)
        {
            if (imagePath != null && imagePath != "/images/default_item_icon.png")
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        private string SaveImage(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }
            return "/images/" + uniqueFileName;
        }

        private void UpdateDeviceAnalogs(string[] selectedAnalogDevices, Device deviceToUpdate)
        {
            if (selectedAnalogDevices == null)
            {
                deviceToUpdate!.AnalogDevice = new List<AnalogDevice>();
                return;
            }
            var selectedAnalogsHS = new HashSet<string> (selectedAnalogDevices);
            var deviceAnalogsHS = new HashSet<int>(deviceToUpdate.AnalogDevice.Select(a => a.AnalogId));

            foreach (var device in _context.Device)
            {
                if (selectedAnalogDevices.Contains(device.ID.ToString()))
                {
                    if (!deviceAnalogsHS.Contains(device.ID))
                    {
                        deviceToUpdate.AnalogDevice!.Add(new AnalogDevice
                        {
                            AnalogId = device.ID,
                            DeviceId = deviceToUpdate.ID
                        });
                    }
                }
                else
                {
                    if (deviceAnalogsHS.Contains(device.ID))
                    {
                        AnalogDevice dpToRemove = deviceToUpdate.AnalogDevice!.FirstOrDefault(i => i.AnalogId == device.ID)!;
                        _context.Remove(dpToRemove!);
                    }
                }
            }
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
            

            var device = await _context.Device
            .FirstAsync(d => d.ID == id);

            if (device == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var analogDevices = _context.AnalogDevice
            .Where(ad => ad.DeviceId == id)
            .ToList();

            var entriesWhereDeviceIsAnalog = _context.AnalogDevice
                .Where(ad => ad.AnalogId == id)
                .ToList();

            try
            {
                // Удаляем связанные аналоги устройства
                foreach (var analogDevice in analogDevices)
                {
                    _context.AnalogDevice.Remove(analogDevice);
                }
                // Удаляем записи где устройство является аналогом
                foreach (var entry in entriesWhereDeviceIsAnalog)
                {
                    _context.AnalogDevice.Remove(entry);
                }
                _context.Device.Remove(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException  ex )
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