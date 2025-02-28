using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
 
namespace Lab2_Johnson_Imlay_Freeman.Pages.BusinessPartner
{
    public class ViewMessagesModel : PageModel
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

        public void OnGet(int? id, int? senderId)
        {
            // ✅ Fetch Business Partner UserID dynamically
            UserID = DBClass.GetCurrentUserID();

            // ✅ Ensure SelectedSenderID updates properly
            SelectedSenderID = senderId;

            // ✅ Fetch senders and messages
            Senders = DBClass.GetBusinessPartnerMessageSenders();
            Messages = DBClass.GetBusinessPartnerMessages(UserID, senderId);

            if (id.HasValue)
            {
                SelectedMessage = DBClass.GetBusinessPartnerMessageByID(id.Value);
            }
        }
    }
}