using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Tasks
{
    public class AddTaskModel : PageModel
    {

        [BindProperty]
        public int ProjectID { get; set; }

        public string ProjectTitle { get; set; } = "";

        [BindProperty, Required]
        public string Description { get; set; } = "";

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required]
        public string Status { get; set; } = "Pending";

        public string Message { get; set; } = "";

        public void OnGet(int projectId)
        {
            ProjectID = projectId;
            ProjectTitle = DBClass.LoadProjectTitle(ProjectID) ?? "Unknown Project"; // Uses null-coalescing operator to set ProjectTitle to "Unknown Project" if LoadProjectTitle returns null. https://www.geeksforgeeks.org/null-coalescing-operator-in-c-sharp/ 
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ProjectTitle = DBClass.LoadProjectTitle(ProjectID) ?? "Unknown Project";
                return Page();
            }

            try
            {
                bool success = DBClass.AddTask(ProjectID, Description, DueDate, Status);

                if (success)
                {
                    Message = "Task added successfully!";
                    return RedirectToPage("/Admin/Projects/ProjectTaskManagement", new { id = ProjectID });
                }
                else
                {
                    Message = "Error adding task.";
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            ProjectTitle = DBClass.LoadProjectTitle(ProjectID) ?? "Unknown Project";
            return Page();
        }



    }
}
