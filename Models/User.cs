using Microsoft.AspNetCore.Identity;

namespace StoreApi.Models
{
    public class User : IdentityUser<int>
    {
        public UserAddress? Address { get; set; }
    }
}
