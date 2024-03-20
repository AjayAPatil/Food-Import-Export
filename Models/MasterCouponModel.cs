namespace Food.Models
{
    public class MasterCouponModel
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public decimal CouponDiscount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
