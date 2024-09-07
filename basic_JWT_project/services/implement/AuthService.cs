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
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Azure;
using japanese_resturant_project.model.response.adminResponse;
using System.Reflection.Metadata.Ecma335;


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
                    var sqlAuth = @"INSERT INTO Authentication_tb(userID,email,password,roleName)
                                    VALUES(@userID,@email,@password,@roleName)";
                    var parameters = new
                    {
                        userID = userID,
                        email = request.email,
                        password = request.password,
                        roleName = request.roleName,
                    };
                    dbConnection.Execute(sqlAuth, parameters);

                    if (request.roleName == "ลูกค้า")
                    {
                        var sqlCustomer = @"INSERT INTO customer_tb (memberID, firstName,lastName, phone,userID,email,password,roleName)
                        VALUES (@memberID,@firstName,@lastName, @phone,@userID,@email,@password,@roleName)";
                        var parameterCustomer = new
                        {
                            memberID = memberID,
                            firstName = request.firstName,
                            lastName = request.lastName,
                            phone = request.phone,
                            userID = userID,
                            email = request.email,
                            password = request.password,
                            roleName = request.roleName,

                        };
                        dbConnection.Execute(sqlCustomer,parameterCustomer); //นำค่าใส่ตารางลูกค้า

                        var sqlpoint = @"INSERT INTO point_tb (pointID,currentPoint,description,createDate,userID)
                        VALUES (@pointID,@currentPoint,@description,@createDate,@userID)";
                        // Use parameterized query to prevent SQL injection
                        var parameterpoint = new
                        {
                            pointID = pointID,
                            currentPoint = 10,
                            description = "ได้รับแต้ม",
                            createDate = DateTime.Now,
                            userID = userID
                        };
                        var customerecord = dbConnection.Execute(sqlpoint,parameterpoint); //นำค่าใส่ตารางคะแนน

                        if (customerecord > 0)
                       {
                        response.member = new Customer_Authentication_tb
                        {
                            memberID = memberID,
                            pointID = pointID,
                            firstName = request.firstName,
                            lastName = request.lastName,
                            phone = request.phone,
                            currentPoint = 10,
                            description = "ได้รับแต้ม",
                            createDate = DateTime.Now,
                            userID = userID,
                            email = request.email,
                            password = request.password,
                            roleName = request.roleName,
                        };
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

                        var sqlAdmin = @"INSERT INTO staft_tb (staftID,firstName,lastName, phone,createDate,updateDate,userID,email,password,roleName,accountStatus)
                        VALUES (@staftID, @firstName,@lastName, @phone,@createDate,@updateDate,@userID,@email,@password,@roleName,@accountStatus)";
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
                            accountStatus = "อยู่ในระบบ"
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

        //login ลูกค้าพร้อมยอดซื้อทั้งหมด & login พนักงาน
      public async Task<UserResponseModel> ToLogin(Login request)
        {
           var response = new UserResponseModel();
           var pointID = Guid.NewGuid();


            var SqlAutn = @"SELECT * FROM Authentication_tb WHERE email = @email AND password = @password";
           //ส่วนแสดงข้อมูลของ ตาราง AUthentication 
            using (var dbConnection = CreateSQLConnection())
            {
                var authValue = await dbConnection.QueryFirstOrDefaultAsync<Authentication_tb>(SqlAutn, new { email = request.email, password = request.password });
                if (authValue != null)
                {
                   if(authValue.email == request.email && authValue.password == request.password && authValue.roleName =="ลูกค้า")  //ตรวจสอยเงื่อนไขทั้ง 3 
                    {
                        var sqlCustomer = @"IF NOT EXISTS (
                                            SELECT @userID
                                            FROM customer_tb
                                            WHERE email = @email AND password = @password
                                            ) 
                                            BEGIN
                                            INSERT INTO point_tb (pointID, currentPoint,description,createDate,userID)
                                            VALUES (@pointID, @currentPoint,@description,@createDate,@userID);
                                            END;";

                        //อัปเดตในส่วนของลูกค้า
                        var parameterpoint = new
                        {
                            email = request.email,
                            password = request.password,
                            pointID = pointID,
                            userID = authValue.userID,
                            currentPoint = 20,
                            description = "ได้รับแต้ม",
                            createDate = DateTime.Now,
                        };
                        var pointValue = await dbConnection.ExecuteAsync(sqlCustomer, parameterpoint);

                        if(pointValue > 0)
                        {
                           // response.pointitem = pointValue;
                            response.message = "เพิ่มข้อมูลสำเร็จ";
                            response.success = true;
                        }
                        else
                        {
                            response.message = "เพิ่มข้อมูลไม่สำเร็จ";
                            response.success = false;
                        }

                    }
                    else
                    {
                        var response2 = new UserResponseModel
                        {
                            message = "ไม่พบเจอบัญชีนี้",
                            success = false
                        };
                        return response2;
                    }

                }
                else if(authValue.email == request.email && authValue.password == request.password && authValue.roleName == "พนักงาน")
                {
                    try
                    {
                        
                            var sql = @"
                          SELECT 
                          staftID,firstName,lastName,phone,userID,createDate,updateDate,userID,email,password,roleName
                          FROM  staft_tb WHERE email = @email AND password = @password AND roleName = @roleName
                          ";
                            var parameterStaft = new
                            {
                                email = authValue.email,
                                password = authValue.password,
                                roleName = authValue.roleName,
                            };
                            var memberValue = await dbConnection.QueryFirstAsync<Staft_Authentication_tb>(sql,parameterStaft);

                            // Check if any reservations were found
                            if (memberValue  != null)
                            {


                                response.account = memberValue;
                                response.message = "successfully.";
                                response.success = true;

                            }
                            else
                            {

                                // Handle case where no reservations were found
                                response.message = "Not found data 404.";
                                response.success = false;

                            }
                        
                    }
                    catch (Exception ex)
                    {
                        // Handle case where no reservations were found
                        response.message = $"{ex}";
                        response.success = false;

                    }
                }
            }
                return response;
        }

        /* static int calculatePoint(decimal totalPrice)
        {
         int point;
         if(totalPrice >= 200 && totalPrice <=300)
         {
             point = 150;
         }
         else if(totalPrice  >= 301 && totalPrice <= 400)
         {
             point = 200;
         }
         else if(totalPrice >= 401 && totalPrice <= 500)
         {
             point = 250;
         }
         else if (totalPrice >= 501)
         {
             point = 300;
         }
         return point;
        }*/
       
        public async Task<UserResponseModel> LoginStaft(LoginStaftRequestModel request)
        {
            var response = new UserResponseModel();
            
            using (var dbConnection = CreateSQLConnection())
            {
                
                    if(request.roleName == "พนักงาน")
                {

                        try
                        {
                            var sql = @"
                            SELECT 
                            staftID,firstName,lastName,phone,userID,createDate,updateDate,userID,email,password,roleName,accountStatus
                            FROM  staft_tb WHERE email = @email AND password = @password AND roleName = @roleName
                            ";
                        var parameterShowStaft = new
                        {
                            email = request.email,
                            password = request.password,
                            roleName = request.roleName,
                        };
                        var staftData = await dbConnection.QueryFirstOrDefaultAsync<Staft_Authentication_tb>(sql, parameterShowStaft);
                        if (staftData!=null) { 
                           var sql2 = @"UPDATE staft_tb
                            SET accountStatus = @accountStatus
                            WHERE email = @email AND password = @password AND roleName = @roleName
                          ";
                             var parameterStaft = new
                            {
                                email = request.email,
                                password = request.password,
                                roleName = request.roleName,
                                accountStatus = "อยู่ในระบบ"
                             };
                            
                            var memberValue = await dbConnection.ExecuteAsync(sql2, parameterStaft);
                            if(memberValue > 0)
                            {
                                response.success = true;
                                response.message = "แก้ไขสำเร็จ";

                                response.account = new Staft_Authentication_tb()
                                {
                                    staftID = staftData.staftID,
                                    firstName = staftData.firstName,
                                    lastName = staftData.lastName,
                                    phone = staftData.phone,
                                    createDate = staftData.createDate,
                                    updateDate = staftData.updateDate,
                                    userID = staftData.userID,
                                    email = staftData.email,
                                    password = staftData.password,
                                    roleName = staftData.roleName,
                                    accountStatus = "อยู่ในระบบ",
                                };
                            }
                            else
                            {
                                response.success = false;
                                response.message = "แก้ไขไม่สำเร็จ";
                            }
                        }
                        else
                        {
                            response = new UserResponseModel()
                            {
                              message = "ไม่พบบัญชีนี้ในระบบ",
                              success = false
                            };
                        }                           

                        }
                        catch (Exception ex)
                        {
                            // Handle case where no reservations were found
                            response.message = $"{ex}";
                            response.success = false;

                        }

                }
                else
                {
                    response.message = "คุณไม่ใช่พนักงาน";
                    response.success = false;
                }
                    
                
            }

                return response;
        }
        public async Task<UserResponseModel> GetMember([FromBody] string roleName)
        {

            var response = new UserResponseModel();

            if (roleName == "ลูกค้า")
            {
                try
                {
                    using (var dbConnection = CreateSQLConnection()) // Establish database connection
                    {
                        var sql = @"
                          SELECT 
                          c.memberID,c.firstName,c.lastName,c.phone,c.userID,c.email,c.password,c.roleName,p.pointID,p.currentPoint,p.description,p.createDate,c.userID
                          FROM  customer_tb c
                          LEFT JOIN
                          point_tb p ON p.userID = c.userID
                          ";
                        var memberValue = await dbConnection.QueryAsync<Customer_Authentication_tb>(sql);

                        // Check if any reservations were found
                        if (memberValue != null && memberValue.Any())
                        {
                            // Populate the booking response with the reservations


                            response.memberList = memberValue.ToList();
                            response.message = "successfully.";
                            response.success = true;

                        }
                        else
                        {


                            // Handle case where no reservations were found
                            response.message = "Not found data 404.";
                            response.success = false;

                        }
                    }
                }
                catch (Exception ex)
                {



                    // Handle case where no reservations were found
                    response.message = $"{ex}";
                    response.success = false;

                }

            }
            else if (roleName == "พนักงาน")
            {

                try
                {
                    using (var dbConnection = CreateSQLConnection()) // Establish database connection
                    {
                        var sql = @"
                          SELECT 
                          staftID,firstName,lastName,phone,userID,createDate,updateDate,userID,email,password,roleName,accountStatus
                          FROM  staft_tb
                          ";
                        var memberValue = await dbConnection.QueryAsync<Staft_Authentication_tb>(sql);

                        // Check if any reservations were found
                        if (memberValue != null && memberValue.Any())
                        {
                         

                            response.staftList = memberValue.ToList();
                            response.message = "successfully.";
                            response.success = true;

                        }
                        else
                        {

                            // Handle case where no reservations were found
                            response.message = "Not found data 404.";
                            response.success = false;

                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle case where no reservations were found
                    response.message = $"{ex}";
                    response.success = false;

                }

            }
           return response;
        }


    }
}
