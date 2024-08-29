namespace japanese_resturant_project.model.DatabaseModel
{
    public class Authentication_tb
    {
        public Guid userID { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string roleName { get; set; }
    }
}
