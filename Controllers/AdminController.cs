using Food.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Food.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;

        public AdminController(IConfiguration configuration)
        {
            _config = configuration;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Products
        public IActionResult Products(string uaction, int? id = null)
        {
            string query = "select * from tbl_Products where isdeleted = 0";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            List<ProductModel> productList = new();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel product = new()
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductName = Convert.ToString(reader["ProductName"]) ?? "",
                        Description = Convert.ToString(reader["Description"]) ?? "",
                        MRPrice = Convert.ToDecimal(reader["MRPrice"]),
                        SalePrice = Convert.ToDecimal(reader["SalePrice"]),
                        DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        Images = reader["Images"] == null || reader["Images"] == DBNull.Value ? Array.Empty<byte>() : (byte[])reader["Images"],
                        IsActive = Convert.ToInt32(reader["ProductId"]) == id
                    };
                    if (product.Images != null && product.Images?.Length > 0)
                    {
                        product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(product.Images);
                    }
                    productList.Add(product);
                }
            }

            ProductModel? activeRow = productList.Where(a => a.IsActive).FirstOrDefault();
            if (activeRow == null && productList.Count > 0)
            {
                productList[0].IsActive = true;
                activeRow = productList[0];
            }

            var data = new
            {
                Products = productList,
                CurrentProduct = uaction == "new" || productList == null || productList.Count == 0 ? new ProductModel() : activeRow,
                Action = string.IsNullOrEmpty(uaction) ? "view" : uaction,
                categoryList = GetCategories(id)
            };
            return View("Products", data);
        }

        [HttpPost]
        public IActionResult SaveProduct(ProductModel product)
        {
            ResponseModel response = new();

            if (product == null)
            {
                response.Status = "error";
                response.Message = "Product data Not Found!";
                return Ok(response);
            }
            else if (string.IsNullOrEmpty(product.ProductName))
            {
                response.Status = "error";
                response.Message = "Please enter Product Name";
                return Ok(response);
            }
            else if (string.IsNullOrEmpty(product.Description))
            {
                response.Status = "error";
                response.Message = "Please enter Description";
                return Ok(response);
            }
            else if (product.MRPrice == null || product.MRPrice <= 0)
            {
                response.Status = "error";
                response.Message = "Please enter M.R.P.";
                return Ok(response);
            }
            else if (product.SalePrice == null || product.SalePrice <= 0)
            {
                response.Status = "error";
                response.Message = "Please enter SalePrice";
                return Ok(response);
            }
            try
            {
                if (!string.IsNullOrEmpty(product.ImagesB64))
                {
                    product.Images = Convert.FromBase64String(product.ImagesB64.Split(",")[1]);
                }
                string query = "";
                if (product.ProductId > 0)
                {
                    query = "UPDATE tbl_products SET ProductName=@ProductName, Description=@Description, Images=@Images, MRPrice=@MRPrice, SalePrice=@SalePrice, DiscountPercent=@DiscountPercent, CategoryId=@CategoryId, AvailableQuantity=@AvailableQuantity, AvailableQuantityUnit=@AvailableQuantityUnit, CreatedBy=@CreatedBy, CreatedOn=@CreatedOn,IsDeleted=@IsDeleted WHERE ProductId=" + product.ProductId.ToString();
                    response.Status = "success";
                    response.Message = "Updated Successfully";
                    response.Data = product;
                }
                else
                {
                    product.CreatedOn = DateTime.Now;
                    query = "INSERT INTO tbl_products(ProductName, Description, Images, MRPrice, SalePrice, DiscountPercent, CategoryId, AvailableQuantity, AvailableQuantityUnit, CreatedBy, CreatedOn, IsDeleted) VALUES (@ProductName,@Description,@Images,@MRPrice,@SalePrice,@DiscountPercent,@CategoryId,@AvailableQuantity,@AvailableQuantityUnit,@CreatedBy,@CreatedOn,@IsDeleted)";
                    response.Status = "success";
                    response.Message = "Added Successfully";
                    response.Data = product;
                }
                SqlConnection connection = new()
                {
                    ConnectionString = _config.GetConnectionString("DefaultConnection")
                };
                connection.Open();
                SqlCommand command = new(query, connection);
                _ = command.Parameters.AddWithValue("@ProductName", product.ProductName);
                _ = command.Parameters.AddWithValue("@Description", product.Description);
                _ = command.Parameters.AddWithValue("@Images", product.Images);
                _ = command.Parameters.AddWithValue("@MRPrice", product.MRPrice);
                _ = command.Parameters.AddWithValue("@SalePrice", product.SalePrice);
                _ = command.Parameters.AddWithValue("@DiscountPercent", product.DiscountPercent);
                _ = command.Parameters.AddWithValue("@AvailableQuantity", product.AvailableQuantity);
                _ = command.Parameters.AddWithValue("@AvailableQuantityUnit", product.AvailableQuantityUnit);
                _ = command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                _ = command.Parameters.AddWithValue("@CreatedBy", product.CreatedBy);
                _ = command.Parameters.AddWithValue("@CreatedOn", product.CreatedOn);
                _ = command.Parameters.AddWithValue("@IsDeleted", product.IsDeleted);
                _ = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                response.Status = "error";
                response.Message = ex.Message + "_" + (ex.InnerException?.Message ?? "");
            }
            return Ok(response);
        }
        #endregion

        #region Categories
        public IActionResult Categories(string uaction, int? id = null)
        {
            List<CategoryModel> categoryList = GetCategories(id).FindAll(a => a.CategoryName != "none");

            CategoryModel? activeRow = categoryList.Where(a => a.IsActive).FirstOrDefault();
            if (activeRow == null && categoryList.Count > 0)
            {
                categoryList[0].IsActive = true;
                activeRow = categoryList[0];
            }

            var data = new
            {
                Categories = categoryList,
                CurrentCategory = uaction == "new" || categoryList == null || categoryList.Count == 0 ? new CategoryModel() : activeRow,
                Action = string.IsNullOrEmpty(uaction) ? "view" : uaction,
            };
            return View("ProductCategories", data);
        }


        [HttpPost]
        public IActionResult SaveCategory(CategoryModel category)
        {
            ResponseModel response = new();

            if (category == null)
            {
                response.Status = "error";
                response.Message = "Category data Not Found!";
                return Ok(response);
            }
            else if (string.IsNullOrEmpty(category.CategoryName))
            {
                response.Status = "error";
                response.Message = "Please enter Category Name";
                return Ok(response);
            }
            else if (string.IsNullOrEmpty(category.Description))
            {
                response.Status = "error";
                response.Message = "Please enter Description";
                return Ok(response);
            }

            try
            {

                string query = "";
                if (category.CategoryId > 0)
                {
                    query = "UPDATE tbl_ProductCategories SET CategoryName=@CategoryName, Description=@Description, CreatedBy=@CreatedBy, CreatedOn=@CreatedOn,IsDeleted=@IsDeleted WHERE CategoryId=" + category.CategoryId.ToString();
                    response.Status = "success";
                    response.Message = "Updated Successfully";
                    response.Data = category;
                }
                else
                {
                    category.CreatedOn = DateTime.Now;
                    query = "INSERT INTO tbl_ProductCategories(CategoryName, Description,CreatedBy, CreatedOn, IsDeleted) VALUES (@CategoryName,@Description,@CreatedBy,@CreatedOn,@IsDeleted)";
                    response.Status = "success";
                    response.Message = "Added Successfully";
                    response.Data = category;
                }
                SqlConnection connection = new()
                {
                    ConnectionString = _config.GetConnectionString("DefaultConnection")
                };
                connection.Open();
                SqlCommand command = new(query, connection);
                _ = command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                _ = command.Parameters.AddWithValue("@Description", category.Description);
                _ = command.Parameters.AddWithValue("@CreatedBy", category.CreatedBy);
                _ = command.Parameters.AddWithValue("@CreatedOn", category.CreatedOn);
                _ = command.Parameters.AddWithValue("@IsDeleted", category.IsDeleted);
                _ = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                response.Status = "error";
                response.Message = ex.Message + "_" + (ex.InnerException?.Message ?? "");
            }
            return Ok(response);
        }

        public List<CategoryModel> GetCategories(int? id = null)
        {
            string query = "select * from tbl_ProductCategories where isdeleted = 0";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            List<CategoryModel> categoryList = new();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CategoryModel category = new()
                    {
                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                        CategoryName = Convert.ToString(reader["CategoryName"]) ?? "",
                        Description = Convert.ToString(reader["Description"]) ?? "",
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        IsActive = Convert.ToInt32(reader["CategoryId"]) == id
                    };
                    categoryList.Add(category);
                }
            }
            reader.Close();
            connection.Close();

            return categoryList;
        }
        #endregion

        #region Coupon 
        public IActionResult Coupon(string uaction, int? id = null)
        {
            List<MasterCouponModel> couponList = GetCoupon(id);

            MasterCouponModel? activeRow = couponList.Where(a => a.IsActive).FirstOrDefault();
            if (activeRow == null && couponList.Count > 0)
            {
                couponList[0].IsActive = true;
                activeRow = couponList[0];
            }

            var data = new
            {
                Coupons = couponList,
                CurrentCoupon = uaction == "new" || couponList == null || couponList.Count == 0 ? new MasterCouponModel() : activeRow,
                Action = string.IsNullOrEmpty(uaction) ? "view" : uaction,
            };
            return View("Coupon", data);
        }

        [HttpPost]
        public IActionResult SaveCoupon(MasterCouponModel coupon)
        {
            ResponseModel response = new();

            if (coupon == null)
            {
                response.Status = "error";
                response.Message = "Coupon data Not Found!";
                return Ok(response);
            }
            else if (string.IsNullOrEmpty(coupon.CouponCode))
            {
                response.Status = "error";
                response.Message = "Please enter Coupon Code";
                return Ok(response);
            }
            else if (coupon.CouponDiscount <= 0)
            {
                response.Status = "error";
                response.Message = "Please enter Discount";
                return Ok(response);
            }

            try
            {

                string query = "";
                if (coupon.CouponId > 0)
                {
                    query = "UPDATE tbl_MasterCoupon SET CouponCode=@CouponCode, CouponDiscount=@CouponDiscount, IsDeleted=@IsDeleted WHERE CouponId=" + coupon.CouponId.ToString();

                    SqlConnection connection = new()
                    {
                        ConnectionString = _config.GetConnectionString("DefaultConnection")
                    };
                    connection.Open();
                    SqlCommand command = new(query, connection);
                    _ = command.Parameters.AddWithValue("@CouponCode", coupon.CouponCode);
                    _ = command.Parameters.AddWithValue("@CouponDiscount", coupon.CouponDiscount);
                    _ = command.Parameters.AddWithValue("@IsDeleted", coupon.IsDeleted);
                    _ = command.ExecuteNonQuery();

                    response.Status = "success";
                    response.Message = "Updated Successfully";
                    response.Data = coupon;
                }
                else
                {
                    coupon.CreatedOn = DateTime.Now;
                    query = "INSERT INTO tbl_MasterCoupon(CouponCode, CouponDiscount,CreatedBy, CreatedOn, IsDeleted) VALUES (@CouponCode,@CouponDiscount,@CreatedBy,@CreatedOn,@IsDeleted)";

                    SqlConnection connection = new()
                    {
                        ConnectionString = _config.GetConnectionString("DefaultConnection")
                    };
                    connection.Open();
                    SqlCommand command = new(query, connection);
                    _ = command.Parameters.AddWithValue("@CouponCode", coupon.CouponCode);
                    _ = command.Parameters.AddWithValue("@CouponDiscount", coupon.CouponDiscount);
                    _ = command.Parameters.AddWithValue("@CreatedBy", coupon.CreatedBy);
                    _ = command.Parameters.AddWithValue("@CreatedOn", coupon.CreatedOn);
                    _ = command.Parameters.AddWithValue("@IsDeleted", coupon.IsDeleted);
                    _ = command.ExecuteNonQuery();

                    response.Status = "success";
                    response.Message = "Added Successfully";
                    response.Data = coupon;
                }
            }
            catch (Exception ex)
            {
                response.Status = "error";
                response.Message = ex.Message + "_" + (ex.InnerException?.Message ?? "");
            }
            return Ok(response);
        }

        public List<MasterCouponModel> GetCoupon(int? id = null)
        {
            string query = "select * from tbl_MasterCoupon where isdeleted = 0";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            List<MasterCouponModel> couponList = new();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    MasterCouponModel coupon = new()
                    {
                        CouponId = Convert.ToInt32(reader["CouponId"]),
                        CouponCode = Convert.ToString(reader["CouponCode"]) ?? "",
                        CouponDiscount = Convert.ToDecimal(reader["CouponDiscount"]),
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                        IsActive = Convert.ToInt32(reader["CouponId"]) == id
                    };
                    couponList.Add(coupon);
                }
            }
            reader.Close();
            connection.Close();

            return couponList;
        }

        public IActionResult VerifyCoupon(string couponCode, int userId)
        {
			ResponseModel response = new();

			if (string.IsNullOrEmpty(couponCode))
			{
				response.Status = "error";
				response.Message = "Coupon code Not Found!";
				return Ok(response);
			}
			string query = "select top 1 " +
                "isnull((select top 1 o.OrderId from tbl_Orders o where o.CreatedBy = @CreatedBy and o.CouponId = ISNULL(c.CouponId, 0)), 0) as OrderId," +
				" c.*" +
                " from tbl_MasterCoupon c where isdeleted = 0 and couponCode= @CouponCode";

			SqlConnection connection = new()
			{
				ConnectionString = _config.GetConnectionString("DefaultConnection")
			};
			connection.Open();
			SqlCommand command = new(query, connection);
			_ = command.Parameters.AddWithValue("@CouponCode", couponCode);
			_ = command.Parameters.AddWithValue("@CreatedBy", userId);
			SqlDataReader reader = command.ExecuteReader();

            MasterCouponModel coupon = new();
			if (reader.HasRows)
			{
				while (reader.Read())
				{
                    if (Convert.ToInt32(reader["OrderId"]) > 0)
                    {
                        response.Status = "error";
                        response.Message = "Coupon Already Used!";
                    }
                    else
                    {
                        coupon = new()
                        {
                            CouponId = Convert.ToInt32(reader["CouponId"]),
                            CouponCode = Convert.ToString(reader["CouponCode"]) ?? "",
                            CouponDiscount = Convert.ToDecimal(reader["CouponDiscount"]),
                            CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            IsDeleted = Convert.ToBoolean(reader["IsDeleted"])
                        };
                    }
				}
				response.Status = "success";
				response.Message = "Coupon applied successfully.";
                response.Data = coupon;
			}
			else
			{
				response.Status = "error";
				response.Message = "Coupon Not Found!";
			}
			reader.Close();
			connection.Close();

			return Ok(response);
        }
		#endregion

		#region Users 
		public IActionResult Users(string uaction, int? id = null)
        {
            List<UserModel> userList = GetUsers(id);

            UserModel? activeRow = userList.Where(a => a.IsActive).FirstOrDefault();
            if (activeRow == null && userList.Count > 0)
            {
                userList[0].IsUpdate = true;
                activeRow = userList[0];
            }

            var data = new
            {
                Users = userList,
                CurrentUser = uaction == "new" || userList == null || userList.Count == 0 ? new UserModel() : activeRow,
                Action = string.IsNullOrEmpty(uaction) ? "view" : uaction,
            };
            return View("Users", data);
        }

        [HttpGet]
        public IActionResult UpdateUsers(string updateaction, int userId)
        {
            ResponseModel response = new();

            if (string.IsNullOrEmpty(updateaction))
            {
                response.Status = "error";
                response.Message = "Action Not Found!";
                return Ok(response);
            }
            else if (userId <= 0)
            {
                response.Status = "error";
                response.Message = "User Id no found";
                return Ok(response);
            }

            try
            {

                string query = "";
                if (userId > 0)
                {
                    query = "UPDATE tbl_UserDetails SET IsDeleted=@IsDeleted, IsActive=@IsActive, Password=@Password WHERE UserId=" + userId.ToString();
                    response.Status = "success";
                    response.Message = "Updated Successfully";
                    response.Data = userId;
                    if(updateaction == "delete")
                    {
                        query = "UPDATE tbl_UserDetails SET IsDeleted=1 WHERE UserId=" + userId.ToString();
                        response.Message = "User Deleted Successfully";
                    } else if(updateaction == "activate")
                    {
                        query = "UPDATE tbl_UserDetails SET IsActive=1 WHERE UserId=" + userId.ToString();
                        response.Message = "User Activated Successfully";
                    } else if(updateaction == "deactivate")
                    {
                        query = "UPDATE tbl_UserDetails SET IsActive=0 WHERE UserId=" + userId.ToString();
                        response.Message = "User Deactivated Successfully";
                    } else if(updateaction == "resetpassword")
                    {
                        query = "UPDATE tbl_UserDetails SET Password='Pass@123' WHERE UserId=" + userId.ToString();
                        response.Message = "Password Reset Successfully. Your new password is \"Pass@123\"";
                    } else
                    {
                        response.Status = "error";
                        response.Message = "Invalid action..";
                        response.Data = userId;
                        return Ok(response);
                    }
                    SqlConnection connection = new()
                    {
                        ConnectionString = _config.GetConnectionString("DefaultConnection")
                    };
                    connection.Open();
                    SqlCommand command = new(query, connection);
                    _ = command.ExecuteNonQuery();
                }
                else
                {
                    response.Status = "error";
                    response.Message = "User not found";
                    response.Data = userId;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = "error";
                response.Message = ex.Message + "_" + (ex.InnerException?.Message ?? "");
            }
            return Ok(response);
        }

        public List<UserModel> GetUsers(int? id = null)
        {
            string query = "select * from tbl_UserDetails where isdeleted = 0";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            List<UserModel> userList = new();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    UserModel user = new()
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = Convert.ToString(reader["UserName"]) ?? "",
                        Password = Convert.ToString(reader["Password"]) ?? "",
                        FirstName = Convert.ToString(reader["FirstName"]) ?? "",
                        LastName = Convert.ToString(reader["LastName"]) ?? "",
                        MobileNo = Convert.ToString(reader["MobileNo"]) ?? "",
                        EmailId = Convert.ToString(reader["EmailId"]) ?? "",
                        Role = Convert.ToString(reader["Role"]) ?? "",
                        Address = Convert.ToString(reader["Address"]) ?? "",
                        City = Convert.ToString(reader["City"]) ?? "",
                        Gender = Convert.ToString(reader["Gender"]) ?? "",
                        PinCode = Convert.ToString(reader["PinCode"]) ?? "",
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        
                        IsUpdate = Convert.ToInt32(reader["UserId"]) == id
                    };
                    userList.Add(user);
                }
            }
            reader.Close();
            connection.Close();

            return userList;
        }
        #endregion
    }
}
