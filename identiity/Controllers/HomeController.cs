using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using identiity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static identiity.Models.AppUser;

namespace identiity.Controllers
{
    public class HomeController : Controller
    {
        UserManager<AppUser> userManager;
        public Task<AppUser> CurrentUser => 
            userManager.FindByNameAsync(User.Identity.Name);

        public HomeController(UserManager<AppUser> usrmgr)
        {
            userManager = usrmgr;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(GetData(nameof(Index)));
        }

        //[Authorize(Roles = "Users")]
        [Authorize(Policy = "DCUsers")]
        public IActionResult OtherAction() => View("Index",GetData(nameof(OtherAction)));

        private Dictionary<string,object> GetData(string actionName)
        {
            return new Dictionary<string, object>
            {
                ["Action"] = actionName,
                ["User"] = HttpContext.User.Identity.Name,
                ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
                ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
                ["Is Users Role"] = HttpContext.User.IsInRole("Users"),
                ["City"] = CurrentUser.Result.City,
                ["Qualification"] = CurrentUser.Result.Qualifications,
            };
        }

        [Authorize]
        public async Task<IActionResult> UserProps()
        {
            return View(await CurrentUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserProps(
            [Required] Cities city,
            [Required] QualificationLevels qualifications
            )
        {
            if (ModelState.IsValid)
            {
                var user = await CurrentUser;
                user.City = city;
                user.Qualifications = qualifications;
                await userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View(await CurrentUser);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
