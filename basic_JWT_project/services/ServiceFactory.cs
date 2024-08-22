using basic_JWT_project.services.implement;
using basic_JWT_project.services.Interfaces;
using japanese_resturant_project.services;
using japanese_resturant_project.services.implement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace basic_JWT_project.services
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
    }
}
