using Food.Common;
using Food.Models;
using Microsoft.AspNetCore.Mvc;

namespace Food.Controllers
{
    public class UserController : Controller
    {
        private readonly FoodDBContext _dbContext;
        private readonly IConfiguration _config;

        public UserController(FoodDBContext dBContext, IConfiguration configuration)
        {
            _dbContext = dBContext;
            _config = configuration;
        }
        public IActionResult LoginPage()
        {
            return View("Login");
        }
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

            //string query = "select  `UserId`, `UserName`, `Password`, `Role`, `IsActive`, `IsDeleted`, `CreatedOn`, `FirstName`, `LastName`, `Gender`, `MobileNo`, `EmailId`, `Address`, `City`, `PinCode` from tbl_UserDetails where username = '" + data.UserName + "' and isdeleted = 0";

            //UserModel? user = CommonFunctions.ExecuteQuery<UserModel>(_config.GetConnectionString("DefaultConnection"), query);

            var user = _dbContext.UserDetails.Where(a=> a.IsDeleted == false && a.UserName == data.UserName).FirstOrDefault();
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
                    user = user,
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
