using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Profile
{
    public class EditProfileModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int? FacultyID { get; set; } //Display only

        [BindProperty]
        public string Username { get; set; } = ""; //Display only

        [BindProperty]
        public string UserType { get; set; } = ""; //Display only

        [BindProperty]
        public int? BusinessPartnerID { get; set; } //Display only

        [BindProperty, Required]
        public string FirstName { get; set; } = "";

        [BindProperty, Required]
        public string LastName { get; set; } = "";

        [BindProperty, Required]
        public string Email { get; set; } = "";

        [BindProperty, Required]
        public string Department { get; set; } = "";

        public List<(int, string)> FacultyList { get; set; } = new();

        public void OnGet()
        {
            // Load faculty list for selection
            using (SqlConnection conn = new SqlConnection("Server=localhost;Database=Lab1;Trusted_Connection=True;"))
            {
                conn.Open();
                string query = "SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'Faculty'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FacultyList.Add((reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
        }

        public void OnPostLoadProfile()
        {
            if (FacultyID == null) return; // Prevent errors

            using (SqlConnection conn = new SqlConnection("Server=localhost;Database=Lab1;Trusted_Connection=True;"))
            {
                conn.Open();
                string query = "SELECT Username, UserType, FirstName, LastName, Email, Department FROM [User] WHERE UserID = @FacultyID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FacultyID", FacultyID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Username = reader.GetString(0);
                            UserType = reader.GetString(1);
                            FirstName = reader.GetString(2);
                            LastName = reader.GetString(3);
                            Email = reader.GetString(4);
                            Department = reader.GetString(5);
                        }
                    }
                }
            }
            OnGet(); // Reload faculty list for dropdown
        }

        public IActionResult OnPostUpdateProfile()
        {
            if (FacultyID == null) return Page(); // Prevent errors

            using (SqlConnection conn = new SqlConnection("Server=localhost;Database=Lab1;Trusted_Connection=True;"))
            {
                conn.Open();
                string query = @"
                    UPDATE [User]
                    SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Department = @Department
                    WHERE UserID = @FacultyID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Department", Department);
                    cmd.Parameters.AddWithValue("@FacultyID", FacultyID);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Message"] = "Profile updated successfully!";
            return RedirectToPage("/Faculty/FacultyDashboard");
        }
    }
}