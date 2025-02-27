using Lab2_Johnson_Imlay_Freeman.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Tasks
{
    public class EditTaskModel : PageModel
    {
        

        [BindProperty]
        public int TaskID { get; set; }

        [BindProperty]
        public int ProjectID { get; set; }

        [BindProperty, Required]
        public string Description { get; set; } = "";

        [BindProperty, Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required]
        public string Status { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet(int id)
        {
            TaskID = id;
            DBClass.TaskModel? task = DBClass.GetTask(TaskID);
            if (task != null)
            {
                ProjectID = task.ProjectID;
                Description = task.Description;
                DueDate = task.DueDate;
                Status = task.Status;
            }
            else
            {
                Message = "Task not found.";
            }

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                bool success = DBClass.EditTask(TaskID, Description, DueDate, Status);

                if (success)
                {
                    Message = "Task updated successfully!";
                    return RedirectToPage("/Admin/Projects/ProjectTaskManagement", new { id = ProjectID });
                }
                else
                {
                    Message = "Error updating task.";
                }
            }
            catch (Exception ex)
            {
                Message = "Database Error: " + ex.Message;
            }

            return Page();
        }


        
    }
}
