using Microsoft.AspNetCore.Identity;

namespace Diplom.Models
{
    public class ApplicationUser :IdentityUser
    {
        [PersonalData]
        public string? FullName { get; set; }
    }
}
