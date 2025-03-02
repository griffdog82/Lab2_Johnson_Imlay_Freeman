using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Messages
{
    public class MessageListModel : PageModel
    {
    

        [BindProperty]
        public int MessageID { get; set; }

        [BindProperty]
        public int UserID { get; set; } // Replace with actual logged-in user ID

        [BindProperty]
        public List<DBClass.MessageModel> Messages { get; set; } = new();

        [BindProperty]
        public DBClass.MessageModel? SelectedMessage { get; set; } // Holds a single message when viewing

        public List<DBClass.UserModel> Senders { get; set; } = new();
        public int? SelectedSenderID { get; set; }

        public void OnGet(int? id, int? senderId)
        {
            // ✅ Fetch UserID dynamically from DB
            UserID = DBClass.GetCurrentUserID();  // Replace with actual method

            // ✅ Ensure SelectedSenderID updates properly
            SelectedSenderID = senderId;

            // ✅ Debugging output (Remove after verifying)
            Console.WriteLine($"Fetched UserID: {UserID}, SelectedSenderID: {SelectedSenderID}");

            // ✅ Fetch senders and messages
            Senders = DBClass.GetMessageSenders();
            Messages = DBClass.GetMessages(UserID, senderId);

            if (id.HasValue)
            {
                SelectedMessage = DBClass.GetMessageByID(id.Value);
            }
        }








    }
}
