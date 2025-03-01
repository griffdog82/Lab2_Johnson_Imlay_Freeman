//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;

//namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Grants
//{
//    public class MyGrantsModel : PageModel
//    {
//        public List<Grant> Grants { get; set; }

//        public void OnGet()
//        {
//            int FacultyId = GetFacultyId();  // Dynamically fetch FacultyId

//            Grants = GetGrantsByFaculty(FacultyId);
//        }

//        private int GetFacultyId()
//        {
//            // Logic to retrieve FacultyId for the logged-in user (this is a placeholder)
//            return 123; // Replace with actual logic to fetch FacultyId for the logged-in user
//        }

//        private List<Grant> GetGrantsByFaculty(int facultyId)
//        {
//            var grants = new List<Grant>();

//            using (SqlConnection conn = new SqlConnection("Server=localhost;Database=Lab1;Trusted_Connection=True;"))
//            {
//                conn.Open();

//                string query = @"
//                    SELECT GrantID, GrantName, Status
//                    FROM [GrantApplication]
//                    WHERE @FacultyID IN (LeadFacultyID, BusinessPartnerID)
//                    ORDER BY SubmissionDate DESC";

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            var grant = new Grant
//                            {
//                                GrantID = reader.GetInt32(0),
//                                GrantName = reader.GetString(1),
//                                Status = reader.GetString(2)
//                            };
//                            grants.Add(grant);
//                        }
//                    }
//                }
//            }

//            return grants;
//        }
//    }

//    // Grant model to hold the data retrieved from the database
//    public class Grant
//    {
//        public int GrantID { get; set; }
//        public string GrantName { get; set; }
//        public string Status { get; set; }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlClient;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Grants
{
    public class MyGrantsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? FacultyID { get; set; } // Selected Faculty ID from dropdown

        public List<DBClass.GrantModel> Grants { get; set; } = new();
        public List<SelectListItem> FacultyList { get; set; } = new();

        public void OnGet()
        {
            FacultyList = DBClass.LoadFacultyList(); // No need for try-catch here

            if (FacultyID.HasValue)
            {
                Grants = DBClass.GetGrantsByFaculty(FacultyID.Value);
            }
        }
    }
}

