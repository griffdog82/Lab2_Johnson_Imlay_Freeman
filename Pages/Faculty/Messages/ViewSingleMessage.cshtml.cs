using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using static Lab2_Johnson_Imlay_Freeman.Pages.DB.DBClass;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Messages
{
    public class ViewSingleMessageModel : PageModel
    {
        public MessageModel? MessageDetails { get; set; } // ✅ Fixed by making it nullable

        public IActionResult OnGet(int messageID)
        {
            Console.WriteLine("DEBUG: Fetching message ID: " + messageID);

            MessageDetails = DBClass.GetMessageByID(messageID);

            if (MessageDetails == null)
            {
                Console.WriteLine("DEBUG: Message not found for ID: " + messageID);
                return RedirectToPage("MessageList"); // Redirect if message not found
            }

            Console.WriteLine("DEBUG: Successfully fetched message: " + MessageDetails.Subject);
            return Page();
        }
    }
}
