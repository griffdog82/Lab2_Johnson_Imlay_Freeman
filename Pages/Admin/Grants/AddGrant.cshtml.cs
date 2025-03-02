using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using Lab2_Johnson_Imlay_Freeman.Pages.DataClasses;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin
{
    public class AddGrantModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";
        // TODO; Fix the connection string
        [BindProperty]
        public Grant Grant { get; set; } = new Grant();

        public List<FacultyModel> FacultyMembers { get; set; } = new();
        public List<BusinessPartnerModel> BusinessPartners { get; set; } = new();

        public void OnGet()
        {
            LoadFacultyMembers();
            LoadBusinessPartners();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadFacultyMembers();
                LoadBusinessPartners();
                return Page();
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO [GrantApplication] (GrantName, Category, FundingSource, SubmissionDate, AwardDate, Amount, Status, LeadFacultyID) " +
                               "VALUES (@GrantName, @Category, @FundingSource, @SubmissionDate, @AwardDate, @Amount, @Status, @LeadFacultyID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GrantName", Grant.GrantName);
                    command.Parameters.AddWithValue("@Category", Grant.Category);
                    command.Parameters.AddWithValue("@FundingSource", Grant.FundingSource);
                    command.Parameters.AddWithValue("@SubmissionDate", Grant.SubmissionDate);
                    command.Parameters.AddWithValue("@AwardDate", Grant.AwardDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Amount", Grant.Amount);
                    command.Parameters.AddWithValue("@Status", Grant.Status);
                    command.Parameters.AddWithValue("@LeadFacultyID", Grant.LeadFacultyID);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/BusinessPartner/ViewGrants");
        }

        private void LoadFacultyMembers()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'Faculty'", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FacultyMembers.Add(new FacultyModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }
        }

        private void LoadBusinessPartners()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BusinessPartners.Add(new BusinessPartnerModel
                        {
                            BusinessPartnerID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
        }

        public class FacultyModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
        }

        public class BusinessPartnerModel
        {
            public int BusinessPartnerID { get; set; }
            public string Name { get; set; } = "";
        }
    }
}
