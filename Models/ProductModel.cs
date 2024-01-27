using System.ComponentModel.DataAnnotations.Schema;

namespace Food.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Images { get; set; }
        public decimal MRPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsActive { get; set; }
    }
}
