using basic_JWT_project.services.implement;
using basic_JWT_project.services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace basic_JWT_project.services
{

    public class serviceFactory
    {
       
        public IAuth AuthService()
        {
            return new AuthService();
        }
    }
}
