using japanese_resturant_project.services.Interfaces;
using japanese_resturant_project.model.response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using japanese_resturant_project.Controllers;
using japanese_resturant_project.model.request;
using Dapper;
using japanese_resturant_project.model.DatabaseModel;


namespace japanese_resturant_project.services.implement
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

        public async Task<UserResponseModel> AddRegister(RegisterRequest request)
        {
            var response = new UserResponseModel();
            var userID = Guid.NewGuid();
            var staftID = Guid.NewGuid();
            var memberID = Guid.NewGuid();
            var pointID = Guid.NewGuid();

            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    if(request.roleName == "ลูกค้า")
                    {
                        var sqlCustomer = @"INSERT INTO customer_tb (memberID, pointID, firstName,lastName, phone, totalPoint,currentPoint,description,createDate,updateDate,userID,email,password,roleName)
                        VALUES (@memberID, @pointID, @firstName,@lastName, @phone, @totalPoint,@currentPoint,@description,@createDate,@updateDate,@userID,@email,@password,@roleName)";
                        // Use parameterized query to prevent SQL injection
                        response.member = new Customer_Authentication_tb
                      {
                        memberID = memberID,
                        pointID = pointID,
                        firstName = request.firstName,
                        lastName = request.lastName,
                        phone = request.phone,
                        totalPoint = 10,
                        currentPoint = 10,
                        description = "ได้รับแต้ม",
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        userID = userID,
                        email = request.email,
                        password = request.password,
                        roleName = request.roleName,
                        };
                        var customerecord = await dbConnection.ExecuteAsync(sqlCustomer, response.member);
                        if (customerecord > 0)
                       {
                            // Set success message
                            response.message = "เพิ่มข้อมูลสำเร็จ";
                            response.success = true;
                        }
                         else
                        {
                            response.message = "เพิ่มข้อมูลไม่สำเร็จ";
                            response.success = false;
                        }

                    
                  }
                    else if(request.roleName == "พนักงาน")
                    {

                        var sqlAdmin = @"INSERT INTO staft_tb (staftID,firstName,lastName, phone,createDate,updateDate,userID,email,password,roleName)
                        VALUES (@staftID, @firstName,@lastName, @phone,@createDate,@updateDate,@userID,@email,@password,@roleName)";
                        // Use parameterized query to prevent SQL injection
                        response.account = new Staft_Authentication_tb
                        {
                            staftID = staftID,
                            firstName = request.firstName,
                            lastName = request.lastName,
                            phone = request.phone,
                            createDate = DateTime.Now,
                            updateDate = DateTime.Now,
                            userID = userID,
                            email = request.email,
                            password = request.password,
                            roleName = request.roleName,
                        };
                        var adminrecord = await dbConnection.ExecuteAsync(sqlAdmin, response.account);
                        if (adminrecord > 0)
                        {

                            // Set success message
                            response.message = "เพิ่มข้อมูลสำเร็จ";
                            response.success = true;
                        }
                        else
                        {
                            response.message = "เพิ่มข้อมูลไม่สำเร็จ";
                            response.success = false;
                        }

                    }
                
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Set error message in the bookingResponse
                response.message = $"An error occurred while adding the booking: {ex.Message}";
            }

            return response;
        }

    }
}
