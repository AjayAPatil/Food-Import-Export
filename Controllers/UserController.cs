using Food.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Food.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _config;

        public UserController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IActionResult LoginPage()
        {
            return View("Login");
        }
        [HttpGet]
        [HttpPut]

        [HttpPost]
        public ResponseModel Login(UserModel data)
        {
            ResponseModel response = new();
            //checking username is found
            if (data == null || string.IsNullOrEmpty(data?.UserName))
            {
                response.Status = "error";
                response.Message = "Username not found";
                return response;
            }

            string query = "select * from tbl_UserDetails where username = '" + data.UserName + "' and isdeleted = 0";

            MySqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            MySqlCommand command = new(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            UserModel? user = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    user = new UserModel
                    {
                        UserId = reader.GetInt32("UserId"),
                        UserName = reader.GetString("UserName"),
                        Password = reader.GetString("Password"),
                        Role = reader.GetString("Role"),
                        IsActive = reader.GetBoolean("IsActive"),
                        IsDeleted = reader.GetBoolean("IsDeleted"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        FirstName = reader["FirstName"]?.ToString(),
                        LastName = reader["LastName"]?.ToString(),
                        Gender = reader["Gender"]?.ToString(),
                        MobileNo = reader["MobileNo"]?.ToString(),
                        EmailId = reader["EmailId"]?.ToString(),
                        Address = reader["Address"]?.ToString(),
                        City = reader["City"]?.ToString(),
                        PinCode = reader["PinCode"]?.ToString()
                    };
                }
            }


            if (user == null)
            {
                response.Status = "error";
                response.Message = "User Not Found!";
                response.Data = data;
            }
            else if (user.IsActive == false)
            {
                response.Status = "fail";
                response.Message = "Inactive User";
                response.Data = data;
            }
            else if (user.Password == data.Password)
            {
                response.Status = "success";
                response.Message = "Login Successfully";
                response.Data = new
                {
                    user,
                    Dashboard = user.Role == "Admin" ? "/Admin" : "/Home"
                };
            }
            else
            {
                response.Status = "fail";
                response.Message = "Invalid Password";
                response.Data = data;
            }

            return response;
        }
    }
}
