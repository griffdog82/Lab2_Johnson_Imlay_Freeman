using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lab2_Johnson_Imlay_Freeman.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                // Redirect logged-in users to the dashboard
                Response.Redirect("/Admin/Admin_Dashboard");
            }
            
            // Show login message if redirected from DBLogin
            if (HttpContext.Session.GetString("LoginError") != null)
            {
                ViewData["LoginMessage"] = HttpContext.Session.GetString("LoginError");
                HttpContext.Session.Remove("LoginError"); // Clear message after displaying
            }
        }
    }
}
