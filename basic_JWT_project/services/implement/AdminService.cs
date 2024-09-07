using japanese_resturant_project.services.Interfaces;
using japanese_resturant_project.model.request;
using japanese_resturant_project.model.response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.response.adminResponse;

namespace japanese_resturant_project.services.implement
{
    public class AdminService : Bases, IAdmin
    {

        //getTable


        //optionGet
        public async Task<AdminResponse> GetOptionList()
        {
            var Response = new AdminResponse()
            {
                optionList = new List<Option_tb_>()
            };

            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT *
                 FROM  option_tb
                 ";
                    var optionnum = await dbConnection.QueryAsync<Option_tb_>(sql);

                    // Check if any reservations were found
                    if (optionnum != null)
                    {
                        // Populate the booking response with the reservations
                        Response.optionList = optionnum.ToList();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        // Handle case where no reservations were found
                        Response.message = "Not found data 404.";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Set error message in the bookingResponse
                Response.message = $"An error occurred while fetching the reservations: {ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //optionAdd
        public async Task<AdminResponse> AddOption(OptionRequest request)
        {
            var response = new AdminResponse();
            var optiionID = Guid.NewGuid();
            var sql = @"INSERT INTO option_tb (optionID,optionName,value)
                        VALUES (@optionID,@optionName,@value)";
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    response.optionitem = new Option_tb_
                    {
                        optionID = optiionID,
                        optionName = request.optionName,
                        value = request.value,
                    };
                    var optionValue = await dbConnection.ExecuteAsync(sql, response.optionitem);
                    if (optionValue > 0)
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
            catch (Exception ex)
            {
                {
                    response.message = ex.Message;
                }
            }
                return response;
        }


            //optionUpdate
            public async Task<AdminResponse> UpdateOption(Option_tb_ request)
            {
                var response = new AdminResponse();
                try
                {
                    using (var dbConnection = CreateSQLConnection()) // Establish database connection
                    {
                        var sql = @"UPDATE option_tb
                        SET optionName = @optionName,
                            value = @value
                        WHERE optionID = @optionID";

                        // Use parameterized query to prevent SQL injection
                        var parameters = new
                        {
                           optionID = request.optionID,
                           optionName = request.optionName,
                           value = request.value,
                        };

                        // Execute the query
                        int optionValue = await dbConnection.ExecuteAsync(sql, parameters);

                        // Check if the update was successful
                        if (optionValue > 0)
                        {
                            // Populate the booking response with the updated reservation
                            response.optionitem = new Option_tb_

                          {
                            
                           optionID = request.optionID,
                           optionName = request.optionName,
                           value = request.value,
                             
                          };
                            response.message = "success";
                            response.success = true;
                        }
                        else
                        {
                            // Handle the case where no rows were affected (e.g., reservation not found)
                            response.message = "Update failed: opject not found.";

                            response.success = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    response.message = $"Update failed: {ex.Message}";
                    response.success = false;
                }
                return response;
            }

        //optionDelete
        public async Task<AdminResponse> DeleteOption([FromBody] Guid optionID)
        {
            var response = new AdminResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"DELETE FROM option_tb WHERE optionID = @optionID";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        optionID = optionID,
                    };

                    // Execute the query
                    int rowsAffected = await dbConnection.ExecuteAsync(sql, parameters);

                    // Check if the delete was successful
                    if (rowsAffected > 0)
                    {
                        response.message = "Delete successful.";
                        response.success = true;

                    }
                    else
                    {
                        // Handle the case where no rows were affected (e.g., reservation not found)
                        response.message = "Delete failed: Reservation not found.";
                        response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.message = $"Delete failed: {ex.Message}";
            }

            return response;
        }

        //getMenu
        public async Task<AdminResponse> GetMenuList()
        {
            var Response = new AdminResponse()
            {
                menuList = new List<Menu_tb>()
            };

            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT 
                 m.menuID,
                 m.menuName,
                 m.menuDescription,
                 m.unitPrice,
                 m.categoryName,
                 m.optionID,
                 m.createDate,
                 m.updateDate,
                 m.rating,
                 m.imageName,
                 o.optionName,
                 o.value
                 FROM  menu_tb m
                 LEFT JOIN
                   option_tb o ON o.optionID = m.optionID
                 ";
                    var menuValue = await dbConnection.QueryAsync<Menu_tb>(sql);

                    // Check if any reservations were found
                    if (menuValue != null&& menuValue.Any())
                    {
                        // Populate the booking response with the reservations
           

                        Response.menuList = menuValue.ToList();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        // Handle case where no reservations were found
                        Response.message = "Not found data 404.";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Set error message in the bookingResponse
                Response.message = $"An error occurred while fetching the reservations: {ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //menuAdd

        public async Task<AdminResponse> AddMenu([FromForm] MenuRequest request)
        {
            var response = new AdminResponse();
            var menuID = Guid.NewGuid();
            string? relativeFilePath = null;
            if (request.imageFile != null)
            {
                var directoryPath = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                // Rename the file to avoid conflicts
                var fileName = $"{Guid.NewGuid()}_{request.imageFile.FileName}";
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.imageFile.CopyToAsync(stream);
                }
                relativeFilePath = Path.Combine(fileName);

            }
            //ฐานข้อมูลเข้าแล้ว
            var sql = @"INSERT INTO menu_tb (menuID,menuName,menuDescription,unitPrice,categoryName,optionID,createDate,updateDate,rating,imageName)
                        VALUES (@menuID,@menuName,@menuDescription,@unitPrice,@categoryName,@optionID,@createDate,@updateDate,@rating,@imageName)";
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    var parameters = new
                    {
                        menuID = menuID,
                        menuName = request.menuName,
                        menuDescription = request.menuDescription,
                        unitPrice = request.unitPrice,
                        categoryName = request.categoryName,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        optionID = request.optionID,
                        imageName = relativeFilePath, //ชื่อไฟล์
                        rating =  0//ต้องอิงตามค่าที่ได้ทำการกดให้คะแนนตามจำนวนลูกค้า
                                  
                    };
                    var menuValue = await dbConnection.ExecuteAsync(sql,parameters);
                    if (menuValue > 0)
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
            catch (Exception ex)
            {
                {
                    response.message = ex.Message;
                }
            }
            return response;
        }
        //memuUpdate
        public async Task<AdminResponse> UpdateMenu([FromForm] MenuUpdate request)
        {
            var response = new AdminResponse();
            string? relativeFilePath = null;
            if (request.imageFile != null)
            {
                var directoryPath = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                // Rename the file to avoid conflicts
                var fileName = $"{Guid.NewGuid()}_{request.imageFile.FileName}";
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.imageFile.CopyToAsync(stream);
                }
                relativeFilePath = Path.Combine(fileName);

            }
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"UPDATE menu_tb
                        SET menuName=@menuName,menuDescription=@menuDescription,unitPrice=@unitPrice,categoryName=@categoryName,optionID=@optionID,updateDate=@updateDate,rating = @rating,imageName = @imageName
                        WHERE menuID = @menuID";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        menuID = request.menuID,
                        menuName = request.menuName,
                        menuDescription = request.menuDescription,
                        unitPrice = request.unitPrice,
                        categoryName = request.categoryName,
                        updateDate = DateTime.Now,
                        optionID = request.optionID,
                        imageName = relativeFilePath, //ชื่อไฟล์
                        rating = 0//ต้องอิงตามค่าที่ได้ทำการกดให้คะแนนตามจำนวนลูกค้า
                    };

                    // Execute the query
                    int menuValue = await dbConnection.ExecuteAsync(sql, parameters);

                    // Check if the update was successful
                    if (menuValue > 0)
                    {
                        response.message = "success";
                        response.success = true;
                    }
                    else
                    {
                        // Handle the case where no rows were affected (e.g., reservation not found)
                        response.message = "Update failed: opject not found.";

                        response.success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.message = $"Update failed: {ex.Message}";
                response.success = false;
            }
            return response;
        }
        //menuDelete
        public async Task<AdminResponse> DeleteMenu([FromBody] Guid menuID)
        {
            var response = new AdminResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"DELETE FROM menu_tb WHERE menuID = @menuID";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        menuID = menuID,
                    };

                    // Execute the query
                    int menuValue = await dbConnection.ExecuteAsync(sql, parameters);

                    // Check if the delete was successful
                    if (menuValue > 0)
                    {
                        response.message = "Delete successful.";
                        response.success = true;

                    }
                    else
                    {
                        // Handle the case where no rows were affected (e.g., reservation not found)
                        response.message = "Delete failed: Reservation not found.";
                        response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.message = $"Delete failed: {ex.Message}";
            }

            return response;
        }
        //orderConfirm
        //chef confirm cooking
        //QAF answer
    }
}
