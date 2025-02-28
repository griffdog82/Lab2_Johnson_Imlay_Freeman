using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Lab2_Johnson_Imlay_Freeman.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.BusinessPartner
{
    public class AddMeetingMinutesModel : PageModel
    {
        [BindProperty]
        public DataClasses.MeetingMinute MeetingMinute { get; set; } = new();

        public List<DataClasses.BusinessPartner> BusinessPartners { get; set; } = new();
        public List<DataClasses.User> Representatives { get; set; } = new();
        public List<DataClasses.User> Users { get; set; } = new();

        public void OnGet()
        {
            LoadDropdowns();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return Page();
            }

            bool success = DBClass.InsertMeetingMinutes(MeetingMinute, out int minuteID);
            if (success)
            {
                return RedirectToPage("/BusinessPartner/ViewMeetingMinutes", new { id = minuteID });
            }
            else
            {
                ModelState.AddModelError("", "Error saving meeting minutes. Please try again.");
                LoadDropdowns();
                return Page();
            }
        }

        private void LoadDropdowns()
        {
            BusinessPartners = DBClass.LoadMeetingMinutesBusinessPartners();
            Representatives = DBClass.LoadMeetingMinutesRepresentatives();
            Users = DBClass.LoadMeetingMinutesUsers();
        }
    }
}

