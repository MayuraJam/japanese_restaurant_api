using japanese_resturant_project.services.implement;
using japanese_resturant_project.services.Interfaces;


namespace japanese_resturant_project.services
{

    public class ServiceFactory
    {
       
        public IAuth AuthService()
        {
            return new AuthService();
        }
        public Itest TestService()
        {
            return new TestService();
        }
        public ICustomer CustomerService()
        {
            return new CustomerService();
        }
    }
}
