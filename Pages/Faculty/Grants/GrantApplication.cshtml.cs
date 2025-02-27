using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Faculty.Grants
{
    public class GrantApplication : PageModel
    {
        [BindProperty, Required]
        public string Category { get; set; } = "";

        [BindProperty, Required]
        public string GrantName { get; set; } = "";

        [BindProperty, Required]
        public string FundingSource { get; set; } = "";

        [BindProperty, Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime? AwardDate { get; set; }

        [BindProperty, Required]
        public decimal Amount { get; set; }

        [BindProperty, Required]
        public string? LeadFacultyID { get; set; } // Now a string? to match SQL change

        [BindProperty]
        public string? BusinessPartnerID { get; set; } // Now a string? to match SQL change

        public List<SelectListItem> FacultyMembers { get; set; } = new();
        public List<SelectListItem> BusinessPartners { get; set; } = new();

        public string Message { get; set; } = "";

        public void OnGet()
        {
            // Load Faculty Members
            FacultyMembers = DBClass.GetFacultyMembers().ConvertAll(faculty =>
                new SelectListItem { Value = faculty.UserID.ToString(), Text = $"{faculty.FirstName} {faculty.LastName}" });  //looked online on how to concatenate

            // Load Business Partner Representatives
            BusinessPartners = DBClass.LoadBusinessPartners().ConvertAll(partner =>
                new SelectListItem { Value = partner.BusinessPartnerID.ToString(), Text = partner.Name });
        }

        public IActionResult OnPost([FromForm] string action)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string status = action == "submit" ? "Submitted" : "Draft";

            try
            {
                int result = DBClass.InsertGrantApplication(Category, GrantName, FundingSource, SubmissionDate,
                    AwardDate, Amount, LeadFacultyID, BusinessPartnerID, status);

                if (result > 0)
                {
                    TempData["Message"] = action == "submit" ? "Application submitted successfully!" : "Draft saved successfully!";
                    return RedirectToPage("/Faculty/FacultyDashboard");
                }
                else
                {
                    TempData["Message"] = "Application submitted failed!";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Database Error: " + ex.Message;
                return Page();
            }
        }
    }
}





