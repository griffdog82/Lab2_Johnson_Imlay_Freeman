using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Messages
{
    public class ComposeMessageModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

        [BindProperty]
        public int RecipientID { get; set; }

        [BindProperty]
        public string Subject { get; set; } = "";

        [BindProperty]
        public string Body { get; set; } = "";

        [BindProperty]
        public int UserID { get; set; } // Replace with actual logged-in user ID


        public List<UserModel> Users { get; set; } = new();

        public string Message { get; set; } = "";

        public void OnGet(int? replyTo)
        {
            LoadUsers();

            if (replyTo.HasValue)
            {
                LoadReplyMessage(replyTo.Value);
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadUsers();
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Message (SenderID, RecipientID, Subject, Body) VALUES (@SenderID, @RecipientID, @Subject, @Body)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SenderID", 1); // Replace with logged-in user ID later
                        cmd.Parameters.AddWithValue("@RecipientID", RecipientID);
                        cmd.Parameters.AddWithValue("@Subject", Subject);
                        cmd.Parameters.AddWithValue("@Body", Body);
                        cmd.ExecuteNonQuery();
                    }
                }

                Message = "Message sent successfully!";
                return RedirectToPage("/Admin/Messages/MessageList");
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            LoadUsers();
            return Page();
        }

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User]", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }
        }
        private void LoadReplyMessage(int replyTo)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT Subject, Body FROM Message WHERE MessageID = @MessageID", conn))
                {
                    cmd.Parameters.AddWithValue("@MessageID", replyTo);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Subject = "RE: " + reader.GetString(0);
                            Body = "\n\n----- Original Message -----\n" + reader.GetString(1);
                        }
                    }
                }
            }
        }

        public class UserModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = "";
        }
        //public class IActionResult OnPostPopulate()
        //{ 
        //    ModelState.Clear();


        //    Return Page();)

        //}
    }
}
