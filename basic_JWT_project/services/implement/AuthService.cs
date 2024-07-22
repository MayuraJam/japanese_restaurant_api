using basic_JWT_project.services.Interfaces;
using basic_JWT_project.model.request;
using basic_JWT_project.model.response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using basic_JWT_project.Controllers;


namespace basic_JWT_project.services.implement
{
    public class AuthService : Bases, IAuth
    {
      public static User user2 = new User(); //response model
     
        public async Task<UserResponseModel> RegiterPost(UserRequestModel request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); //การแปลง password ให้เป็นรหัส Hash
            var response = new UserResponseModel();
            //การนำค่าใส่ลงใน Model
            response.user = new User() { 
               massage = "Success",
               PasswordHash = passwordHash,
               Username = request.Username, 
            };

            //response.Username = request.Username;
            //response.PasswordHash = passwordHash;
            //response.massage = "Success";
            
            return response;
        }

    }
}
