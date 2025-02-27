using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Projects
{
    public class ProjectListModel : PageModel
    {
        public List<DBClass.ProjectModel> Projects { get; set; } = new();
        public List<DBClass.UserModel> FacultyMembers { get; set; } = new();
        public int? SelectedFacultyID { get; set; }

        public void OnGet(int? facultyId)
        {
            SelectedFacultyID = facultyId;
            FacultyMembers = DBClass.GetFacultyMembers();
            Projects = DBClass.GetProjectsByFacultyID(facultyId);
        }
    }
}
