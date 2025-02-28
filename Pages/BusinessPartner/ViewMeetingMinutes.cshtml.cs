using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Lab2_Johnson_Imlay_Freeman.Pages.DataClasses;

namespace Lab2_Johnson_Imlay_Freeman.Pages.BusinessPartner
{
    public class ViewMeetingMinutesModel : PageModel
    {
        public List<MeetingMinute> MeetingMinutes { get; set; } = new();

        public void OnGet()
        {
            MeetingMinutes = DBClass.LoadMeetingMinutes();
        }
    }
}
