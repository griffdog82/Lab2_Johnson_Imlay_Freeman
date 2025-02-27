using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using Lab2_Johnson_Imlay_Freeman.Pages.DB;

namespace Lab2_Johnson_Imlay_Freeman.Pages.Admin.Users
{
    public class ViewUsersModel : PageModel
    {

        public List<DBClass.UserModel> Users { get; set; } = new();

        public void OnGet()
        {
            Users = DBClass.LoadUsers();
        }

        

       
    }
}
