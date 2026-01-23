using Microsoft.AspNetCore.Identity;

namespace ScholaAi.Models
{
    public class applicationUser:IdentityUser
    {
        public virtual user UserProfile { get; set; }
       
    }
}
