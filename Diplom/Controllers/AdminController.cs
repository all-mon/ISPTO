using Diplom.Data;
using Diplom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Diplom.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly DiplomContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(DiplomContext context,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> GetUserList()
        {
            return _context.IdentityUser != null ? View(await _context.IdentityUser.ToListAsync()) : View();
            
        }

        // GET: AdminController/Details/5
        public ActionResult Details(string id)
        {
           

            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            var roles = _roleManager.Roles.ToList();
            SelectList roleSelectList = new SelectList(roles);
            ViewBag.RoleSelectList = roleSelectList;
            return View(user);
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserAsync(string id, string userRole)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            

            try
            {
                await _userManager.AddToRoleAsync(user,userRole);
                return RedirectToAction(nameof(GetUserList));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
