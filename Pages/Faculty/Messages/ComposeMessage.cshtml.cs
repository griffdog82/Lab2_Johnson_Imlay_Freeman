using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Messages
{
    public class ComposeMessageModel : PageModel
    {
        [BindProperty]
        public int RecipientID { get; set; } = 0; // Default to 0 (no recipient selected)

        [BindProperty]
        public string Subject { get; set; } = "";

        [BindProperty]
        public string Body { get; set; } = "";

        public List<DBClass.UserModel> Users { get; set; } = new();
        public string Message { get; set; } = "";
        public int UserID { get; private set; }

        public void OnGet(int? replyTo)
        {
            Users = DBClass.LoadUsers();
            UserID = DBClass.GetCurrentUserID(HttpContext); // Get logged-in user ID

            if (UserID == 0)
            {
                Console.WriteLine("DEBUG: No valid session detected, redirecting to Login.");
                RedirectToPage("/Login");
            }

            if (replyTo.HasValue)
            {
                // Load message details for the reply
                var replyMessage = DBClass.GetMessageByID(replyTo.Value);
                if (replyMessage != null)
                {
                    RecipientID = replyMessage.SenderID; // Set recipient as the original sender
                    Subject = "RE: " + replyMessage.Subject;
                    Body = "\n\n----- Original Message -----\n" + replyMessage.Body;
                }
            }

            Console.WriteLine($"DEBUG: ComposeMessage loaded. UserID={UserID}, RecipientID={RecipientID}");
        }

        public IActionResult OnPost()
        {
            int senderID = DBClass.GetCurrentUserID(HttpContext);
            Console.WriteLine($"DEBUG: Attempting to send message from UserID={senderID} to RecipientID={RecipientID}");

            if (senderID == 0)
            {
                Console.WriteLine("DEBUG: No valid session, redirecting to Login.");
                return RedirectToPage("/Login");
            }

            if (RecipientID == 0)
            {
                Console.WriteLine("DEBUG: No recipient selected.");
                ModelState.AddModelError("RecipientID", "Please select a recipient.");
                Users = DBClass.LoadUsers();
                return Page();
            }

            bool success = DBClass.SendMessage(senderID, RecipientID, Subject, Body);

            if (success)
            {
                Console.WriteLine("DEBUG: Message sent successfully.");
                return RedirectToPage("MessageList");
            }
            else
            {
                Console.WriteLine("DEBUG: Failed to send message.");
                Message = "Error sending message.";
                Users = DBClass.LoadUsers();
                return Page();
            }
        }
    }
}
