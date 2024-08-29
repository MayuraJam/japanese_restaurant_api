using japanese_resturant_project.services;
using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.request.customerRequest;
using japanese_resturant_project.model.response.customerResponse;
using Dapper;

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
    }
}
