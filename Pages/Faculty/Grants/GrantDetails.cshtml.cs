using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using static Lab2_Johnson_Imlay_Freeman.Pages.DB.DBClass;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Grants
{
    public class GrantDetailsModel : PageModel
    {
        public GrantModel? GrantDetails { get; set; }

        public void OnGet(int grantId)
        {
            GrantDetails = DBClass.GetGrantDetails(grantId);
        }

    }
}

