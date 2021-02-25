using CustomIdentityApp.Models;
using CustomIdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel viewModel) {
            if (ModelState.IsValid) {
                User user = new User { Email = viewModel.Email, UserName = viewModel.Email, Year = viewModel.Year };

                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(string id) {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            EditUserViewModel viewModel = new EditUserViewModel { Id = user.Id, Email = user.Email, Year = user.Year };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel viewModel) {
            if (ModelState.IsValid) {
                User user = await _userManager.FindByIdAsync(viewModel.Id);

                if (user != null) {
                    user.Email = viewModel.Email;
                    user.UserName = viewModel.Email;
                    user.Year = viewModel.Year;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else {
                        foreach (var error in result.Errors) {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> ChangePassword(string id) {
            User user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            ChangePasswordViewModel viewModel = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel) {
            if (ModelState.IsValid) { 
                User user = await _userManager.FindByIdAsync(viewModel.Id);

                var passwordValidator = 
                    HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                var _passwordHasher =
                    HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                var result = await passwordValidator.ValidateAsync(_userManager, user, viewModel.NewPassword);

                if (result.Succeeded)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, viewModel.NewPassword);
                    await _userManager.UpdateAsync(user);
                }
                else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View();
        }

        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

    }
}
