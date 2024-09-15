using japanese_resturant_project.services;
using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.request.customerRequest;
using japanese_resturant_project.model.response.customerResponse;
using Dapper;
using japanese_resturant_project.model.response.adminResponse;
using Microsoft.AspNetCore.Mvc;
using japanese_resturant_project.model.request;
using Azure.Core;
using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.WebSockets;

namespace japanese_resturant_project.services.implement
{
    public class CustomerService : Bases,ICustomer
    {
        //เปิดโต๊ะ
        public async Task<CustomerResponse> OpenTable(OpenTableRequest request)
        {
            var openResponse = new CustomerResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"UPDATE table_tb
                        SET tableStatus = @tableStatus
                        WHERE tableID = @tableID AND tableStatus = 'ว่าง'";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                       tableID = request.tableID,
                       tableStatus = "จองแล้ว"
                     };

                    // Execute the query
                    int rowsAffected = await dbConnection.ExecuteAsync(sql, parameters);

                    // Check if the update was successful
                    if (rowsAffected > 0)
                    {
                        // Populate the booking response with the updated reservation
                        openResponse.table = new Table_tb
                        {
                            tableID = request.tableID,
                            tableStatus = "มีลูกค้า"
                        };

                        openResponse.success = true;
                    }
                    else
                    {
                        // Handle the case where no rows were affected (e.g., reservation not found)
                        openResponse.message = "Update failed: Reservation not found beacuse this table is booking.";

                        openResponse.success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                openResponse.message = $"Update failed: {ex.Message}";

                openResponse.success = false;
            }

            return openResponse;
        }
        //ปิดโต๊ะ

        public async Task<CustomerResponse> CloseTable(OpenTableRequest request)
        {
            var openResponse = new CustomerResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"UPDATE table_tb
                        SET tableStatus = @tableStatus
                        WHERE tableID = @tableID AND tableStatus = 'จองแล้ว'";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        tableID = request.tableID,
                        tableStatus = "ว่าง"
                    };

                    // Execute the query
                    int rowsAffected = await dbConnection.ExecuteAsync(sql, parameters);

                    // Check if the update was successful
                    if (rowsAffected > 0)
                    {
                        // Populate the booking response with the updated reservation
                        openResponse.table = new Table_tb
                        {
                            tableID = request.tableID,
                            tableStatus = "ว่าง"
                        };

                        openResponse.success = true;
                    }
                    else
                    {
                        // Handle the case where no rows were affected (e.g., reservation not found)
                        openResponse.message = "Update failed: Reservation not found beacuse this table not booking.";

                        openResponse.success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                openResponse.message = $"Update failed: {ex.Message}";

                openResponse.success = false;
            }

            return openResponse;
        }
        
        public async Task<CustomerResponse> GetCartBytableID(string tableID)
        {
            var Response = new CustomerResponse()
            {
                cartList = new List<Cart_tb>()
            };
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT 
                 c.cartID,
                 c.menuID,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 c.tableID,
                 c.quantity,
                 c.optionValue
                 FROM  Cart_tb c
                 LEFT JOIN
                   menu_tb m ON m.menuID = c.menuID 
                WHERE c.tableID = @tableID
                 ";
                    var cartValue = await dbConnection.QueryAsync(sql,new { tableID = tableID});

                    if (cartValue != null && cartValue.Any())
                    {

                        Response.cartList = cartValue.Select(x => new Cart_tb()
                        {
                            cartID = x.cartID,
                            menuID = x.menuID,
                            menuName = x.menuName,
                            unitPrice = x.unitPrice,
                            imageName = x.imageName,
                            imageSrc = String.Format("https://localhost:7202/Image/{0}", x.imageName),
                            tableID = x.tableID,
                            quantity = x.quantity,
                            optionValue = x.optionValue,
                            netprice = x.unitPrice * x.quantity

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

        //AddCart
        public async Task<CustomerResponse> AddCart(AddCartRequest request)
        {
            var response = new CustomerResponse();
            var cardID = Guid.NewGuid();
            //ฐานข้อมูลเข้าแล้ว
            var sql = @"INSERT INTO Cart_tb (cartID,menuID,tableID,quantity,optionValue)
                        VALUES (@cartID,@menuID,@tableID,@quantity,@optionValue)";
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    var parameters = new
                    {
                        cartID = cardID,
                        menuID = request.menuID,
                        tableID = request.tableID,
                        quantity = 1,
                        optionValue = request.optionValue


                    };
                    var menuValue = await dbConnection.ExecuteAsync(sql, parameters);
                    if (menuValue > 0)
                    {
                        // Set success message
                        response.cartItem = new Cart_tb()
                        {
                            cartID = cardID,
                            menuID = request.menuID,
                            tableID = request.tableID,
                            quantity = 1,
                            optionValue = request.optionValue

                        };
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
        //UpdateCart
        public async Task<CustomerResponse> UpdateCart(UpdateCartRequest request)
        {
            var response = new CustomerResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"UPDATE Cart_tb
                        SET quantity = @quantity
                        WHERE cartID = @cartID";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        quantity = request.quantity,
                        cartID = request.cartID,
                    };

                    // Execute the query
                    int cartValue = await dbConnection.ExecuteAsync(sql, parameters);

                    // Check if the update was successful
                    if (cartValue > 0)
                    {
                        // Populate the booking response with the updated reservation
                        response.cartItem = new Cart_tb

                        {

                            quantity = request.quantity,
                            cartID = request.cartID,
                          
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
        //DeleteCart
        public async Task<CustomerResponse> DeleteCartItem(Guid cartID)
        {
            var response = new CustomerResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"DELETE FROM Cart_tb WHERE cartID = @cartID";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        cartID = cartID,
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
                        response.message = "Delete failed: CartItem not found.";
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

        //AddNeworder
        public async Task<CustomerResponse> AddOrder(AddOrderRequest request)
        {
            Random random = new Random();
            int randomID = random.Next(0,99999);
            string genOrderID = "OD"+randomID.ToString();
            var response = new CustomerResponse();
            try
            {
                var orderSQL = @"INSERT INTO order_tb (orderID,tableID,staftID,orderStatus,orderDate,totalPrice,confirmOrder)
                        VALUES (@orderID,@tableID,@staftID,@orderStatus,@orderDate,@totalPrice,@confirmOrder)";
                var cartSQL = @"SELECT 
                 c.cartID,
                 c.menuID,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 c.tableID,
                 c.quantity,
                 c.optionValue
                 FROM  Cart_tb c
                 LEFT JOIN
                   menu_tb m ON m.menuID = c.menuID 
                WHERE c.tableID = @tableID";

                var orderDetailSQL = @"INSERT INTO orderDetail_tb (orderID,menuID,orderDetailStatus,quantity,optionValue,productiPrice)
                        VALUES (@orderID,@menuID,@orderDetailStatus,@quantity,@optionValue,@productiPrice)";

                var deletecartSQL = @"DELETE FROM Cart_tb WHERE tableID = @tableID";
                var updateMenuSQL = @"UPDATE menu_tb
                        SET stockQuantity = stockQuantity - @quantity
                        WHERE menuID = @menuID";
                var showOrderDetailSQL = @" SELECT * FROM  orderDetail_tb WHERE orderID = @orderID";
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var parameters = new
                    {
                        orderID = genOrderID,
                        tableID = request.tableID,
                        staftID = request.staftID,
                        orderStatus = "ไม่สำเร็จ",
                        orderDate = DateTime.Now,
                        totalPrice = request.totalPrice,
                        confirmOrder = false,
                    };
                    var orderValue = await dbConnection.ExecuteAsync(orderSQL, parameters);

                    if (orderValue > 0) {
                        response.orderItem = new Order_tb()
                        {
                            orderID = genOrderID,
                            tableID = request.tableID,
                            staftID = request.staftID,
                            orderStatus = "ไม่สำเร็จ",
                            orderDate = DateTime.Now,
                            totalPrice = request.totalPrice,
                            confirmOrder = "ยังไม่อนุมัติ",
                        };
                        var cartValue = await dbConnection.QueryAsync<Cart_tb>(cartSQL, new { tableID = request.tableID });
                        if (cartValue != null && cartValue.Any()) {
                            foreach (var item in cartValue) {
                                var orderDetailInput = new OrderDetail_tb()
                                {
                                    orderID = response.orderItem.orderID,
                                    menuID = item.menuID,
                                    orderDetailStatus = "กำลังรอการอนุมัติ",
                                    quantity = item.quantity,
                                    optionValue = item.optionValue,
                                    productiPrice = item.netprice,
                                    menuName = item.menuName,
                                    imageName = item.imageName,
                                    imageSrc = item.imageSrc,
                                };
                                var orderDetailData = await dbConnection.ExecuteAsync(orderDetailSQL, orderDetailInput); //เพิ่มข้อมูลลงในตาราง orderDetail
                                var updateMenuValue = await dbConnection.ExecuteAsync(updateMenuSQL, new { menuID = item.menuID, quantity = item.quantity });
                                if(orderDetailData > 0)
                                {
                                    response.message = "เพิ่มข้อมูลลงในรายการการสั่งซื้อสำเร็จ";
                                    var deletecartData = await dbConnection.ExecuteAsync(deletecartSQL, new { tableID = request.tableID });
                                    if (deletecartData == 0)
                                    {
                                        response.message = "ลบข้อมูลในตะกร้าสำเร็จ";
                                    }
                                    else
                                    {
                                        response.message = "ลบข้อมูลในตะกร้าไม่สำเร็จ";
                                    }

                                }
                                else
                                {
                                    response.message = "เพิ่มข้อมูลลงในรายการการสั่งซื้อไม่สำเร็จ";
                                }
                            }
                          }
                        else
                        {
                            response.message = "ไม่มีสินค้าในตะกร้า";
                        }
                     
                    }
                    else
                    {
                        response.message = "เพิ่มข้อมูลการสั่งอาหารไม่สำเร็จ";
                    }
                }
            }
            catch (Exception ex) { 
                     
                response.message = $"{ex}";

            }
            return response;
        }
        public async Task<CustomerResponse> GetOrderDetail(string orderID)
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
                 od.productiPrice,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 m.stockQuantity
                 FROM  orderDetail_tb od
                 LEFT JOIN 
                   menu_tb m ON m.menuID = od.menuID 
                WHERE od.orderID = @orderID
                 ";
                    var cartValue = await dbConnection.QueryAsync(sql, new { orderID = orderID });

                    if (cartValue != null && cartValue.Any())
                    {

                        Response.orderList = cartValue.Select(x => new OrderDetail_tb()
                        {
                            orderID = x.orderID,
                            menuID = x.menuID,
                            orderDetailStatus = x.orderDetailStatus,
                            quantity = x.quantity,
                            optionValue = x.optionValue,
                            productiPrice = x.unitPrice * x.quantity,
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



    }
}
