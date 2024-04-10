namespace Food.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int RatingGiven { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}
