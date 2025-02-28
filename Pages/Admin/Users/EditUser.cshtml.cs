using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Users
{
    public class EditUserModel : PageModel
    {
        // TODO: @Griffin -> I need to ensure that this is the mimimum amount of code needed to make this work.
        [BindProperty]
        public int UserID { get; set; }

        [BindProperty, Required]
        public string Username { get; set; } = "";

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

        public List<DataClasses.BusinessPartner> BusinessPartners { get; set; } = new();

        public void OnGet(int id)
        {
            UserID = id;
            DBClass.LoadUsers();
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
                bool success = DBClass.EditUser(UserID, Username, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID);

                if (success)
                {
                    Message = "User updated successfully!";
                    return RedirectToPage("/Admin/Users/ViewUsers");
                }
                else
                {
                    Message = "Error updating user.";
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            BusinessPartners = DBClass.LoadBusinessPartners();
            return Page();
        } // TODO From Griffin -> Explanation for @Nicole/@Zach: This method will first check if the model state is valid. If it is not, it will reload the business partners and return the page. If it is valid, it will attempt to edit the user in the database. If the edit is successful, it will redirect to the ViewUsers page. If the edit is not successful, it will set the message to "Error updating user." and return the page. If there is an exception, it will set the message to "Database Error: " + the exception message and return the page.





        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();

            return Page();
        }
    }
}
