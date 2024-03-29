﻿using Food.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

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

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            UserModel? user = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    user = new UserModel
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = Convert.ToString(reader["UserName"]) ?? "",
                        Password = Convert.ToString(reader["Password"]) ?? "",
                        Role = Convert.ToString(reader["Role"]) ?? "",
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
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


        [HttpPost]
        public ResponseModel RegisterUser(UserModel data)
        {
            ResponseModel response = new();
            //checking username is found
            if (data == null || string.IsNullOrEmpty(data?.UserName))
            {
                response.Status = "error";
                response.Message = "Username not found";
                return response;
            } else if(string.IsNullOrEmpty(data.Password) || string.IsNullOrEmpty (data.ConfirmPassword))
            {
                response.Status = "error";
                response.Message = "Please Enter Password.";
                return response;
            } else if(data.Password != data.ConfirmPassword)
            {
                response.Status = "error";
                response.Message = "Password Not matched!.";
                return response;
            }

            string query = "select * from tbl_UserDetails where username = '" + data.UserName + "'";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                response.Status = "error";
                response.Message = data.UserName + " already registred.";
                response.Data = data;
                connection.Close();
                return response;
            }
            reader.Close();

            query = "INSERT INTO tbl_userdetails(UserName, Password, Role, IsActive, IsDeleted, CreatedOn, FirstName, LastName, Gender, MobileNo, EmailId, Address, City, PinCode) VALUES (@UserName, @Password, @Role, @IsActive, @IsDeleted, @CreatedOn, @FirstName, @LastName, @Gender, @MobileNo, @EmailId, @Address, @City, @PinCode)";

            command = new(query, connection);
            _ = command.Parameters.AddWithValue("@UserName", data.UserName);
            _ = command.Parameters.AddWithValue("@Password", data.Password);
            _ = command.Parameters.AddWithValue("@Role", data.Role);
            _ = command.Parameters.AddWithValue("@IsActive", true);
            _ = command.Parameters.AddWithValue("@IsDeleted", false);
            _ = command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
            _ = command.Parameters.AddWithValue("@FirstName", data.FirstName);
            _ = command.Parameters.AddWithValue("@LastName", data.LastName);
            _ = command.Parameters.AddWithValue("@Gender", data.Gender);
            _ = command.Parameters.AddWithValue("@MobileNo", data.MobileNo);
            _ = command.Parameters.AddWithValue("@EmailId", data.EmailId);
            _ = command.Parameters.AddWithValue("@Address", data.Address);
            _ = command.Parameters.AddWithValue("@City", data.City);
            _ = command.Parameters.AddWithValue("@PinCode", data.PinCode);
            _ = command.ExecuteNonQuery();


            response.Status = "success";
            response.Message = "User registration successfull.";
            response.Data = data;
            connection.Close();
            return response;
        }

        public IActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        public ResponseModel UpdateUser(UserModel data)
        {
            ResponseModel response = new();
            //checking username is found
            if (data == null || string.IsNullOrEmpty(data?.UserName))
            {
                response.Status = "error";
                response.Message = "Username not found";
                return response;
            }
            else if (string.IsNullOrEmpty(data.Password) || string.IsNullOrEmpty(data.ConfirmPassword))
            {
                response.Status = "error";
                response.Message = "Please Enter Password.";
                return response;
            }
            else if (data.Password != data.ConfirmPassword)
            {
                response.Status = "error";
                response.Message = "Password Not matched!.";
                return response;
            }

            string query = "select * from tbl_UserDetails where username = '" + data.UserName + "'";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                response.Status = "error";
                response.Message = data.UserName + " not found.";
                response.Data = data;
                connection.Close();
                return response;
            }
            reader.Close();

            query = "Update tbl_userdetails set Password = @Password, Role = @Role, IsActive = @IsActive, IsDeleted = @IsDeleted, FirstName = @FirstName, LastName = @LastName, Gender = @Gender, MobileNo = @MobileNo, EmailId = @EmailId, Address = @Address, City = @City, PinCode = @PinCode where UserName = @UserName";

            command = new(query, connection);
            _ = command.Parameters.AddWithValue("@UserName", data.UserName);
            _ = command.Parameters.AddWithValue("@Password", data.Password);
            _ = command.Parameters.AddWithValue("@Role", data.Role);
            _ = command.Parameters.AddWithValue("@IsActive", data.IsActive);
            _ = command.Parameters.AddWithValue("@IsDeleted", data.IsDeleted);
            _ = command.Parameters.AddWithValue("@FirstName", data.FirstName);
            _ = command.Parameters.AddWithValue("@LastName", data.LastName);
            _ = command.Parameters.AddWithValue("@Gender", data.Gender);
            _ = command.Parameters.AddWithValue("@MobileNo", data.MobileNo);
            _ = command.Parameters.AddWithValue("@EmailId", data.EmailId);
            _ = command.Parameters.AddWithValue("@Address", data.Address);
            _ = command.Parameters.AddWithValue("@City", data.City);
            _ = command.Parameters.AddWithValue("@PinCode", data.PinCode);
            _ = command.ExecuteNonQuery();


            response.Status = "success";
            response.Message = "Profile updated successfully.";
            response.Data = data;
            connection.Close();
            return response;
        }

    }
}
