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
using Microsoft.Extensions.Options;
using Azure.Core;
using japanese_resturant_project.model.response.customerResponse;
using japanese_resturant_project.model.request.customerRequest;
using Azure;

namespace japanese_resturant_project.services.implement
{
    public class AdminService : Bases, IAdmin
    {

        //get staftProfile

        public async Task<AdminResponse> GetStaftProfile(string staftID)
        {
            var response = new AdminResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT *
                 FROM  staft_tb 
                WHERE staftID = @staftID
                 ";
                    var staftValue = await dbConnection.QueryFirstOrDefaultAsync<Staft_Authentication_tb>(sql, new {staftID=staftID});

                    // Check if any reservations were found
                    if (staftValue != null)
                    {
                        // Populate the booking response with the reservations
                        response.staftItem = staftValue;
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
            catch (Exception ex) { 
               
            }
            return response;
        }
        //editStaftData
        public async Task<AdminResponse> UpdateStaftProfile(UpdateStaftProfileRequest request)
        {
            var response = new AdminResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                UPDATE staft_tb
                SET firstName =@firstName,lastName = @lastName,phone=@phone,updateDate=@updateDate,email=@email,jobType=@jobType
                 FROM  staft_tb 
                WHERE staftID = @staftID
                 ";
                    var parameter = new
                    {
                        staftID = request.staftID,
                        firstName = request.firstName,
                        lastName = request.lastName,
                        phone = request.phone,
                        email = request.email,
                        jobType = request.jobType,
                        updateDate = DateTime.Now,
                    };
                    var staftValue = await dbConnection.ExecuteAsync(sql,parameter);


                    if (staftValue != null)
                    {
                        response.staftItem = new Staft_Authentication_tb()
                        {
                            staftID = request.staftID,
                            firstName = request.firstName,
                            lastName = request.lastName,
                            phone = request.phone,
                            email = request.email,
                            jobType = request.jobType,
                            updateDate = DateTime.Now,
                        };
                        response.message = "แก้ไขเรียบร้อย";
                        response.success = true;

                    }
                    else
                    {

                        response.message = "ไม่สามารถแก้ไขข้อมูลได้";
                        response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.success = false;
            }
            return response;
        }

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
            Random random = new Random();
            int randomID = random.Next(0, 99999);
            string genOptionID = "OP" + randomID.ToString();

            var response = new AdminResponse();
            var sql = @"INSERT INTO option_tb (optionID,optionName,value)
                        VALUES (@optionID,@optionName,@value)";
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    response.optionitem = new Option_tb_
                    {
                        optionID = genOptionID,
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


                    var parameters = new
                    {
                        optionID = optionID,
                    };

   
                    int rowsAffected = await dbConnection.ExecuteAsync(sql, parameters);

           
                    if (rowsAffected > 0)
                    {
                        response.message = "Delete successful.";
                        response.success = true;

                    }
                    else
                    {
          
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
        public async Task<AdminResponse> GetMenuList2(SearchRequest request)
        {
            var Response = new AdminResponse()
            {
                menuList = new List<Menu_tb>()
            };
            //function ในการเปลี่ยนจาก imagePath => imageFile
            
            try
            {

                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {

                    //ภาพขึ้นล่ะ ถ้าใช้เป็นformat https://localhost:7202/Image/16aba539-bbd5-472d-bd14-31ab4227c4ec_food.jpg อันนี้เป็นตัวอย่าง
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
                 m.stockQuantity,
                 o.optionName,
                 o.value
                 FROM  menu_tb m
                 LEFT JOIN
                   option_tb o ON o.optionID = m.optionID
                 WHERE (@menuName = '' OR m.menuName LIKE '%' + @menuName + '%'OR m.categoryName LIKE '%' + @menuName + '%')
                 ORDER BY m.rating DESC
                 ;
                 ";

                    var sql2 = @"
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
                 m.stockQuantity,
                 o.optionName,
                 o.value
                 FROM  menu_tb m
                 LEFT JOIN
                   option_tb o ON o.optionID = m.optionID
                 ORDER BY m.rating DESC;
                 ";

                    var parameter = new
                    {
                        menuName = string.IsNullOrEmpty(request.menuName)? string.Empty : request.menuName,
                    };
                    var menuValue = await dbConnection.QueryAsync(sql,parameter);

                    // Check if any reservations were found
                    if (menuValue != null&& menuValue.Any())
                    {
                        // Populate the booking response with the reservations
                        //ดึง imageName จาก database
                        //ใส่ function  แปลงในตรงนี้ แล้วก็นำไปใส่ในตัว Menu_tb
                        //image path from database => get file => get to front end
                        //Response.menuList = menuValue.ToList();

                        Response.menuList = menuValue.Select(x => new Menu_tb()
                        {
                            menuID = x.menuID,
                            menuName = x.menuName,
                            menuDescription = x.menuDescription,
                            unitPrice = x.unitPrice,
                            categoryName = x.categoryName,
                            optionID = x.optionID,
                            createDate = x.createDate,
                            updateDate = x.createDate,
                            rating = x.rating,
                            imageName = x.imageName,
                            optionName = x.optionName,
                            value = x.value,
                            imageSrc = String.Format("https://localhost:7202/Image/{0}", x.imageName),
                            stockQuantity = x.stockQuantity,

                        }).ToList();
                        //ดึงข้อมูลออกมา

                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        var menuValue2 = await dbConnection.QueryAsync(sql2);

     
                        Response.menuList = menuValue2.Select(x => new Menu_tb()
                        {
                            menuID = x.menuID,
                            menuName = x.menuName,
                            menuDescription = x.menuDescription,
                            unitPrice = x.unitPrice,
                            categoryName = x.categoryName,
                            optionID = x.optionID,
                            createDate = x.createDate,
                            updateDate = x.createDate,
                            rating = x.rating,
                            imageName = x.imageName,
                            optionName = x.optionName,
                            value = x.value,
                            imageSrc = String.Format("https://localhost:7202/Image/{0}", x.imageName),
                            stockQuantity = x.stockQuantity,

                        }).ToList();
                        Response.message = "ไม่พบข้อมูลที่ท่านต้องการค้นหา";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
   
                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"An error occurred while fetching the reservations: {ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        public async Task<AdminResponse> GetMenuByID(string menuID)
        {
            var Response = new AdminResponse()
            {
                menuitem = new Menu_tb()
            };
            //function ในการเปลี่ยนจาก imagePath => imageFile

            try
            {
                using (var dbConnection = CreateSQLConnection())
                {

                    //ภาพขึ้นล่ะ ถ้าใช้เป็นformat https://localhost:7202/Image/16aba539-bbd5-472d-bd14-31ab4227c4ec_food.jpg อันนี้เป็นตัวอย่าง
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
                 m.stockQuantity,
                 o.optionName,
                 o.value
                 FROM  menu_tb m
                 LEFT JOIN
                   option_tb o ON o.optionID = m.optionID
                 WHERE m.menuID = @menuID
                 ";
                    var menuValue = await dbConnection.QueryFirstOrDefaultAsync(sql, new { menuID = menuID });
                    if (menuValue != null)
                    {

                        Response.menuitem = new Menu_tb()
                        {
                            menuID = menuValue.menuID,
                            menuName = menuValue.menuName,
                            menuDescription = menuValue.menuDescription,
                            unitPrice = menuValue.unitPrice,
                            categoryName = menuValue.categoryName,
                            optionID = menuValue.optionID,
                            createDate = menuValue.createDate,
                            updateDate = menuValue.createDate,
                            rating = menuValue.rating,
                            imageName = menuValue.imageName,
                            optionName = menuValue.optionName,
                            value = menuValue.value,
                            imageSrc = String.Format("https://localhost:7202/Image/{0}", menuValue.imageName),
                            stockQuantity = menuValue.stockQuantity,

                        };
                        //ดึงข้อมูลออกมา

                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {

                        Response.message = "ไม่พบเจอข้อมูลอาหาร";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }

        public async Task<AdminResponse> GetBestMenu(SearchMenuRequest request)
        {
            var Response = new AdminResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {

                    //ภาพขึ้นล่ะ ถ้าใช้เป็นformat https://localhost:7202/Image/16aba539-bbd5-472d-bd14-31ab4227c4ec_food.jpg อันนี้เป็นตัวอย่าง
                    var sql = $@"
                SELECT TOP {request.number} m.menuID,m.menuName, m.rating,m.imageName
                 FROM  menu_tb m  
                 ORDER BY  m.rating DESC
                 ";
                    var menuValue = await dbConnection.QueryAsync(sql, new { number= request.number});
                    //string.IsNullOrEmpty(request.menuName) ? string.Empty : request.menuName
                    if (menuValue != null)
                    {

                        Response.menuList = menuValue.Select(x => new Menu_tb()
                        {
                            menuID = x.menuID,
                            menuName = x.menuName,
                            rating = x.rating,
                            imageName = x.imageName, 
                            imageSrc = String.Format("https://localhost:7202/Image/{0}", x.imageName),

                        }).ToList();
                        //ดึงข้อมูลออกมา

                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        
                        Response.message = "ไม่พบเจอข้อมูลอาหาร";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //menuAdd

        public async Task<AdminResponse> AddMenu([FromForm] MenuRequest request)
        {
            var response = new AdminResponse();

            Random random = new Random();
            int randomID = random.Next(0, 99999);
            string genMenuID = "M" + randomID.ToString();

            string? relativeFilePath = null;
            if (request.imageFile != null)
            {
                var directoryPath = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var fileName = $"{Guid.NewGuid()}_{request.imageFile.FileName}";
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.imageFile.CopyToAsync(stream);
                }
                relativeFilePath = Path.Combine(fileName);

            }
            //ฐานข้อมูลเข้าแล้ว
            var sql = @"INSERT INTO menu_tb (menuID,menuName,menuDescription,unitPrice,categoryName,optionID,createDate,updateDate,rating,imageName,stockQuantity)
                        VALUES (@menuID,@menuName,@menuDescription,@unitPrice,@categoryName,@optionID,@createDate,@updateDate,@rating,@imageName,@stockQuantity)";
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    var parameters = new
                    {
                        menuID = genMenuID,
                        menuName = request.menuName,
                        menuDescription = request.menuDescription,
                        unitPrice = request.unitPrice,
                        categoryName = request.categoryName,
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        optionID = request.optionID,
                        imageName = relativeFilePath, //ชื่อไฟล์
                        rating =  0,//ต้องอิงตามค่าที่ได้ทำการกดให้คะแนนตามจำนวนลูกค้า
                        stockQuantity = request.stockQuantity,

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
        public async Task<AdminResponse> UpdateMenu( MenuUpdate request)
        {
            var response = new AdminResponse();
           
            try
            {
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var sql = @"UPDATE menu_tb
                        SET menuName=@menuName,
                            menuDescription=@menuDescription,
                            unitPrice=@unitPrice,
                            categoryName=@categoryName,
                            optionID=@optionID,
                            updateDate=@updateDate,
                            rating = @rating,
                            stockQuantity= stockQuantity + @stockQuantity
                        WHERE menuID = @menuID";

    
                    var parameters = new
                    {
                        menuID = request.menuID,
                        menuName = request.menuName,
                        menuDescription = request.menuDescription,
                        unitPrice = request.unitPrice,
                        categoryName = request.categoryName,
                        updateDate = DateTime.Now,
                        optionID = request.optionID,
                        rating = 0,//ต้องอิงตามค่าที่ได้ทำการกดให้คะแนนตามจำนวนลูกค้า
                        stockQuantity = request.stockQuantity,
                    };

                    int menuValue = await dbConnection.ExecuteAsync(sql, parameters);

                    if (menuValue > 0)
                    {
                        response.message = "success";
                        response.success = true;
                    }
                    else
                    {

                        response.message = "Update failed: opject not found.";

                        response.success = false;
                    }
                }
            }
            catch (Exception ex)
            {

                response.message = $"Update failed: {ex.Message}";
                response.success = false;
            }
            return response;
        }
        //menuDelete
        public async Task<AdminResponse> DeleteMenu(string menuID)
        {
            var response = new AdminResponse();
            var menuSql = @"SELECT imageName FROM menu_tb WHERE menuID = @menuID";
            var sql = @"DELETE FROM menu_tb WHERE menuID = @menuID";
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {


                    var parameters = new
                    {
                        menuID = menuID,
                    };

                    var imageValue = dbConnection.QueryFirstOrDefault<string>(menuSql, parameters);
                    if (imageValue != null) { 

                     var deleteMenu = await dbConnection.ExecuteAsync(sql, parameters);
                    if (deleteMenu > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageValue);
                            if (System.IO.File.Exists(filePath)) {
                                System.IO.File.Delete(filePath);
                            }
                            Console.WriteLine("delet imageFIle success");
                        
                        response.message = "Delete successful.";
                        response.success = true;

                    }
                    else
                    {

                        response.message = "Delete failed: Reservation not found.";
                        response.success = false;

                    }
                      
                    }else Console.WriteLine("ImageName don't found");


                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.message = $"Delete failed: {ex.Message}";
            }

            return response;
        }


        //getAllTable แก้ไข
        public async Task<AdminResponse> GetTableList()
        {
            var Response = new AdminResponse()
            {
                tableList = new List<Table_tb>()
            };

            try
            {
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var sql = @"
                SELECT *
                 FROM  table_tb
                 ";
                    var tablenum = await dbConnection.QueryAsync<Table_tb>(sql);


                    if (tablenum != null)
                    {

                        Response.tableList = tablenum.ToList();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {

                        Response.message = "Not found data 404.";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //orderConfirm แก้ไข
        public Task<AdminResponse> GetOrderForAdmin(SearchOrderRequest request)
        {
            

            var Response = new AdminResponse()
            {
                orders = new List<Order_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var sql = @"
                SELECT 
                 o.orderID,
                 o.tableID,
                 o.staftID,
                 o.orderStatus,
                 o.orderDate,
                 o.totalPrice,
                 o.confirmOrder,
                 o.paymentStatus,
                 o.customerID,
                 od.menuID,
                 od.orderDetailStatus,
                 od.quantity,
                 od.optionValue,
                 od.netprice,
                 m.menuName,
                 m.unitPrice,
                 m.imageName
                 FROM  order_tb o
                 LEFT JOIN 
                   orderDetail_tb od ON od.orderID = o.orderID 
                 LEFT JOIN 
                   menu_tb m ON m.menuID = od.menuID 
                 WHERE (@orderID IS NULL OR @orderID = '' OR o.orderID LIKE '%' + @orderID + '%'
                       OR o.orderStatus LIKE '%' + @orderID + '%')
                 ORDER BY o.orderDate DESC;
                 ";

                    var searchParameter = new
                    {
                        orderID = string.IsNullOrEmpty(request.orderID) ? string.Empty : request.orderID,
                    };

                  


                    var Value = dbConnection.Query<Order_tb, OrderDetail_tb, Order_tb>(sql,(order, orderDatail) =>
                    {
                        order.OrderDetailList = order.OrderDetailList ?? new List<OrderDetail_tb>();
                        if (orderDatail != null)
                        {
                            orderDatail.imageSrc = String.Format("https://localhost:7202/Image/{0}", orderDatail.imageName);
                            order.OrderDetailList.Add(orderDatail);
                        }
                        return order;
                    }, searchParameter, splitOn: "menuID").GroupBy(o => o.orderID).Select(g =>
                    {
                        var groupOrderList = g.First();
                        groupOrderList.OrderDetailList = g.SelectMany(o => o.OrderDetailList).ToList();
                        return groupOrderList;
                    }).ToList();

                  


                    if (Value != null && Value.Any())
                    {

                        Response.orders = Value.ToList();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                       
                        Response.message = "ไม่พบรายการสั่งของโต๊ะนี้";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Task.FromResult(Response);
        }

        //แก้ไข
        public Task<AdminResponse> GetOrderByID(string orderID)
        {
            var Response = new AdminResponse();
            
            try
            {
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var sql = @"
                SELECT 
                 o.orderID,
                 o.tableID,
                 o.staftID,
                 o.orderStatus,
                 o.orderDate,
                 o.totalPrice,
                 o.confirmOrder,
                 o.paymentStatus,
                 o.customerID,
                 od.menuID,
                 od.orderDetailStatus,
                 od.quantity,
                 od.optionValue,
                 od.netprice,
                 m.menuName,
                 m.unitPrice,
                 m.imageName
                 FROM  order_tb o
                 LEFT JOIN 
                   orderDetail_tb od ON od.orderID = o.orderID 
                 LEFT JOIN 
                   menu_tb m ON m.menuID = od.menuID 
                 WHERE o.orderID = @orderID
                 ";
                    var parameter = new
                    {
                        orderID = orderID
                    };
                    var Value = dbConnection.Query<Order_tb, OrderDetail_tb, Order_tb>(sql, (order, orderDatail) =>
                    {
                        order.OrderDetailList = order.OrderDetailList ?? new List<OrderDetail_tb>();
                        if (orderDatail != null)
                        {
                            orderDatail.imageSrc = String.Format("https://localhost:7202/Image/{0}", orderDatail.imageName);
                            order.OrderDetailList.Add(orderDatail);
                        }
                        return order;
                    }, parameter,splitOn: "menuID").GroupBy(o => o.orderID).Select(g =>
                    {
                        var groupOrderList = g.First();
                        groupOrderList.OrderDetailList = g.SelectMany(o => o.OrderDetailList).ToList();
                        return groupOrderList;
                    }).ToList();



                    if (Value != null && Value.Any())
                    {

                        Response.orderItem = Value.FirstOrDefault();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {

                        Response.message = "ไม่พบรายการสั่งของโต๊ะนี้";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Task.FromResult(Response);
        }
        //chef confirm cooking
        public async Task<AdminResponse> ConfirmOrder(ConfirmRequest request)
        {
            var response = new AdminResponse();
            try
            {
                var orderSQL = @"UPDATE order_tb
                        SET confirmOrder = @confirmOrder,
                         staftID = @staftID
                        WHERE orderID = @orderID";
               
                var upadateorderDetailSQL = @"UPDATE orderDetail_tb
                        SET orderDetailStatus = @orderDetailStatus
                        WHERE orderID = @orderID";
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var parameters = new
                    {
                        orderID = request.orderID,
                        confirmOrder = request.confirm,
                        staftID = request.staftID,
                    };
                    var orderValue = await dbConnection.ExecuteAsync(orderSQL, parameters);

                    if (orderValue > 0)
                    {
                        response.orderItem = new Order_tb()
                        {
                            orderID = request.orderID,
                            confirmOrder = request.confirm,
                            staftID = request.staftID,
                        };
                        if(request.confirm == "อนุญาติเรียบร้อย")
                        {
                                var orderDetailupdate = new 
                                {
                                    orderID = request.orderID,
                                    orderDetailStatus = "กำลังปรุง"
                                };
                                var orderDetailData = await dbConnection.ExecuteAsync(upadateorderDetailSQL, orderDetailupdate); 
                                
                                if (orderDetailData > 0)
                                {
                                    response.message = "แก้ไขข้อมูลลงในรายการการสั่งซื้อสำเร็จ";
                                }
                                else
                                {
                                    response.message = "แก้ไขข้อมูลลงในรายการการสั่งซื้อไม่สำเร็จ";
                                }
                         }else if (request.confirm == "ยกเลิกรายการสั่งนี้")
                        {
                            var orderDetailupdate = new
                            {
                                orderID = request.orderID,
                                orderDetailStatus = "เมนูนี้ถูกยกเลิกโดยพนักงานเนื่องจาก มีบางอย่างผิดปกติ"
                            };
                            var orderDetailData = await dbConnection.ExecuteAsync(upadateorderDetailSQL, orderDetailupdate);

                            if (orderDetailData > 0)
                            {
                                response.message = "แก้ไขข้อมูลลงในรายการการสั่งซื้อสำเร็จ";
                            }
                            else
                            {
                                response.message = "แก้ไขข้อมูลลงในรายการการสั่งซื้อไม่สำเร็จ";
                            }
                        }
                     }
                      

                    
                    else
                    {
                        response.message = "เพิ่มข้อมูลการสั่งอาหารไม่สำเร็จ";
                    }
                }
            }
            catch (Exception ex)
            {

                response.message = $"{ex}";

            }
            return response;
        }
        //แก้ไข
        public async Task<AdminResponse> GetOrderDetail(SearchOrderRequest request)
        {
          

            var Response = new AdminResponse()
            {
                orderList = new List<OrderDetail_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT 
                 od.orderID,
                 od.menuID,
                 od.orderDetailStatus,
                 od.quantity,
                 od.optionValue,
                 od.netprice,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 m.stockQuantity,
                 m.categoryName,
                 o.orderDate,
                 o.tableID,
                 o.customerID,
                 o.Q
                 FROM  orderDetail_tb od
                 LEFT JOIN 
                   menu_tb m ON m.menuID = od.menuID 
                 LEFT JOIN order_tb o ON od.orderID = o.orderID
                 WHERE (@orderID IS NULL OR @orderID = '' OR o.orderID LIKE '%' + @orderID + '%')
                 ORDER BY  od.orderDetailStatus
                 ";
                    var searchParameter = new
                    {
                        orderID = string.IsNullOrEmpty(request.orderID) ? string.Empty : request.orderID,
                    };

                    var Value = await dbConnection.QueryAsync<OrderDetail_tb>(sql,searchParameter);
                    if (Value != null && Value.Any())
                    {

                        Response.orderList = Value.ToList();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        Response.message = "ไม่พบรายการสั่งของโต๊ะนี้";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //ทำการเปลี่ยนแปลงสถานะของการประกอบอาหาร
        public async Task<AdminResponse> updateOrderStatus(UpdateOrderStatusRequest request)
        {
            var Response = new AdminResponse();

            var updateOrderStatus = @"UPDATE order_tb SET orderStatus = @orderStatus WHERE orderID=@orderID";

            var OrderDetailsql = @"
                SELECT 
                 COUNT (*)
                FROM orderDetail_tb
                WHERE orderID = @orderID AND orderDetailStatus != 'เสริฟแล้ว'
                 ";

            try
            {   
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var sql = @"
                UPDATE orderDetail_tb
                SET orderDetailStatus = @orderDetailStatus
                WHERE orderID = @orderID AND menuID = @menuID 
                 ";

                    
                    var cartValue = await dbConnection.ExecuteAsync(sql, new { orderID = request.orderID, orderDetailStatus = request.orderDetailStatus,menuID = request.menuID });

                    if (cartValue > 0)
                    {


                        Response.orderOne = new OrderDetail_tb()
                        {
                            orderID = request.orderID,
                            orderDetailStatus = request.orderDetailStatus,
                            menuID = request.menuID
                        };

                        var orderStatusValue = await dbConnection.ExecuteScalarAsync<int>(OrderDetailsql,new {orderID = request.orderID});
                        if(orderStatusValue == 0)
                        {
                            await dbConnection.ExecuteAsync(updateOrderStatus, new { orderStatus = "ดำเนินรายการสำเร็จ", orderID = request.orderID});
                        }
                        Response.message = "เปลี่ยนสถานะสำเร็จ.";
                        Response.success = true;

                    }
                    else
                    {
                        Response.message = "ไม่พบรายการสั่ง";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //Show notification
        public async Task<AdminResponse> GetNotification()
        {
            var Response = new AdminResponse()
            {
                notiList = new List<Notification_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    var sql = @"
                SELECT 
                   notificationID,title,message,createDate,tableID,isRead,sentBy
                 FROM  notification_tb
                 ORDER BY  createDate DESC
                 ";

                    var Value = await dbConnection.QueryAsync<Notification_tb>(sql);
                    if (Value != null && Value.Any())
                    {

                        Response.notiList = Value.ToList();
                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        Response.message = "ไม่พบรายการการแจ้งเตือน";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        public async Task<AdminResponse> Readable(string notificationID)
        {
            var response = new AdminResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) 
                {
                    var sql = @"UPDATE notification_tb
                        SET isRead = @isRead
                        WHERE notificationID = @notificationID";


                    var parameters = new
                    {
                        notificationID = notificationID,
                        isRead = "อ่านแล้ว"
                    };


                    int optionValue = await dbConnection.ExecuteAsync(sql, parameters);


                    if (optionValue > 0)
                    {

                        response.notiItem = new Notification_tb
                        {
                            notificationID = notificationID,
                            isRead = "อ่านแล้ว"
                        };

                        var sqlDelete = @"DELETE FROM notification_tb WHERE notificationID = @notificationID";

                        int Value = await dbConnection.ExecuteAsync(sqlDelete, new { notificationID = notificationID });

                        if (Value > 0)
                        {
                            response.message = "Delete successful.";
                            response.success = true;

                        }
                        else
                        {
                            response.message = "Delete failed: Data not found.";
                            response.success = false;

                        }
                    }
                    else
                    {
                        response.message = "Update failed: opject not found.";

                        response.success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = $"Update failed: {ex.Message}";
                response.success = false;
            }
            return response;
        }
        

        public async Task<CustomerResponse> GetOrderDetailStatus()
        {
            var Response = new CustomerResponse()
            {
                orderList = new List<OrderDetail_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT 
                 od.orderID,
                 od.menuID,
                 od.orderDetailStatus,
                 od.quantity,
                 od.optionValue,
                 od.netprice,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 m.stockQuantity
                 FROM  orderDetail_tb od
                 LEFT JOIN 
                   menu_tb m ON m.menuID = od.menuID 
             
                 ";
                    var cartValue = await dbConnection.QueryAsync(sql);

                    if (cartValue != null && cartValue.Any())
                    {

                        Response.orderList = cartValue.Select(x => new OrderDetail_tb()
                        {
                            orderID = x.orderID,
                            menuID = x.menuID,
                            orderDetailStatus = x.orderDetailStatus,
                            quantity = x.quantity,
                            optionValue = x.optionValue,
                            netprice = x.netprice,
                            menuName = x.menuName,
                            unitPrice = x.unitPrice,
                            imageName = x.imageName,
                            imageSrc = String.Format("https://localhost:7202/Image/{0}", x.imageName),
                            stockQuantity = x.stockQuantity,
                        }).ToList();

                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        Response.message = "ไม่พบรายการสั่ง";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }

        public async Task<AdminResponse> GetRevenue()
        {
            var Response = new AdminResponse()
            {
               revenueList = new List<Revenue_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT 
                 revenueID,
                 createDate,
                 netAmount
                 FROM revenue_tb 
             
                 ";
                    var cartValue = await dbConnection.QueryAsync<Revenue_tb>(sql);

                    if (cartValue != null && cartValue.Any())
                    {

                        Response.revenueList = cartValue.ToList();

                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        Response.message = "ไม่พบรายการสั่ง";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");

                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
    }
}
