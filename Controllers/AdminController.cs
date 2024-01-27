using Food.Models;
using Microsoft.AspNetCore.Mvc;

namespace Food.Controllers
{
    public class AdminController : Controller
    {
        private readonly FoodDBContext _dbContext;

        public AdminController(FoodDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        [HttpGet]
        public IActionResult Index(string uaction, int? id = null)
        {
            List<ProductModel> productList = _dbContext.Products.Where(a => a.IsDeleted == false).ToList();
            if (id != null)
            {
                productList.ForEach(a =>
                {
                    a.IsActive = a.ProductId == id;
                });
            }
            var data = new
            {
                Products = productList,
                CurrentProduct = uaction == "new" ? new ProductModel() : productList.Where(a => a.IsActive).FirstOrDefault(),
                Action = uaction
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
            }
            else if (product.ProductId > 0)
            {
                ProductModel? prod = _dbContext.Products.Where(a => a.ProductId == product.ProductId).FirstOrDefault();
                if (prod != null)
                {
                    prod.ProductName = product.ProductName;
                    prod.SalePrice = product.SalePrice;
                    prod.MRPrice = product.MRPrice;
                    prod.Description = product.Description;

                    _ = _dbContext.Products.Update(prod);
                    _ = _dbContext.SaveChanges();
                    response.Status = "success";
                    response.Message = "Updated Successfully";
                    response.Data = prod;
                }
                else
                {
                    response.Status = "error";
                    response.Message = "Product Not Found!";
                }
            }
            else
            {
                product.CreatedOn = DateTime.Now;
                _ = _dbContext.Products.Add(product);
                _ = _dbContext.SaveChanges();
                response.Status = "success";
                response.Message = "Added Successfully";
                response.Data = product;
            }
            return Ok(response);
        }
    }
}
