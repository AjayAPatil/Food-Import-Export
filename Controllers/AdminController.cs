using Food.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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
        public IActionResult Index(string uaction, int? id = null)
        {
            string query = "select * from tbl_Products where isdeleted = 0";

            MySqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            MySqlCommand command = new(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<ProductModel> productList = new();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel product = new()
                    {
                        ProductId = reader.GetInt32("ProductId"),
                        ProductName = reader.GetString("ProductName"),
                        Description = reader.GetString("Description"),
                        MRPrice = reader.GetDecimal("MRPrice"),
                        SalePrice = reader.GetDecimal("SalePrice"),
                        DiscountPercent = reader.GetDecimal("DiscountPercent"),
                        CreatedBy = reader.GetInt32("CreatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        Images = reader["Images"] == null ? Array.Empty<byte>() : (byte[])reader["Images"],
                        IsActive = reader.GetInt32("ProductId") == id
                    };
                    if (product.Images != null)
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
            };
            return View("Index", data);
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
            } else if (string.IsNullOrEmpty(product.ProductName))
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
                    query = "UPDATE `tbl_products` SET `ProductName`=@ProductName, `Description`=@Description, `Images`=@Images, `MRPrice`=@MRPrice, `SalePrice`=@SalePrice, `DiscountPercent`=@DiscountPercent, `CreatedBy`=@CreatedBy, `CreatedOn`=@CreatedOn,`IsDeleted`=@IsDeleted WHERE ProductId=" + product.ProductId.ToString();
                    response.Status = "success";
                    response.Message = "Updated Successfully";
                    response.Data = product;
                }
                else
                {
                    product.CreatedOn = DateTime.Now;
                    query = "INSERT INTO `tbl_products`(`ProductName`, `Description`, `Images`, `MRPrice`, `SalePrice`, `DiscountPercent`, `CreatedBy`, `CreatedOn`, `IsDeleted`) VALUES (@ProductName,@Description,@Images,@MRPrice,@SalePrice,@DiscountPercent,@CreatedBy,@CreatedOn,@IsDeleted)";
                    response.Status = "success";
                    response.Message = "Added Successfully";
                    response.Data = product;
                }
                MySqlConnection connection = new()
                {
                    ConnectionString = _config.GetConnectionString("DefaultConnection")
                };
                connection.Open();
                MySqlCommand command = new(query, connection);
                _ = command.Parameters.AddWithValue("@ProductName", product.ProductName);
                _ = command.Parameters.AddWithValue("@Description", product.Description);
                _ = command.Parameters.AddWithValue("@Images", product.Images);
                _ = command.Parameters.AddWithValue("@MRPrice", product.MRPrice);
                _ = command.Parameters.AddWithValue("@SalePrice", product.SalePrice);
                _ = command.Parameters.AddWithValue("@DiscountPercent", product.DiscountPercent);
                _ = command.Parameters.AddWithValue("@CreatedBy", product.CreatedBy);
                _ = command.Parameters.AddWithValue("@CreatedOn", product.CreatedOn);
                _ = command.Parameters.AddWithValue("@IsDeleted", product.IsDeleted);
                _ = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                response.Status = "error";
                response.Message = ex.Message + "_" +(ex.InnerException?.Message ?? "");
            }
            return Ok(response);
        }
    }
}
