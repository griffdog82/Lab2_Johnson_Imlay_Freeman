using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Messages
{
    public class MessageListModel : PageModel
    {
        [BindProperty]
        public int MessageID { get; set; }

        [BindProperty]
        public int UserID { get; set; }

        [BindProperty]
        public List<DBClass.MessageModel> Messages { get; set; } = new();

        [BindProperty]
        public DBClass.MessageModel? SelectedMessage { get; set; }

        public List<DBClass.UserModel> Senders { get; set; } = new();
        public int? SelectedSenderID { get; set; }

        public void OnGet(int? id = null, int? senderId = null)
        {
            int userID = DBClass.GetCurrentUserID(HttpContext);
            Console.WriteLine("DEBUG: Retrieved UserID from session = " + userID);

            if (senderId == 0)
            {
                senderId = null;
            }

            Messages = DBClass.GetMessages(userID, senderId);
            Console.WriteLine("DEBUG: Retrieved " + Messages.Count + " messages.");

            Senders = DBClass.GetMessageSenders(userID);
            Console.WriteLine("DEBUG: Retrieved " + Senders.Count + " senders for UserID " + userID);

            SelectedSenderID = senderId;
        }
    }
}
