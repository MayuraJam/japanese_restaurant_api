using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.services;

namespace japanese_resturant_project.model.response
{
    public class UserResponseModel : Message
    {
        public User user { get; set; }
        public Customer_Authentication_tb member { get; set; }
        public Staft_Authentication_tb account { get; set; }
        public Authentication_tb authen {  get; set; }

    }
}
