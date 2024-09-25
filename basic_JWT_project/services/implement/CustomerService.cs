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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;

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
                 c.optionValue,
                 c.netprice
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
            var sql = @"INSERT INTO Cart_tb (cartID,menuID,tableID,quantity,optionValue,netprice)
                        VALUES (@cartID,@menuID,@tableID,@quantity,@optionValue,@netprice)";
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
                        optionValue = request.optionValue,
                        netprice = request.unitPrice

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
                            optionValue = request.optionValue,
                            netprice = request.unitPrice


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
                        SET quantity = @quantity,
                        netprice = @netprice
                        WHERE cartID = @cartID";

                    // Use parameterized query to prevent SQL injection
                    var parameters = new
                    {
                        quantity = request.quantity,
                        cartID = request.cartID,
                        netprice = request.unitPrice * request.quantity
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
                            netprice = request.unitPrice*request.quantity
                          
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

            //string genOrderDetailID = "ODT" + randomID.ToString();
            var response = new CustomerResponse();
            try
            {
                var orderSQL = @"INSERT INTO order_tb (orderID,tableID,staftID,orderStatus,orderDate,totalPrice,confirmOrder,paymentStatus)
                        VALUES (@orderID,@tableID,@staftID,@orderStatus,@orderDate,@totalPrice,@confirmOrder,@paymentStatus)";
                var cartSQL = @"SELECT 
                 c.cartID,
                 c.menuID,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 c.tableID,
                 c.quantity,
                 c.optionValue,
                 c.netprice
                 FROM  Cart_tb c
                 LEFT JOIN
                   menu_tb m ON m.menuID = c.menuID 
                WHERE c.tableID = @tableID";

                var orderDetailSQL = @"INSERT INTO orderDetail_tb (orderID,menuID,orderDetailStatus,quantity,optionValue,netprice)
                        VALUES (@orderID,@menuID,@orderDetailStatus,@quantity,@optionValue,@netprice)";

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
                        confirmOrder = "ยังไม่อนุมัติ",
                        paymentStatus = "ยังไม่ได้ชำระ"
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
                            paymentStatus = "ยังไม่ได้ชำระ"
                        };
                              
                        var cartValue = await dbConnection.QueryAsync<Cart_tb>(cartSQL, new { tableID = request.tableID });
                        if (cartValue != null && cartValue.Any()) {

                              
                            foreach (Cart_tb item in cartValue) {
                               // var randomID2 = random.Next(0, 99999);
                                var orderDetailInput = new OrderDetail_tb()
                                {
                                   // orderDetailID = "ODT" + randomID2.ToString(),
                                    orderID = response.orderItem.orderID,
                                    menuID = item.menuID,
                                    orderDetailStatus = "กำลังรอการอนุมัติ",
                                    quantity = item.quantity,
                                    optionValue = item.optionValue,
                                    netprice = item.netprice,
                                    menuName = item.menuName,
                                    imageName = item.imageName,
                                    imageSrc = item.imageSrc,
                                };
                                var orderDetailData = await dbConnection.ExecuteAsync(orderDetailSQL, orderDetailInput); //เพิ่มข้อมูลลงในตาราง orderDetail
                                Console.WriteLine($"Added OrderDetail for MenuID: {item.menuID}, Price: {item.netprice}");
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
                 od.netprice,
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

        public  Task<CustomerResponse> GetOrder(string tableID)
        {
            var Response = new CustomerResponse()
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
                WHERE o.tableID = @tableID
                 ";
                    var parameter = new
                    {
                        tableID = tableID
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
                    }, parameter, splitOn: "menuID").GroupBy(o => o.orderID).Select(g =>
                    {
                        var groupOrderList = g.First();
                        groupOrderList.OrderDetailList = g.SelectMany(o=>o.OrderDetailList).ToList();
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

        public async Task<CustomerResponse> CancleOrder(string orderID) //ถ้าลบของใน stock ก็ต้องกลับมาจำนวนเท่าเดิม
        {
            var response = new CustomerResponse();
            try
            {
                using (var dbConnection = CreateSQLConnection())
                {
                    var sqlorder = @"UPDATE order_tb
                                   SET orderStatus = @orderStatus,
                                   confirmOrder = @confirmOrder
                                   WHERE orderID = @orderID";
                    var sqlorderDetail = @"UPDATE orderDetail_tb 
                                           SET orderDetailStatus = @orderDetailStatus
                                           WHERE orderID = @orderID";
                    var selectmenufromorderDetail = @"SELECT menuID,quantity FROM orderDetail_tb WHERE orderID = @orderID";

                    var updateMenuQuantity = @"UPDATE menu_tb
                                            SET stockQuantity = stockQuantity + @quantity
                                            WHERE menuID = @menuID";
                    //menuID ไปดึงมาจาก ตาราง orderDETAIL
                    var parameters = new
                    {
                        orderID = orderID,
                        orderStatus = "รายการถูกยกเลิก",
                        confirmOrder = "ยกเลิกรายการสั่งนี้"
                    };
                    int rowsAffected = await dbConnection.ExecuteAsync(sqlorder, parameters);
                    if (rowsAffected > 0)
                    {
                        var showOrderDetailvalue = await dbConnection.QueryAsync(selectmenufromorderDetail,new {orderID = orderID });
                        foreach (var detail in showOrderDetailvalue)
                        {
                            await dbConnection.ExecuteAsync(updateMenuQuantity, new
                            {
                                quantity = detail.quantity,
                                menuID = detail.menuID
                            });
                        Console.WriteLine("quantity : "+detail.quantity+ "menuID :" +detail.menuID);
                        }

                        var parameter2 = new
                        {
                            orderID = orderID,
                            orderDetailStatus = "รายการถูกยกเลิก"
                        };
                        var deleteOrderDetail = await dbConnection.ExecuteAsync( sqlorderDetail, parameter2);
                        response.message = "ลบรายการสำเร็จ";
                        response.success = true;

                    }
                    else
                    {
                        response.message = "ลบรายการไม่สำเร็จ เนื่องจากไม่พบ เลขหมายรายการดักล่าว";
                        response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                response.message = $"Delete failed: {ex.Message}";
            }

            return response;
        }

        //ชำระเงิน ชำระในทุกๆ order ที่มีหมายเลขโต๊ะเดียวกัน

    }
}
