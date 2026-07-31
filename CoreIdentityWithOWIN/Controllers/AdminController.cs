using CoreIdentityWithOWIN.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreIdentityWithOWIN.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users =await _userManager.Users.ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> ListRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
           if (!string.IsNullOrEmpty(roleName))
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
                return RedirectToAction("ListRoles");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            await _roleManager.DeleteAsync(role);
            return RedirectToAction("ListRoles");
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var model = new ManageUserRoleViewModel
            {
                UserId = userId,
                Email = user.Email
            };
            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                var roleSelection = new RoleSelection { RoleName = role.Name };
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    roleSelection.IsSelected = true;
                }
                model.Roles.Add(roleSelection);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(ManageUserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();
            var roles=await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Roles.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return View(model);
            }
            return RedirectToAction("Index");
        }
    }
}
