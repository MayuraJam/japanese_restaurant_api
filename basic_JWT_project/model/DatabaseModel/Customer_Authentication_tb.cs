namespace japanese_resturant_project.model.DatabaseModel
{
    public class Customer_Authentication_tb
    {
        public Guid memberID { get; set; }
        public Guid pointID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string description { get; set; }
        public int currentPoint { get; set; }
        public DateTime createDate { get; set; }
        public Guid userID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string roleName { get; set; }
    }
}
