using System.ComponentModel.DataAnnotations.Schema;

namespace Food.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsActive { get; set; }
    }
}
