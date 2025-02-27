namespace Lab2_Johnson_Imlay_Freeman.Pages.DataClasses
{
    public class Message
    {
        public int MessageID { get; set; }
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
