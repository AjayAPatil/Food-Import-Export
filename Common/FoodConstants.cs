using Food.Models;

namespace Food.Common
{
    public static class FoodConstants
    {
        public static readonly object Role = new
        {
            SuperAdmin = "SuperAdmin",
            Admin = "Admin",
            Customer = "Customer"
        };
        public static readonly List<KeyValueModel> AvailableQuantityUnitList = new List<KeyValueModel>
        {
            new KeyValueModel{ Key = "Ton", Value = "Tons" },
            new KeyValueModel{ Key = "KG", Value = "Kilograms" },
            new KeyValueModel{ Key = "GM", Value = "Grams" },
            new KeyValueModel{ Key = "UNIT", Value = "Units" },
        };
    }
}
