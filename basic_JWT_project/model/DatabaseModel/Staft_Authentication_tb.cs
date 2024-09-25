namespace japanese_resturant_project.model.DatabaseModel
{
    public class Staft_Authentication_tb
    {
        public string staftID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        public Guid userID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string roleName {  get; set; }
        public string accountStatus { get; set; }
        public string? jobType { get; set; }
    }
}
