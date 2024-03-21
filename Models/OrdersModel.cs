namespace Food.Models
{
    public class OrdersModel
    {
        public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public string? PaymentMethod { get; set; }
		public DateTime DeliveryDate { get; set; }
		public string? DeliveryStatus { get; set; }
        public int? CouponId { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PinCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
		public decimal Amount { get; set; }
		public List<ProductOrdersModel>? ProductOrders { get; set; }
        public MasterCouponModel? Coupon { get; set; }
    }
    public class ProductOrdersModel
    {
        public int ProductOrderId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public decimal MrpAmount { get; set; }
        public decimal Amount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public ProductModel? Product { get; set; }
    }
}
