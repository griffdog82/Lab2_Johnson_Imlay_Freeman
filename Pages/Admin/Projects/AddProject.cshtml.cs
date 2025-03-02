using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;


namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Projects
{
    public class AddProjectModel : PageModel
    {
        

        [BindProperty, Required]
        public string Title { get; set; } = "";

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required]
        public int CreatedBy { get; set; }

        [BindProperty]
        public int? BusinessPartnerID { get; set; } = null;

        [BindProperty]
        public int? GrantID { get; set; } = null;

        [BindProperty]
        public int? AssignedFacultyID { get; set; } = null; // FIXED: Faculty Member property

        [BindProperty]
        public int AssigningAdminID { get; set; } = 1; // FIXED: Simulated Admin ID for now

        public string Message { get; set; } = "";

        public List<DBClass.UserModel> Users { get; set; } = new();
        public List<DataClasses.BusinessPartner> BusinessPartners { get; set; } = new();
        public List<DBClass.GrantModel> Grants { get; set; } = new();
        public List<DBClass.UserModel> FacultyMembers { get; set; } = new();


        public void OnGet()
        {
            Users = DBClass.LoadUsers();
            BusinessPartners = DBClass.LoadBusinessPartners();
            Grants = DBClass.LoadGrants();
            FacultyMembers = DBClass.LoadFacultyMembers();

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Users = DBClass.LoadUsers();
                BusinessPartners = DBClass.LoadBusinessPartners();
                Grants = DBClass.LoadGrants();
                FacultyMembers = DBClass.LoadFacultyMembers();
                return Page();
            }

            try
            {
                int projectID = DBClass.AddProject(Title, DueDate, CreatedBy, BusinessPartnerID, GrantID, AssignedFacultyID);

                Message = "Project added successfully!";
                return RedirectToPage("/Admin/Projects/ProjectList");
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            Users = DBClass.LoadUsers();
            BusinessPartners = DBClass.LoadBusinessPartners();
            Grants = DBClass.LoadGrants();
            FacultyMembers = DBClass.LoadFacultyMembers();
            return Page();
        }




        public class UserModel { public int UserID { get; set; } public string FullName { get; set; } = ""; }
        public class BusinessPartner { public int BusinessPartnerID { get; set; } public string Name { get; set; } = ""; }
        
    }
}
