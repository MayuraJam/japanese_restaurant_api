namespace japanese_resturant_project.model.request
{
    public class RegisterRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string roleName { get; set; }
        public string? jobType { get; set; }

    }
    public class Login
    {
        //public string email { set; get; }
       // public string password { set; get; }
        public string phone { set; get; }
        public string roleName {  set; get; }
        public decimal? totalPrice { set; get; }
        public string pointType { set; get; }

    }

    public class LoginStaftRequestModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string roleName { get; set; }


    }
}
