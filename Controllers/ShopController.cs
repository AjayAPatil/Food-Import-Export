using Food.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Food.Controllers
{
    public class ShopController : Controller
    {
        private readonly IConfiguration _config;
        public ShopController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(int category = 0)
        {
            string query = "select * from tbl_Products where isdeleted = 0";

            if (category != 0)
            {
                query += " and CategoryId = " + category.ToString();
            }

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
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = reader.GetInt32("CreatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        Images = reader["Images"] == null ? Array.Empty<byte>() : (byte[])reader["Images"]
                    };
                    if (product.Images != null)
                    {
                        product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(product.Images);
                    }
                    productList.Add(product);
                }
            }
            var categories = new AdminController(_config).GetCategories();
            return View("Index", new { productList = productList, categoryList = categories, categoryId  = category });
        }

        public ResponseModel GetUserProducts(int userid)
        {
            ResponseModel response = new();
            if (userid < 0)
            {
                response.Status = "error";
                response.Message = "UserId not found";
                return response;
            }

            string query = "select * from tbl_userproducts where CreatedBy = @CreatedBy and IsDeleted = 0";

            MySqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@CreatedBy", userid);
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                List<UserProductsModel> userProductList = new List<UserProductsModel>();
                while (reader.Read())
                {
                    userProductList.Add(new UserProductsModel()
                    {
                        UserProductId = reader.GetInt32("UserProductId"),
                        ProductId = reader.GetInt32("ProductId"),
                        CreatedBy = reader.GetInt32("CreatedBy"),
                        SavedAs = reader.GetString("SavedAs"),
                        IsDeleted = reader.GetBoolean("IsDeleted"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    });
                }
                reader.Close();

                response.Status = "success";
                response.Message = "Success";
                response.Data = userProductList;
                return response;
            } else
            {
                response.Status = "error";
                response.Message = "Products not found";
                return response;
            }
        }

        public ResponseModel AddToFavourites(UserProductsModel userProducts)
        {
            ResponseModel response = new();
            if (userProducts == null)
            {
                response.Status = "error";
                response.Message = "Data not found";
                return response;
            }
            else if (userProducts.ProductId == 0)
            {
                response.Status = "error";
                response.Message = "Product not found.";
                return response;
            }
            else if (string.IsNullOrEmpty(userProducts.SavedAs))
            {
                response.Status = "error";
                response.Message = "SavedAs not found";
                return response;
            }
            else if (userProducts.CreatedBy == 0)
            {
                response.Status = "error";
                response.Message = "CreatedBy not found";
                return response;
            }
            string query = "select * from tbl_userproducts where CreatedBy = @CreatedBy and SavedAs = @SavedAs and ProductId = @ProductId";

            MySqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ProductId", userProducts.ProductId);
            command.Parameters.AddWithValue("@SavedAs", userProducts.SavedAs);
            command.Parameters.AddWithValue("@CreatedBy", userProducts.CreatedBy);
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                List< UserProductsModel >  userProductList = new List< UserProductsModel >();
                while (reader.Read())
                {
                    userProductList.Add(new UserProductsModel()
                    {
                        UserProductId = reader.GetInt32("UserProductId"),
                        ProductId = reader.GetInt32("ProductId"),
                        CreatedBy = reader.GetInt32("CreatedBy"),
                        SavedAs = reader.GetString("SavedAs"),
                        IsDeleted = reader.GetBoolean("IsDeleted"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    });
                }
                reader.Close();
                foreach (var item in userProductList)
                {
                    query = "update tbl_userproducts set IsDeleted = @IsDeleted, CreatedOn = @CreatedOn where UserProductId = @UserProductId";
                    command = new(query, connection);
                    item.IsDeleted = !item.IsDeleted;
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    command.Parameters.AddWithValue("@UserProductId", item.UserProductId);
                    command.Parameters.AddWithValue("@IsDeleted", item.IsDeleted);
                    response.Message = (item.IsDeleted ? "Removed from " : "Added to ") + item.SavedAs;
                    response.Data = item;
                    command.ExecuteNonQuery();
                }
                response.Status = "success";
                return response;
            }
            else
            {
                reader.Close();
                query = "insert into tbl_userproducts (ProductId, SavedAs, CreatedBy, CreatedOn, IsDeleted) values (@ProductId, @SavedAs, @CreatedBy, @CreatedOn, @IsDeleted)";
                command = new(query, connection);
                command.Parameters.AddWithValue("@ProductId", userProducts.ProductId);
                command.Parameters.AddWithValue("@SavedAs", userProducts.SavedAs);
                command.Parameters.AddWithValue("@CreatedBy", userProducts.CreatedBy);
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@IsDeleted", userProducts.IsDeleted);
                command.ExecuteNonQuery();

                response.Status = "success";
                response.Message = "Added to " + userProducts.SavedAs;
                return response;
            }
        }

        public IActionResult Product(int productId)
        {
            if (productId == 0)
            {
                return Index();
            }
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
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = reader.GetInt32("CreatedBy"),
						CreatedOn = reader.GetDateTime("CreatedOn"),
						Images = reader["Images"] == null ? Array.Empty<byte>() : (byte[])reader["Images"]
					};
					if (product.Images != null)
					{
						product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(product.Images);
					}
					productList.Add(product);
				}
			}
			var currentProduct = productList.Where(a=> a.ProductId == productId).FirstOrDefault();
            if (currentProduct == null)
            {
				return Index();
			}
			productList = productList.Where(a=> a.CategoryId == currentProduct.CategoryId && a.ProductId != productId).ToList();
			return View("Product", new { productList = productList, currentProduct = currentProduct });
        }

        public IActionResult OrderProduct(string products = "")
        {
            if (string.IsNullOrEmpty(products))
            {
                return Index();
            }
            var currentProduct = JsonConvert.DeserializeObject<List<ProductOrdersModel>>(products);
            if(currentProduct == null || currentProduct.Count <= 0)
            {
                return Index();
            }
            string query = "select * from tbl_Products where isdeleted = 0 and ProductId IN (" + string.Join(",", currentProduct.Select(a=> a.ProductId)) + ")";

            MySqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            MySqlCommand command = new(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            List<ProductModel> productList = new List<ProductModel>();
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
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = reader.GetInt32("CreatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        Images = reader["Images"] == null ? Array.Empty<byte>() : (byte[])reader["Images"]
                    };
                    if (product.Images != null)
                    {
                        product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(product.Images);
                    }
                    productList.Add(product);
                }
            }

            if (productList.Count <= 0)
            {
                return Index();
            }
            return View("OrderProduct", new { productList });
        }
    }
}
