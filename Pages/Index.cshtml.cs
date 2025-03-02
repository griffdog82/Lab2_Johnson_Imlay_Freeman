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
            // ✅ Handle Logout when user clicks "Logout" link
            if (Request.Query["handler"] == "Logout")
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Index"); // ✅ Redirect to login page after logout
            }

            // ✅ Redirect logged-in users to their dashboard based on role
            string userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                return RedirectToPage("/Admin/Admin_Dashboard");
            }
            else if (userRole == "Faculty")
            {
                return RedirectToPage("/Faculty/FacultyDashboard");
            }
            else if (userRole == "RepOfBusiness")  // ✅ Business Partners now get redirected
            {
                return RedirectToPage("/BusinessPartner/BusinessPartner_Dashboard");
            }

            // ✅ Display login error message if it exists
            if (HttpContext.Session.GetString("LoginError") != null)
            {
                ViewData["LoginMessage"] = HttpContext.Session.GetString("LoginError");
                HttpContext.Session.Remove("LoginError");
            }

            return Page();
        }



        public IActionResult OnPost()
        {
            if (DBClass.StoredProcedureLogin(Username, Password, HttpContext))
            {
                // ✅ Redirect users based on their role
                string userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "Admin")
                {
                    return RedirectToPage("/Admin/Admin_Dashboard");
                }
                else if (userRole == "Faculty")
                {
                    return RedirectToPage("/Faculty/FacultyDashboard");
                }
                else
                {
                    return RedirectToPage("/BusinessPartner/BusinessPartner_Dashboard");
                }
            }
            else
            {
                HttpContext.Session.SetString("Login Error", "Username and/or Password Incorrect");
                return RedirectToPage("/Index");
            }   
        }


    }
}
