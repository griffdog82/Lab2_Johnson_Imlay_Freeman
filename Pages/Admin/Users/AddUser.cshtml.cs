using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Users
{
    public class AddUserModel : PageModel
    {

        [BindProperty, Required]
        public string Username { get; set; } = "";

        [BindProperty, Required]
        public string Password { get; set; } = "";

        [BindProperty]
        public string? Email { get; set; } = "";

        [BindProperty, Required]
        public string FirstName { get; set; } = "";

        [BindProperty, Required]
        public string LastName { get; set; } = "";

        [BindProperty, Required]
        public string UserType { get; set; } = "";

        [BindProperty]
        public string? Department { get; set; } = "";

        [BindProperty]
        public string? AdminType { get; set; } = "";

        [BindProperty]
        public int? BusinessPartnerID { get; set; } = null;

        public string Message { get; set; } = "";

        public List<DBClass.BusinessPartner> BusinessPartners { get; set; } = new();

        public void OnGet()
        {
            BusinessPartners = DBClass.LoadBusinessPartners();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                BusinessPartners = DBClass.LoadBusinessPartners();
                return Page();
            }

            try
            {
                bool success = DBClass.AddUser(Username, Password, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID);

                if (success)
                {
                    Message = "User added successfully!";
                    return RedirectToPage("/Admin/Users/ViewUsers");
                }
                else
                {
                    Message = "Error adding user.";
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            BusinessPartners = DBClass.LoadBusinessPartners();
            return Page();
        }






        //public class BusinessPartner
        //{
        //    public int BusinessPartnerID { get; set; }
        //    public string Name { get; set; } = "";
        //}
        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear(); // Clear validation state

            Username = "testuser";
            Password = "Password123";
            Email = "test@example.com";
            FirstName = "Test";
            LastName = "User";
            UserType = "RepOfBusiness"; // Setting this to ensure BusinessPartnerID is used
            AdminType = "Super Admin"; // Example Admin Type
            Department = "Engineering"; // Example Department

            // Reload business partners so dropdown doesn't break
            BusinessPartners = DBClass.LoadBusinessPartners();

            // Ensure BusinessPartnerID is set properly if "RepOfBusiness"
            if (UserType == "RepOfBusiness" && BusinessPartners.Count > 0)
            {
                BusinessPartnerID = BusinessPartners[0].BusinessPartnerID;
            }
            else
            {
                BusinessPartnerID = null;
            }

            return Page();
        }


    }
}
