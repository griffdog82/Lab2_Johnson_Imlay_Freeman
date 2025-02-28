using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Lab2_Johnson_Imlay_Freeman.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToPage("/Admin/Admin_Dashboard");
            }

            if (HttpContext.Session.GetString("LoginError") != null)
            {
                ViewData["LoginMessage"] = HttpContext.Session.GetString("LoginError");
                HttpContext.Session.Remove("LoginError");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (DBClass.AuthenticateUser(Username, Password))
            {
                Debug.WriteLine($"User {Username} authenticated!");
                // TODO: Remove Debug lines

                HttpContext.Session.SetString("Username", Username);
                return RedirectToPage("/Admin/Admin_Dashboard");
            }
            else
            {
                // TODO: Remove Debug lines
            Debug.WriteLine("Authentication failed.");
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                return Page();
            }
        }
    }
}
