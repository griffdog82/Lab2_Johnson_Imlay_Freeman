using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Messages
{
    public class ComposeMessageModel : PageModel
    {
        [BindProperty]
        public int RecipientID { get; set; }

        [BindProperty]
        public string Subject { get; set; } = "";

        [BindProperty]
        public string Body { get; set; } = "";

        [BindProperty]
        public int UserID { get; set; }// Replace with actual logged-in user ID

        public List<DBClass.UserModel> Users { get; set; } = new();
        public string Message { get; set; } = "";

        public void OnGet(int? replyTo)
        {
            Users = DBClass.LoadUsers();

            if (replyTo.HasValue)
            {
                var replyMessage = DBClass.LoadReplyMessage(replyTo.Value); // Fixed method name
                if (replyMessage.HasValue)
                {
                    Subject = replyMessage.Value.Subject;
                    Body = replyMessage.Value.Body;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Users = DBClass.LoadUsers();
                return Page();
            }

            if (DBClass.SendMessage(UserID, RecipientID, Subject, Body))
            {
                Message = "Message sent successfully!";
                return RedirectToPage("MessageList");
            }
            else
            {
                Message = "Error sending message.";
            }

            Users = DBClass.LoadUsers();
            return Page();
        }
    }
}
