namespace japanese_resturant_project.model.DatabaseModel
{
    public class Notification_tb
    {
        public string notificationID { get; set; }
        public string? title { get; set; }
        public string? message { get; set; }
        public string tableID { get; set; }
        public DateTime createDate { get; set; }
        public string isRead { get; set; }
        public string sentBy { get; set; }

    }
}
