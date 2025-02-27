namespace Lab2_Johnson_Imlay_Freeman.Pages.DataClasses
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatedBy { get; set; }
        public int? BusinessPartnerID { get; set; }
        public int? GrantID { get; set; }
    }
}
