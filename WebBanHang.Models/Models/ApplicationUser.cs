using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebBanHang.Models.Models;

namespace WebBanHang.DAL.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FulName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? BirthDay { get; set; }
        //public string? PhoneNumber { get; set; }
        //public string? Role { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        
    }
}
