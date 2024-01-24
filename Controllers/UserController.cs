using Food.Models;
using Microsoft.AspNetCore.Mvc;

namespace Food.Controllers
{
    public class UserController : Controller
    {
        public IActionResult LoginPage()
        {
            return View("Login");
        }
        public ResponseModel Login(UserModel data)
        {
            ResponseModel response = new();
            if (string.IsNullOrEmpty(data?.UserName))
            {
                response.Status = "error";
                response.Message = "Username not found";
            }
            else if(data.Password == "123")
            {
                response.Status = "success";
                response.Message = "Login Successfully";
                response.Data = new UserModel
                {
                    UserId = 001,
                    UserName = data.UserName,
                    Password = data.Password,
                    Role = "Admin"
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
