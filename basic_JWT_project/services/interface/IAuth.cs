using japanese_resturant_project.model.request;
using japanese_resturant_project.model.response;
using Microsoft.AspNetCore.Mvc;

namespace japanese_resturant_project.services.Interfaces
{
    public interface IAuth
    {
        // public Task<UserResponseModel> RegiterPost(UserRequestModel request);
        public Task<UserResponseModel> AddRegister(RegisterRequest request);


    }
}
