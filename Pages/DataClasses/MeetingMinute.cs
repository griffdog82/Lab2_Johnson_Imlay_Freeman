namespace Lab2_Johnson_Imlay_Freeman.Pages.DataClasses
{
    public class MeetingMinute
    {
        public int MinuteID { get; set; }
        public int BusinessPartnerID { get; set; }
        public int RepresentativeID { get; set; }
        public int MeetingWithID { get; set; }
        public DateTime MeetingDate { get; set; }
        public string MinutesText { get; set; }
    }
}
