using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Projects
{
    public class ProjectTaskManagementModel : PageModel
    {

        [BindProperty]
        public int ProjectID { get; set; }

        public string ProjectTitle { get; set; } = "";
        public List<DBClass.TaskModel> Tasks { get; set; } = new();

        public void OnGet(int id)
        {
            ProjectID = id;  // Ensure ProjectID is set dynamically
            ProjectTitle = DBClass.GetProjectTitle(ProjectID) ?? "Unknown Project";
            Tasks = DBClass.GetTasksByProjectID(ProjectID);
        }


        public IActionResult OnPostUpdateStatus(int TaskID, string NewStatus)
        {
            try
            {
                if (!DBClass.UpdateTaskStatus(TaskID, NewStatus))
                {
                    ModelState.AddModelError("", "Error updating task status.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Database error: " + ex.Message);
            }

            // Refresh project details and tasks after update
            ProjectTitle = DBClass.GetProjectTitle(ProjectID) ?? "Unknown Project";
            Tasks = DBClass.GetTasksByProjectID(ProjectID);

            return Page();
        }









    }
}
