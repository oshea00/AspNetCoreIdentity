using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using identiity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identiity.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        UserManager<AppUser> userManager;
        IUserValidator<AppUser> userValidator;
        IPasswordValidator<AppUser> passwordValidator;
        IPasswordHasher<AppUser> passwordHasher;

        public AdminController(
            UserManager<AppUser> usrmgr,
            IUserValidator<AppUser> usrVal,
            IPasswordValidator<AppUser> pswdVal,
            IPasswordHasher<AppUser> pswdHash)
        {
            userManager = usrmgr;
            userValidator = usrVal;
            passwordValidator = pswdVal;
            passwordHasher = pswdHash;
        }

        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUser model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                } else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", userManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(new CreateUser {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email
                });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateUser u)
        {
            var user = await userManager.FindByIdAsync(u.Id);
            if (user != null)
            {
                user.Email = u.Email;
                var validEmail = await userValidator.ValidateAsync(userManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(u.Password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, u.Password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, u.Password);
                    } 
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null)||
                    (validEmail.Succeeded && u.Password != string.Empty && validPass.Succeeded))
                {
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(u);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}