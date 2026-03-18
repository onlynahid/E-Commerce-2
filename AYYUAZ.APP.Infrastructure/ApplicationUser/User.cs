using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AYYUAZ.APP.Infrastructure.ApplicationUser
{
    public class User : IdentityUser
    {
        [NotMapped]
        public bool IsAdmin { get; set; }
    }
}