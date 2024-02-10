namespace Food.Models
{
    public class UserProductsModel
    {
        public int UserProductId { get; set; }
        public int ProductId { get; set; }
        public string SavedAs { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
