using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FleetMasterPro013.Areas.Authorization.Models;

namespace FleetMasterPro013.Areas.Authorization.Controllers
{
    [Area("Authorization")]
    public class UserRoleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            return View(userRolesViewModel);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "Invalid user ID.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ToastMessage"] = "User not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = await GetRoleSelectionAsync(user)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Invalid data submitted.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ToastMessage"] = "User not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesToAdd = model.Roles
                .Where(r => r.IsSelected && !currentRoles.Contains(r.RoleName))
                .Select(r => r.RoleName)
                .ToList();

            var rolesToRemove = currentRoles
                .Where(r => model.Roles.All(m => m.RoleName != r || !m.IsSelected))
                .ToList();

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    TempData["ToastMessage"] = "Error adding roles.";
                    TempData["ToastType"] = "error";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    TempData["ToastMessage"] = "Error removing roles.";
                    TempData["ToastType"] = "error";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["ToastMessage"] = $"Roles updated for user '{user.UserName}'.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<RoleSelection>> GetRoleSelectionAsync(IdentityUser user)
        {
            var allRoles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            return allRoles.Select(role => new RoleSelection
            {
                RoleName = role.Name,
                IsSelected = userRoles.Contains(role.Name)
            }).ToList();
        }
    }
}
