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

namespace japanese_resturant_project.services.implement
{
    public class AdminService : Bases, IAdmin
    {


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
                 WHERE (@menuName = '' OR m.menuName LIKE '%' + @menuName + '%'OR m.categoryName LIKE '%' + @menuName + '%');
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
                   option_tb o ON o.optionID = m.optionID;
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

                        // Handle case where no reservations were found
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
                // Handle exceptions here, e.g., log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Set error message in the bookingResponse
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
                        // Handle case where no reservations were found
                        Response.message = "ไม่พบเจอข้อมูลอาหาร";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error
                // Set error message in the bookingResponse
                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //menuAdd

        public async Task<AdminResponse> AddMenu([FromForm] MenuRequest request)
        {
            var response = new AdminResponse();
            //var menuID = Guid.NewGuid();

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
                        SET menuName=@menuName,
                            menuDescription=@menuDescription,
                            unitPrice=@unitPrice,
                            categoryName=@categoryName,
                            optionID=@optionID,
                            updateDate=@updateDate,
                            rating = @rating,
                            imageName = @imageName,
                            stockQuantity= stockQuantity + @stockQuantity
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
                        rating = 0,//ต้องอิงตามค่าที่ได้ทำการกดให้คะแนนตามจำนวนลูกค้า
                        stockQuantity = request.stockQuantity,
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
        public async Task<AdminResponse> DeleteMenu(string menuID)
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


        //getAllTable
        public async Task<AdminResponse> GetTableList()
        {
            var Response = new AdminResponse()
            {
                tableList = new List<Table_tb>()
            };

            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT *
                 FROM  table_tb
                 ";
                    var tablenum = await dbConnection.QueryAsync<Table_tb>(sql);

                    // Check if any reservations were found
                    if (tablenum != null)
                    {
                        // Populate the booking response with the reservations
                        Response.tableList = tablenum.ToList();
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
                Response.message = $"{ex.Message}";
                Response.success = false;
            }

            return Response;
        }
        //orderConfirm
        public Task<AdminResponse> GetOrderForAdmin()
        {
            var Response = new AdminResponse()
            {
                orders = new List<Order_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
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
                 ";
                   
                    var Value = dbConnection.Query<Order_tb, OrderDetail_tb, Order_tb>(sql, (order, orderDatail) =>
                    {
                        order.OrderDetailList = order.OrderDetailList ?? new List<OrderDetail_tb>();
                        if (orderDatail != null)
                        {
                            orderDatail.imageSrc = String.Format("https://localhost:7202/Image/{0}", orderDatail.imageName);
                            order.OrderDetailList.Add(orderDatail);
                        }
                        return order;
                    },splitOn: "menuID").GroupBy(o => o.orderID).Select(g =>
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

        public Task<AdminResponse> GetOrderByID(string orderID)
        {
            var Response = new AdminResponse();
            
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
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
        //QAF answer
    }
}
