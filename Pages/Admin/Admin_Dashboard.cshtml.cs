using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient; 


namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin
{
    public class Admin_DashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            string userRole = HttpContext.Session.GetString("UserRole");

            
            if (userRole == "Faculty")
            {
                return RedirectToPage("/Faculty/FacultyDashboard"); // Adjust destination as needed
            }
            else if(userRole == "RepOfBusiness")
            {
                return RedirectToPage("/Business/BusinessPartner_Dashboard");
            }
            else
            {
                return Page();
            }

        }
    }

}
