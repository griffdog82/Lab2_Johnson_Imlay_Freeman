using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Messages
{


    public class MessageListModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";
        [BindProperty]
        public int MessageID { get; set; }

        [BindProperty]
        public int UserID { get; set; }

        [BindProperty]

        public List<MessageModel> Messages { get; set; } = new();
        
        

        public void OnGet()
        {
            LoadMessages();
        }

        public IActionResult OnPostDelete(int MessageID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Message WHERE MessageID = @MessageID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MessageID", MessageID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting message: " + ex.Message);
            }

            LoadMessages();
            return Page();
        }

        private void LoadMessages()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT m.MessageID, u.FirstName + ' ' + u.LastName AS SenderName, 
                           m.Subject, m.Timestamp
                    FROM Message m
                    JOIN [User] u ON m.SenderID = u.UserID
                    WHERE m.RecipientID = @UserID
                    ORDER BY m.Timestamp DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", 1); // Replace with logged-in user ID later

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Messages.Add(new MessageModel
                            {
                                MessageID = reader.GetInt32(0),
                                SenderName = reader.GetString(1),
                                Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                                Timestamp = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }
        }

        public class MessageModel
        {
            public int MessageID { get; set; }
            public string SenderName { get; set; } = "";
            public string Subject { get; set; } = "";
            public DateTime Timestamp { get; set; }
        }
    }
}
