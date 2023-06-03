using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class ApplicationUser :IdentityUser
    {
        [PersonalData]
        [Display(Name ="ФИО")]
        public string? FullName { get; set; }
    }
}
