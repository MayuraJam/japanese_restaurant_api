namespace japanese_resturant_project.model.DatabaseModel
{
    public class Point_tb
    {
        public string pointID { get; set; }
        public string description { get; set; }
        public int currentPoint { get; set; }
        public DateTime createDate { get; set; }
        public Guid userID { get; set; }
        public string email { get; set; }
        public string password { get; set; }

    }
}
