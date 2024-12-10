using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FleetMasterPro013.Areas.Authorization.Models;

namespace FleetMasterPro013.Areas.Authorization.Controllers
{
    [Area("Authorization")]
    public class RolesManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: List all roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // GET: Create a new role
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create a new role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                this.AddToastMessage("Role name cannot be empty.", "warning");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                this.AddToastMessage($"Role '{roleName}' created successfully.", "success");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                this.AddToastMessage(error.Description, "error");
            }
            return View();
        }

        // GET: Delete confirmation modal
        public async Task<IActionResult> Delete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                this.AddToastMessage("Role not found.", "error");
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_DeleteConfirmation", role);
        }

        // POST: Confirm deletion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                this.AddToastMessage("Role not found.", "error");
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                this.AddToastMessage($"Role '{role.Name}' deleted successfully.", "success");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                this.AddToastMessage(error.Description, "error");
            }
            return RedirectToAction(nameof(Index));
        }

        private void AddToastMessage(string message, string type = "info")
        {
            TempData["ToastMessage"] = message;
            TempData["ToastType"] = type;
        }
    }
}
