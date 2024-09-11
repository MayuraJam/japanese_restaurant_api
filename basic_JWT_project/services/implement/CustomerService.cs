using japanese_resturant_project.services;
using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.request.customerRequest;
using japanese_resturant_project.model.response.customerResponse;
using Dapper;
using japanese_resturant_project.model.response.adminResponse;
using Microsoft.AspNetCore.Mvc;
using japanese_resturant_project.model.request;

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
                            tableStatus = "จองแล้ว"
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
        
        public async Task<CustomerResponse> GetCartBytableID([FromBody] Guid tableID)
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
                 c.cartID
                 c.menuID,
                 m.menuName,
                 m.unitPrice,
                 m.imageName,
                 c.tableID,
                 c.quantity,
                 c.optionValue,
                 FROM  Cart_tb c
                 LEFT JOIN
                   menu_tb m ON m.menuID = c.menuID
                 ";
                    var cartValue = await dbConnection.QueryAsync(sql);

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

                        }).ToList();

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

    }
}
