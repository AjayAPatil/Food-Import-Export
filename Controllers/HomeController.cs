using Food.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;

namespace Food.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        private void CreateDBIfNotExists()
        {
            //get one time script
            string scriptPath = Directory.GetCurrentDirectory() + "\\DBScripts\\OneTimeScript.sql";
            if (System.IO.File.Exists(scriptPath))
            {
                // Read entire text file content in one string
                string query = System.IO.File.ReadAllText(scriptPath);
                SqlConnection connection = new()
                {
                    ConnectionString = _config.GetConnectionString("DefaultConnection")
                };
                connection.Open();
                SqlCommand command = new(query, connection);
                _ = command.ExecuteNonQuery();
            }
        }

        public IActionResult Index()
        {
            CreateDBIfNotExists();

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
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        Images = reader["Images"] == null || reader["Images"] == DBNull.Value ? Array.Empty<byte>() : (byte[])reader["Images"]
                    };
                    if (product.Images != null)
                    {
                        product.ImagesB64 = "data:image/jpg;base64," + Convert.ToBase64String(product.Images);
                    }
                    productList.Add(product);
                }
            }

            var categories = new AdminController(_config).GetCategories();
            var fruit = categories.Find(ct => ct.CategoryName.ToLower().Contains("fruit"));
            var juice = categories.Find(ct => ct.CategoryName.ToLower().Contains("juice"));
            var vegetable = categories.Find(ct => ct.CategoryName.ToLower().Contains("vegetable"));
            var dried = categories.Find(ct => ct.CategoryName.ToLower().Contains("dried"));

            return View("Index", new { productList, categories, fruit, juice, vegetable, dried, Reviews = GetCustomerReviews() });
        }

        public IActionResult About()
        {
            return View(new
            {
                Reviews = GetCustomerReviews()
            });
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public List<CustomerModel> GetCustomerReviews()
        {
            return new()
            {
                new CustomerModel
                {
                    CustomerId = 1,
                    CustomerName = "John H.",
                    IsActive = true,
                    RatingGiven = 5,
                    Remark = "I had an amazing experience with this company! The customer service was top-notch, and the product exceeded my expectations. I highly recommend them to anyone looking for quality products and excellent service."
                },
                new CustomerModel
                {
                    CustomerId = 2,
                    CustomerName = "Sarah L.",
                    IsActive = true,
                    RatingGiven = 5,
                    Remark = "I am a repeat customer of this business, and they never disappoint. The team is always friendly and helpful, and their products are outstanding. I’ve recommended them to all my friends and family, and I will continue to do so!"
                },
                new CustomerModel
                {
                    CustomerId = 3,
                    CustomerName = "David W.",
                    IsActive = true,
                    RatingGiven = 5,
                    Remark = "After trying several other companies, I finally found the perfect fit with this one. Their attention to detail and commitment to customer satisfaction is unparalleled. I will definitely be using their services again in the future."
                },
                new CustomerModel
                {
                    CustomerId = 4,
                    CustomerName = "Emily R.",
                    IsActive = true,
                    RatingGiven = 5,
                    Remark = "I can’t say enough good things about this business. From the moment I contacted them, they were responsive and accommodating. The quality of their work is exceptional, and I couldn’t be happier with the final result."
                },
                new CustomerModel
                {
                    CustomerId = 5,
                    CustomerName = "Mark T.",
                    IsActive = true,
                    RatingGiven = 5,
                    Remark = "I’ve been a loyal customer for years, and I continue to be impressed with this company. Their team is dedicated to providing the best service possible, and it shows in every interaction. I highly recommend them to anyone in need of their services."
                }
            };
        }
    }
}