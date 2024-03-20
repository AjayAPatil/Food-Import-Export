using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Food.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PinCode { get; set; }
        public bool IsUpdate { get; set; }
    }
}
