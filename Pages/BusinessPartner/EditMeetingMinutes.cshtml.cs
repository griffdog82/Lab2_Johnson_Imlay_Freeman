using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using Lab2_Johnson_Imlay_Freeman.Pages.DataClasses;

namespace Lab2_Johnson_Imlay_Freeman.Pages.BusinessPartner
{
    public class EditMeetingMinutesModel : PageModel
    {
        public DataClasses.MeetingMinute MeetingMinute { get; set; }

        public void OnGet(int minuteId)
        {
            string connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        MinuteID, 
                        BusinessPartnerID, 
                        RepresentativeID, 
                        MeetingWithID, 
                        MeetingDate, 
                        MinutesText 
                    FROM MeetingMinute 
                    WHERE MinuteID = @MinuteID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MinuteID", minuteId);  // minuteId as query parameter
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MeetingMinute = new DataClasses.MeetingMinute
                        {
                            MinuteID = reader.GetInt32(0),
                            BusinessPartnerID = reader.GetInt32(1),
                            RepresentativeID = reader.GetInt32(2),
                            MeetingWithID = reader.GetInt32(3),
                            MeetingDate = reader.GetDateTime(4),
                            MinutesText = reader.GetString(5)
                        };
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            string connectionString = "Server=localhost;Database=Lab1;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE MeetingMinute 
                    SET 
                        MeetingDate = @MeetingDate, 
                        MinutesText = @MinutesText 
                    WHERE MinuteID = @MinuteID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MinuteID", MeetingMinute.MinuteID);
                command.Parameters.AddWithValue("@MeetingDate", MeetingMinute.MeetingDate);
                command.Parameters.AddWithValue("@MinutesText", MeetingMinute.MinutesText);

                command.ExecuteNonQuery();
            }

            // Redirect to the View Meeting Minutes page after saving changes
            return RedirectToPage("/BusinessPartner/ViewMeetingMinutes");
        }
    }
}
