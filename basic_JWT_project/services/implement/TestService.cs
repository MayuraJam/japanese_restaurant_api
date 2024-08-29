using Azure;
using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.response;
using Dapper;
using japanese_resturant_project.services;

namespace japanese_resturant_project.services.implement
{
    public class TestService : Bases,Itest
    {
        public async Task<testResponse> GetMenuData()
        {
            var Response = new testResponse()
            {
                menutest = new List<Menu_tb>()
            };

            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"
                SELECT 
                   *
                 FROM  menu_tb "; // Order by DateTime in descending order
                    
                    // Execute the query
                    var review = await dbConnection.QueryAsync<Menu_tb>(sql);

                    // Check if any reservations were found
                    if (review != null)
                    {
                        // Populate the booking response with the reservations
                        Response.menutest = review.ToList();
                        Response.message = "Review retrieved successfully.";
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
    }
}
