//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.ComponentModel.DataAnnotations;
//using System.Data.SqlClient;

//namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.BusinessPartners
//{
//    public class AddBusinessPartnerModel : PageModel
//    {
//        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

//        [BindProperty, Required]
//        public string Name { get; set; } = "";

//        [BindProperty, Required]
//        public string OrgType { get; set; } = "";

//        [BindProperty, Required]
//        public string BusinessType { get; set; } = "";

//        [BindProperty, Required]
//        public string StatusFlag { get; set; } = "";

//        // ActiveStatus is an int, with 1 = Active, 0 = Inactive
//        [BindProperty]
//        public int ActiveStatus { get; set; } = 1;  // Default to Active (1)

//        public string Message { get; set; } = "";

//        public IActionResult OnPost()
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(_connectionString))
//                {
//                    conn.Open();
//                    string query = @"
//                        INSERT INTO BusinessPartner (Name, OrgType, BusinessType, StatusFlag, ActiveStatus) 
//                        VALUES (@Name, @OrgType, @BusinessType, @StatusFlag, @ActiveStatus)";

//                    using (SqlCommand cmd = new SqlCommand(query, conn))
//                    {
//                        cmd.Parameters.AddWithValue("@Name", Name);
//                        cmd.Parameters.AddWithValue("@OrgType", OrgType);
//                        cmd.Parameters.AddWithValue("@BusinessType", BusinessType);
//                        cmd.Parameters.AddWithValue("@StatusFlag", StatusFlag);
//                        cmd.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); // Insert 1 for Active, 0 for Inactive

//                        int rowsAffected = cmd.ExecuteNonQuery();
//                        if (rowsAffected > 0)
//                        {
//                            Message = "Business Partner added successfully!";
//                            return RedirectToPage("/Admin/BusinessPartner/AddBusinessPartner");
//                        }
//                        else
//                        {
//                            Message = "Error adding business partner.";
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Message = "Database Error: " + ex.Message;
//            }

//            return Page();
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.BusinessPartners
{
    public class AddBusinessPartnerModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty, Required]
        public string Name { get; set; } = "";

        [BindProperty, Required]
        public string OrgType { get; set; } = "";

        [BindProperty, Required]
        public string BusinessType { get; set; } = "";

        [BindProperty, Required]
        public string StatusFlag { get; set; } = "";

        [BindProperty]
        public int PrimaryContact { get; set; } // Stores the selected business representative's UserID

        [BindProperty]
        public int ActiveStatus { get; set; } = 1;  // Default to Active (1)

        public List<BusinessRep> BusinessReps { get; set; } = new List<BusinessRep>(); // Stores business representatives

        public string Message { get; set; } = "";

        public void OnGet()
        {
            LoadBusinessReps();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadBusinessReps(); // Reload reps in case of form error
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO BusinessPartner (Name, OrgType, BusinessType, StatusFlag, PrimaryContact, ActiveStatus) 
                        VALUES (@Name, @OrgType, @BusinessType, @StatusFlag, @PrimaryContact, @ActiveStatus)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@OrgType", OrgType);
                        cmd.Parameters.AddWithValue("@BusinessType", BusinessType);
                        cmd.Parameters.AddWithValue("@StatusFlag", StatusFlag);
                        cmd.Parameters.AddWithValue("@PrimaryContact", PrimaryContact);
                        cmd.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Message = "Business Partner added successfully!";
                            return RedirectToPage("/Admin/BusinessPartner/ViewBusinessPartners");
                        }
                        else
                        {
                            Message = "Error adding business partner.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            LoadBusinessReps(); // Ensure dropdown is populated again if there's an error
            return Page();
        }

        private void LoadBusinessReps()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'RepOfBusiness'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BusinessReps.Add(new BusinessRep
                            {
                                UserID = reader.GetInt32(0),
                                FullName = reader.GetString(1)
                            });
                        }
                    }
                }
            }
        }
    }

    public class BusinessRep
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = "";
    }
}
