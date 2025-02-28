using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Lab2_Johnson_Imlay_Freeman.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Lab2_Johnson_Imlay_Freeman.Pages.BusinessPartner
{
    public class ViewGrantsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? BusinessPartnerID { get; set; } // Selected Business Partner ID from dropdown

        public List<Grant> Grants { get; set; } = new();
        public List<SelectListItem> BusinessPartners { get; set; } = new();

        public void OnGet()
        {
            BusinessPartners = DBClass.LoadBusinessPartnerGrantsList(); // Load available business partners

            if (BusinessPartnerID.HasValue)
            {
                Grants = DBClass.LoadBusinessPartnerGrants(BusinessPartnerID.Value);
            }
        }
    }
}