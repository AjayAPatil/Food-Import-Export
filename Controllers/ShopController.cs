using Food.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
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
            return View("Index", new { productList = productList, categoryList = categories, categoryId = category });
        }

        public ResponseModel GetUserProducts(int userid, string? productSavedAs = "")
        {
            ResponseModel response = new();
            if (userid < 0)
            {
                response.Status = "error";
                response.Message = "UserId not found";
                return response;
            }

            string query = "select p.ProductId, p.ProductName, p.Images, p.Description, p.MRPrice, p.SalePrice, up.SavedAs, up.CreatedBy, p.AvailableQuantity, p.AvailableQuantityUnit " +
                "from tbl_userproducts as up join tbl_products p on up.ProductId = p.ProductId " +
                "where up.IsDeleted = 0 and up.CreatedBy = @CreatedBy";

            if (!string.IsNullOrEmpty(productSavedAs))
            {
                query += " and up.SavedAs = @SavedAs";
            }

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@CreatedBy", userid);
            command.Parameters.AddWithValue("@SavedAs", productSavedAs);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                List<UserProductsModel> userProductList = new List<UserProductsModel>();
                while (reader.Read())
                {
                    var obj = new UserProductsModel()
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        SavedAs = Convert.ToString(reader["SavedAs"]) ?? "",
                        Product = new ProductModel
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            ProductName = Convert.ToString(reader["ProductName"]) ?? "",
                            Images = reader["Images"] == null || reader["Images"] == DBNull.Value ? Array.Empty<byte>() : (byte[])reader["Images"],
                            Description = Convert.ToString(reader["Description"]) ?? "",
                            MRPrice = Convert.ToDecimal(reader["MRPrice"]),
                            SalePrice = Convert.ToDecimal(reader["SalePrice"]),
                            AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                            AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        }
                    };
                    if (obj.Product.Images != null && obj.Product.Images?.Length > 0)
                    {
                        obj.Product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(obj.Product.Images);
                    }
                    userProductList.Add(obj);
                }
                reader.Close();

                response.Status = "success";
                response.Message = "Success";
                response.Data = userProductList;
                return response;
            }
            else
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

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@ProductId", userProducts.ProductId);
            command.Parameters.AddWithValue("@SavedAs", userProducts.SavedAs);
            command.Parameters.AddWithValue("@CreatedBy", userProducts.CreatedBy);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                List<UserProductsModel> userProductList = new List<UserProductsModel>();
                while (reader.Read())
                {
                    userProductList.Add(new UserProductsModel()
                    {
                        UserProductId = Convert.ToInt32(reader["UserProductId"]),
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        SavedAs = Convert.ToString(reader["SavedAs"]) ?? "",
                        IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
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
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        Images = reader["Images"] == null ? Array.Empty<byte>() : (byte[])reader["Images"]
                    };
                    if (product.Images != null)
                    {
                        product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(product.Images);
                    }
                    productList.Add(product);
                }
            }
            var currentProduct = productList.Where(a => a.ProductId == productId).FirstOrDefault();
            if (currentProduct == null)
            {
                return Index();
            }
            productList = productList.Where(a => a.CategoryId == currentProduct.CategoryId && a.ProductId != productId).ToList();
            return View("Product", new { productList = productList, currentProduct = currentProduct });
        }

        public IActionResult OrderProduct(string inputData = "")
        {
            if (string.IsNullOrEmpty(inputData))
            {
                return Index();
            }
            var order = JsonConvert.DeserializeObject<OrdersModel>(inputData);
            var currentProductList = order?.ProductOrders;
            if (currentProductList == null || currentProductList.Count <= 0)
            {
                return Index();
            }
            string query = "select * from tbl_Products where isdeleted = 0 and ProductId IN (" + string.Join(",", currentProductList.Select(a => a.ProductId)) + ")";

            SqlConnection connection = new()
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            };
            connection.Open();
            SqlCommand command = new(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            List<ProductOrdersModel> productList = new List<ProductOrdersModel>();
            decimal Total = 0M, Discount = 0M, CouponDiscount = 0M;
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
                        AvailableQuantity = reader["AvailableQuantity"] != DBNull.Value ? Convert.ToInt32(reader["AvailableQuantity"]) : null,
                        AvailableQuantityUnit = reader["AvailableQuantityUnit"] != DBNull.Value ? Convert.ToString(reader["AvailableQuantityUnit"]) : null,
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                    };
                    var currentProduct = currentProductList.Find(a => a.ProductId == product.ProductId) ?? new ProductOrdersModel();
                    productList.Add(new ProductOrdersModel
                    {
                        ProductId = product.ProductId,
                        Quantity = currentProduct.Quantity,
                        QuantityUnit = product.AvailableQuantityUnit ?? "",
						MrpAmount = currentProduct.Quantity * product.MRPrice,
                        Amount = currentProduct.Quantity * product.SalePrice,
                        Product = product,
                    });
                    Discount = Discount + ((product.MRPrice - product.SalePrice) * currentProduct.Quantity);
					Total += (product.SalePrice * currentProduct.Quantity);
				}

				if (order?.CouponId != null)
				{
                    order.Coupon = GetCoupon((int)order.CouponId);
                    if(order.Coupon != null)
					{
						CouponDiscount = Total * order.Coupon.CouponDiscount / 100;
						Total -= CouponDiscount;
					}
				}
			}

            if (productList.Count <= 0)
            {
                return Index();
            }
            return View("OrderProduct", new { productList, Discount, CouponDiscount, Total, order });
        }

		private MasterCouponModel GetCoupon(int couponId)
		{
			string query = "select * from tbl_MasterCoupon where isdeleted = 0 and CouponId=" + couponId.ToString();

			SqlConnection connection = new()
			{
				ConnectionString = _config.GetConnectionString("DefaultConnection")
			};
			connection.Open();
			SqlCommand command = new(query, connection);
			SqlDataReader reader = command.ExecuteReader();

            MasterCouponModel coupon = new();
			if (reader.HasRows)
			{
				while (reader.Read())
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
			reader.Close();
			connection.Close();

			return coupon;
		}

		public IActionResult Favourites()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(OrdersModel orders)
        {
            ResponseModel response = new();
            if (orders == null)
            {
                response.Status = "error";
                response.Message = "Data not found";
                return Ok(response);
            }

            try
            {
                string query = "select * from tbl_Orders where CreatedBy = @CreatedBy and OrderId = @OrderId";

                SqlConnection connection = new()
                {
                    ConnectionString = _config.GetConnectionString("DefaultConnection")
                };
                connection.Open();
                SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@OrderId", orders.OrderId);
                command.Parameters.AddWithValue("@CreatedBy", orders.CreatedBy);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();
                    connection.Close();
                    response.Status = "fail";
                    response.Message = "Order already placed!";
                    return Ok(response);
                }
                else
                {
                    reader.Close();
                    query = "declare @id TABLE (newKey INT)" +
                        "\r\ninsert into tbl_Orders([OrderDate],[PaymentMethod],[DeliveryDate],[DeliveryStatus],[MobileNo],[EmailId],[Address],[City],[PinCode],[CreatedBy],[CreatedOn],[IsDeleted]" 
                        + ((orders.CouponId != null) ? "),[CouponId]": "") + ")" +
                        " OUTPUT Inserted.OrderId into @id " +
                        "values (@OOrderDate,@OPaymentMethod,@ODeliveryDate,@ODeliveryStatus,@OMobileNo, @OEmailId, @OAddress, @OCity, @OPinCode, @CreatedBy, @CreatedOn, @IsDeleted" 
                        + ((orders.CouponId != null) ? "),@CouponId" : "") + ")" +
                        "\r\n";
                        if (orders.ProductOrders?.Count > 0)
                    {
                        for (int p = 0; p < orders.ProductOrders.Count; p++)
                        {
                            var currP = new
                            {
                                orders.ProductOrders[p].ProductId,
                                orders.ProductOrders[p].Quantity,
                                orders.ProductOrders[p].QuantityUnit,
                                orders.ProductOrders[p].Amount,
                                orders.ProductOrders[p].CreatedBy,
                                orders.ProductOrders[p].CreatedOn,
                                IsDeleted = orders.ProductOrders[p].IsDeleted ? "1" : "0"
                            };
                            query += "\r\nINSERT INTO [dbo].[tbl_ProductOrders] ([OrderId],[ProductId],[Quantity],[QuantityUnit],[Amount],[CreatedBy],[CreatedOn],[IsDeleted])" +
                            "\r\nVALUES ((SELECT TOP 1 newKey FROM @id), " + currP.ProductId + ", " + currP.Quantity + ", '" + currP.QuantityUnit + "', " + currP.Amount + ", @CreatedBy, @CreatedOn, @IsDeleted)";
                        }
                    }
                    else
                    {
                        reader.Close();
                        connection.Close();
                        response.Status = "fail";
                        response.Message = "Items not added..";
                        return Ok(response);
                    }
                    command = new(query, connection);
                    command.Parameters.AddWithValue("@OOrderDate", orders.OrderDate);
                    command.Parameters.AddWithValue("@OPaymentMethod", orders.PaymentMethod);
                    command.Parameters.AddWithValue("@ODeliveryDate", orders.DeliveryDate);
                    command.Parameters.AddWithValue("@ODeliveryStatus", orders.DeliveryStatus);
                    command.Parameters.AddWithValue("@OMobileNo", orders.MobileNo);
                    command.Parameters.AddWithValue("@OEmailId", orders.EmailId);
                    command.Parameters.AddWithValue("@OAddress", orders.Address);
                    command.Parameters.AddWithValue("@OCity", orders.City);
                    command.Parameters.AddWithValue("@OPinCode", orders.PinCode);
                    command.Parameters.AddWithValue("@CreatedBy", orders.CreatedBy);
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    command.Parameters.AddWithValue("@IsDeleted", orders.IsDeleted);
                    if(orders.CouponId != null)
                    command.Parameters.AddWithValue("@CouponId", orders.CouponId);
                    command.ExecuteNonQuery();
                }

                response.Status = "success";
                response.Message = "Order placed.";
                response.Data = orders;
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult UserOrders()
        {
            return View();
        }
        public IActionResult GetUserOrders(int userid)
		{
			UpdateOrderStatus();

			ResponseModel response = new();
			if (userid < 0)
			{
				response.Status = "error";
				response.Message = "UserId not found";
				return Ok(response);
			}

			string query = "select o.OrderId, o.OrderDate, o.DeliveryDate, o.DeliveryStatus, o.MobileNo, o.Address, o.City, o.PinCode, o.CreatedBy, o.CreatedOn," +
				"\r\n p.ProductName, p.Images, p.SalePrice, p.MRPrice, p.DiscountPercent, p.ProductId, " +
                "\r\n po.Quantity, po.QuantityUnit, po.Amount" +
                "\r\n from tbl_Orders o" +
                "\r\n join tbl_ProductOrders po on o.OrderId = po.OrderId" +
				"\r\n join tbl_Products p on po.ProductId = p.ProductId" +
				"\r\n where ISNULL(o.IsDeleted, 0) = 0 and  ISNULL(po.IsDeleted, 0) = 0 and  ISNULL(p.IsDeleted, 0) = 0" +
				"\r\n and o.CreatedBy = @CreatedBy" +
				"\r\n order by o.CreatedOn desc";

			SqlConnection connection = new()
			{
				ConnectionString = _config.GetConnectionString("DefaultConnection")
			};
			connection.Open();
			SqlCommand command = new(query, connection);
			command.Parameters.AddWithValue("@CreatedBy", userid);
			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				List<OrdersModel> orders = new List<OrdersModel>();
				while (reader.Read())
				{
                    var orderedProducts = new ProductOrdersModel()
					{
						OrderId = Convert.ToInt32(reader["OrderId"]),
						Quantity = Convert.ToInt32(reader["Quantity"]),
						QuantityUnit = Convert.ToString(reader["QuantityUnit"]) ?? "",
						Amount = Convert.ToDecimal(reader["Amount"]),
						CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
						CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        Product = new ProductModel()
						{
							ProductId = Convert.ToInt32(reader["ProductId"]),
							ProductName = Convert.ToString(reader["ProductName"]) ?? "",
							Images = reader["Images"] == null || reader["Images"] == DBNull.Value ? Array.Empty<byte>() : (byte[])reader["Images"],
							SalePrice = Convert.ToDecimal(reader["SalePrice"]),
							MRPrice = Convert.ToDecimal(reader["MRPrice"]),
							DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
						}
					};
					if (orderedProducts.Product.Images != null && orderedProducts.Product.Images?.Length > 0)
					{
						orderedProducts.Product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(orderedProducts.Product.Images);
					}


					var ordIdx = orders.FindIndex(o => o.OrderId == Convert.ToInt32(reader["OrderId"]));
                    var obj = new OrdersModel();
                    if(ordIdx < 0)
                    {
                        //no existing order
                        orders.Add(new OrdersModel
                        {
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            OrderDate = reader["OrderDate"] == null || reader["OrderDate"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(reader["OrderDate"]),
                            DeliveryDate = reader["DeliveryDate"] == null || reader["DeliveryDate"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(reader["DeliveryDate"]),
                            DeliveryStatus = Convert.ToString(reader["DeliveryStatus"]),
                            MobileNo = Convert.ToString(reader["MobileNo"]),
                            Address = Convert.ToString(reader["Address"]),
                            City = Convert.ToString(reader["City"]),
                            PinCode = Convert.ToString(reader["PinCode"]),
                            CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            Amount = orderedProducts.Amount,
                            ProductOrders = new List<ProductOrdersModel>() { orderedProducts }
                        });
					} else
                    {
						//order already added but product not added
						orders[ordIdx].Amount += orderedProducts.Amount;
						orders[ordIdx].ProductOrders?.Add(orderedProducts);

					}
				}
				reader.Close();

				response.Status = "success";
				response.Message = "Success";
				response.Data = orders;
			}
			else
			{
				response.Status = "error";
				response.Message = "Orders not found";
			}
			return Ok(response);
        }

        public void UpdateOrderStatus()
		{
            try
            {
                string query = "update tbl_Orders set DeliveryStatus = 'Delivered' where DeliveryDate <= GETDATE()";

                SqlConnection connection = new()
                {
                    ConnectionString = _config.GetConnectionString("DefaultConnection")
                };
                connection.Open();
                SqlCommand command = new(query, connection);
                _ = command.ExecuteNonQuery();
            }catch (Exception ex) { }
		}
    }
}
