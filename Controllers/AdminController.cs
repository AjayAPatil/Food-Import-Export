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
    }
}
