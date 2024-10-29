using japanese_resturant_project.model.request;
using japanese_resturant_project.model.response;
using Microsoft.AspNetCore.Mvc;

namespace japanese_resturant_project.services.Interfaces
{
    public interface IAuth
    {
        // public Task<UserResponseModel> RegiterPost(UserRequestModel request);
        public Task<UserResponseModel> AddRegister(RegisterRequest request);
        public Task<UserResponseModel> GetMember(string roleName);
        public Task<UserResponseModel> ToLoginCustomer(Login request); //แก้ไข
        public Task<UserResponseModel> LoginStaft(LoginStaftRequestModel request);
        public Task<UserResponseModel> LogoutStaft(string staftID);
        public Task<UserResponseModel> DeleteMemberAccount(Guid memberID);

    }
}
