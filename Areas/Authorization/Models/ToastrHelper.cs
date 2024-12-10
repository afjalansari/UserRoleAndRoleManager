using Microsoft.AspNetCore.Mvc;

namespace FleetMasterPro013.Areas.Authorization.Models
{
    public static class ToastrHelper
    {
        public static void AddToastMessage(this Controller controller, string message, string type = "success")
        {
            controller.TempData[$"toast-{type}"] = message;
        }
    }

}
