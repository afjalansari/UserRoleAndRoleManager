using FleetMasterPro013.Data;
using Microsoft.AspNetCore.Mvc;

namespace FleetMasterPro013.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to get the real user count
        public IActionResult GetUserCount()
        {
            int userCount = _context.Users.Count(); // Replace with your user table logic
            return Json(new { count = userCount });
        }


        public IActionResult Index()
        {
            int userCount = _context.Users.Count(); // Get the user count for initial load
            ViewData["UserCount"] = userCount;
            return View();
        }
    }
}
