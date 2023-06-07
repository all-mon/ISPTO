using Diplom.Data;
using Diplom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        // GET: AdminController/Edit/5
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (String.IsNullOrEmpty(id) || user == null)
            {
                return Problem("Id or user is null.");
            }
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

            if(!String.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                try
                {
                    if (!String.IsNullOrEmpty(userRole))
                    {
                        var ur = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user,ur);
                        await _userManager.AddToRoleAsync(user, userRole);
                    }
                        
                  return RedirectToAction(nameof(GetUserList));
                }
                catch
                {
                    return View();
                }
            }
            return NotFound();
        }

        // GET: AdminController/Delete/5
        
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        // POST: AdminController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null) 
                {
                    await _userManager.DeleteAsync(user);
                }
                
                return RedirectToAction(nameof(GetUserList));
            }
            catch
            {
                return View();
            }
        }
    }
}
