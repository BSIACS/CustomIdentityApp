using CustomIdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() => View(_roleManager.Roles.ToList());

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var result = await _roleManager.CreateAsync(new IdentityRole(name));

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(name);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id) {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                return NotFound();
        }
    }
}
