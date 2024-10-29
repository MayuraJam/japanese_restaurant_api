using japanese_resturant_project.services.Interfaces;
using japanese_resturant_project.model.response;
using Microsoft.AspNetCore.Mvc;
using japanese_resturant_project.model.request;
using Dapper;
using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.model.response.adminResponse;


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
            var memberID = Guid.NewGuid();
            //var pointID = Guid.NewGuid();
            Random random = new Random();
            int randomID = random.Next(0, 99999);
            string pointID = "P" + randomID.ToString();
            var staftID = "STAFT"+randomID.ToString();

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
                        var sqlCustomer = @"INSERT INTO member_tb (memberID, firstName,lastName, phone,userID,email,password,roleName,totalPoint)
                        VALUES (@memberID,@firstName,@lastName, @phone,@userID,@email,@password,@roleName,@totalPoint)";
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
                            totalPoint = 10

                        };
                       var customer =  dbConnection.Execute(sqlCustomer,parameterCustomer); //นำค่าใส่ตารางลูกค้า

                        var sqlpoint = @"INSERT INTO point_tb (pointID,currentPoint,description,createDate,memberID)
                        VALUES (@pointID,@currentPoint,@description,@createDate,@memberID)";
                        // Use parameterized query to prevent SQL injection
                        var parameterpoint = new
                        {
                            pointID = pointID,
                            currentPoint = 10,
                            description = "ได้รับแต้ม เนื่องจากเป็นสมาชิกใหม่",
                            createDate = DateTime.Now,
                            memberID = memberID
                        };
                        var customerecord = dbConnection.Execute(sqlpoint,parameterpoint); //นำค่าใส่ตารางคะแนน

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

                        var sqlAdmin = @"INSERT INTO staft_tb (staftID,firstName,lastName, phone,createDate,updateDate,userID,email,password,roleName,accountStatus,jobType)
                        VALUES (@staftID, @firstName,@lastName, @phone,@createDate,@updateDate,@userID,@email,@password,@roleName,@accountStatus,@jobType)";

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
                            accountStatus = "อยู่ในระบบ",
                             jobType = request.jobType,
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
      public async Task<UserResponseModel> ToLoginCustomer(Login request)
        {
           var response = new UserResponseModel();
            Random random = new Random();
            int randomID = random.Next(0, 99999);
            string pointID = "P" + randomID.ToString();
            //เข้าผ่านของ customer 
            //var SqlAutn = @"SELECT * FROM Authentication_tb WHERE email = @email AND password = @password";
            var sqlCustomer = @"SELECT 
                          c.memberID,c.firstName,c.lastName,c.phone,c.userID,c.email,c.password,c.roleName,c.userID,c.totalPoint,p.pointID,p.currentPoint,p.description,p.createDate
                          FROM  member_tb c
                          LEFT JOIN
                          point_tb p ON p.memberID = c.memberID 
                          WHERE c.phone = @phone AND c.roleName = @roleName 
                          ORDER BY p.createDate DESC;
                         ";
            //ส่วนแสดงข้อมูลของ ตาราง AUthentication 
            using (var dbConnection = CreateSQLConnection())
            {
                var paramLogin = new
                {
                    phone = request.phone,
                    roleName = request.roleName
                };
                try
                {
                var authValue = await dbConnection.QueryFirstOrDefaultAsync<Customer_tb>(sqlCustomer, paramLogin);
                if (authValue != null)
                {

                    
                  if (request.pointType == "เพิ่มคะแนน")
                  {
                    var sqlPoint = @"
                                            INSERT INTO point_tb (pointID, currentPoint,description,createDate,memberID)
                                            VALUES (@pointID, @currentPoint,@description,@createDate,@memberID);";

                    var UpdatecustomerSQL = @"UPDATE member_tb SET totalPoint = totalPoint + @currentPoint WHERE memberID = @memberID";
                    //อัปเดตในส่วนของลูกค้า
                    var parameterpoint = new
                    {
                        pointID = pointID,
                        memberID = authValue.memberID,
                        currentPoint = calculatePoint(request.totalPrice),
                        description = "ได้รับแต้ม",
                        createDate = DateTime.Now,
                    };
                    var pointValue = await dbConnection.ExecuteAsync(sqlPoint, parameterpoint);

                    if (pointValue > 0)
                    {
                        await dbConnection.ExecuteAsync(UpdatecustomerSQL, new { currentPoint = parameterpoint.currentPoint, memberID =parameterpoint.memberID});

                                var memberValue = dbConnection.Query<Customer_tb, Point_tb, Customer_tb>(sqlCustomer, (customer, point) =>
                                {
                                    customer.pointlList = customer.pointlList ?? new List<Point_tb>();
                                    if (point != null)
                                    {

                                        customer.pointlList.Add(point);
                                    }
                                    return customer;
                                }, paramLogin, splitOn: "pointID").GroupBy(o => o.memberID).Select(g =>
                                {
                                    var groupPointList = g.First();
                                    groupPointList.pointlList = g.SelectMany(o => o.pointlList).ToList();
                                    groupPointList.countOfPoint = g.Count();
                                    return groupPointList;
                                }).ToList();
                                if (memberValue != null && memberValue.Any())
                                {
                                    response.customerList = memberValue.ToList();
                                    response.message = "successfully.";
                                    response.success = true;
                                }
                                else
                                {
                                    response.customerList = new List<Customer_tb>();
                                    response.message = "Not found data 404.";
                                    response.success = false;

                                }

                                response.message = "เพิ่มข้อมูลสำเร็จ";
                        response.success = true;
                    }
                    else
                    {
                        response.message = "เพิ่มข้อมูลไม่สำเร็จ";
                        response.success = false;
                    }
                  }
                 else if (request.pointType == "ลดคะแนน")
                        {

                            if(authValue.totalPoint >= request.totalPrice)
                            {
                            var sqlPoint = @"
                                            INSERT INTO point_tb (pointID, currentPoint,description,createDate,memberID)
                                            VALUES (@pointID, @currentPoint,@description,@createDate,@memberID);";
                            var UpdatecustomerSQL = @"UPDATE member_tb SET totalPoint = totalPoint - @currentPoint WHERE memberID = @memberID";
                            //อัปเดตในส่วนของลูกค้า
                            var parameterpoint = new
                            {
                                pointID = pointID,
                                memberID = authValue.memberID,
                                currentPoint = request.totalPrice,
                                description = "ใช้แต้มในการชำระสินค้า",
                                createDate = DateTime.Now,
                            };

                            var pointValue = await dbConnection.ExecuteAsync(sqlPoint, parameterpoint);

                            if (pointValue > 0)
                            {
                                    await dbConnection.ExecuteAsync(UpdatecustomerSQL, new { currentPoint = parameterpoint.currentPoint, memberID = parameterpoint.memberID });
                                    var memberValue = dbConnection.Query<Customer_tb, Point_tb, Customer_tb>(sqlCustomer, (customer, point) =>
                                    {
                                        customer.pointlList = customer.pointlList ?? new List<Point_tb>();
                                        if (point != null)
                                        {
                                            customer.pointlList.Add(point);
                                        }
                                        return customer;
                                    },paramLogin, splitOn: "pointID").GroupBy(o => o.memberID).Select(g =>
                                    {
                                        var groupPointList = g.First();
                                        groupPointList.pointlList = g.SelectMany(o => o.pointlList).ToList();
                                        groupPointList.countOfPoint = g.Count();
                                        return groupPointList;
                                    }).ToList();
                                    if (memberValue != null && memberValue.Any())
                                    {
                                        response.customerList = memberValue.ToList();
                                        response.message = "successfully.";
                                        response.success = true;
                                    }
                                    else
                                    {
                                        response.message = "Not found data 404.";
                                        response.success = false;

                                    }
                                   

     
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
                                response.message = $"แต้มคะแนนไม่เพียงพอ";
                                response.success = false;
                                response.pesentpoint = authValue.totalPoint;
                            }
                        }

                }
                else
                {
                    response.message = "ไม่พบบัญชีผู้ใช้งานรายนี้";
                    response.success = false;
                }

                }
                catch(Exception ex) {
                 
                     response.message = ex.Message;
                    response.success = false;
                }
                return response;
            }
        }

        public int calculatePoint(decimal? totalPrice)
        {
            if (totalPrice <= 80)
            {
                return 0; // ไม่มีแต้มสำหรับราคารวมต่ำกว่า 81 บาท
            }
            else if (totalPrice > 80 && totalPrice <= 150)
            {
                return (int)((double)totalPrice * 0.05); // 5% สำหรับราคาระหว่าง 81-150 บาท
            }
            else if (totalPrice > 150 && totalPrice <= 300)
            {
                return (int)((double)totalPrice * 0.1); // 7% สำหรับราคาระหว่าง 151-300 บาท
            }
            else if (totalPrice > 300 && totalPrice <= 600)
            {
                return (int)((double)totalPrice * 0.15); // 10% สำหรับราคาระหว่าง 301-600 บาท
            }
            else // ครอบคลุมกรณีที่มากกว่า 600 บาท
            {
                return (int)((double)totalPrice * 0.20); // 15% สำหรับราคาที่เกิน 600 บาท
            }
        }

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
                            staftID,firstName,lastName,phone,userID,createDate,updateDate,userID,email,password,roleName,accountStatus,jobType
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
        public Task<UserResponseModel> GetMember(string roleName)
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
                          c.memberID,c.firstName,c.lastName,c.phone,c.userID,c.email,c.password,c.roleName,c.userID,c.totalPoint,p.pointID,p.currentPoint,p.description,p.createDate
                          FROM  member_tb c
                          LEFT JOIN
                          point_tb p ON p.memberID = c.memberID
                          ";
                        var memberValue = dbConnection.Query<Customer_tb, Point_tb, Customer_tb>(sql, (customer, point) =>
                        {
                            customer.pointlList = customer.pointlList?? new List<Point_tb>();
                            if(point != null)
                            {
                                customer.pointlList.Add(point);
                            }
                            return customer;
                        },splitOn: "pointID").GroupBy(o => o.memberID).Select(g =>
                        {
                            var groupPointList = g.First();
                            groupPointList.pointlList = g.SelectMany(o => o.pointlList).ToList();
                            groupPointList.countOfPoint = g.Count();
                            return groupPointList;
                        }).ToList();
                            if (memberValue != null && memberValue.Any())
                        {
                            response.customerList = memberValue.ToList();
                            response.message = "successfully.";
                            response.success = true;
                        }
                        else
                        {
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
                          staftID,firstName,lastName,phone,userID,createDate,updateDate,userID,email,password,roleName,accountStatus,jobType
                          FROM  staft_tb
                          ";
                        var memberValue = dbConnection.Query<Staft_Authentication_tb>(sql);

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
           return Task.FromResult(response); ;
        }

        //logoutStaft ตรวจสอบด้วยรหัสพนักงาน
        public async Task<UserResponseModel> LogoutStaft(string staftID)
        {
            var response = new UserResponseModel();

            using (var dbConnection = CreateSQLConnection())
            {


                    try
                    {
                       
                            var sql2 = @"UPDATE staft_tb
                            SET accountStatus = @accountStatus
                            WHERE staftID = @staftID
                          ";
                            var parameterStaft = new
                            {
                                staftID = staftID,
                                accountStatus = "ออกจากระบบ"
                            };

                            var memberValue = await dbConnection.ExecuteAsync(sql2, parameterStaft);
                            if (memberValue > 0)
                            {
                                response.success = true;
                                response.message = "ออกจากระบบสำเร็จ";
                            }
                            else
                            {
                                response.success = false;
                                response.message = "ออกจากระบบไม่สำเร็จ";
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

        //เข้าสู่ระบบสะสมแต้ม เพื่อทำการดึงคะแนนรวมของแต้ม โดยค่าที่ส่งไปคือ ราคารวมสินค้า+ภาษีแล้ว email , password , roleName แสดงผลออกมาเป็น ผลรวมแต้มที่โดนหัก ต้องเชื่อมตารางลูกค้ากับคะแนน

        //ลบบัญชีสมาชิก 
        public async Task<UserResponseModel> DeleteMemberAccount(Guid memberID)
        {
            var response = new UserResponseModel();
            try
            {
                using (var dbConnection = CreateSQLConnection()) // Establish database connection
                {
                    var sql = @"DELETE FROM member_tb WHERE memberID = @memberID";
                    var sqlPoint = @"DELETE FROM point_tb WHERE memberID = @memberID";

                    var parameters = new
                    {
                        memberID = memberID,
                    };


                    int rowsAffected = await dbConnection.ExecuteAsync(sql, parameters);


                    if (rowsAffected > 0)
                    {
                        await dbConnection.ExecuteAsync(sqlPoint,parameters); //ลบคะแนนทั้งหมด
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
    }
}
