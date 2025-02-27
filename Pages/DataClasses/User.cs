namespace Lab2_Johnson_Imlay_Freeman.Pages.DataClasses
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string? Department { get; set; } // Nullable
        public string? AdminType { get; set; } // Nullable
        public int? BusinessPartnerID { get; set; } // Nullable foreign key
    }
}
