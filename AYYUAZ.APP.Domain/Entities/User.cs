using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AYYUAZ.APP.Domain.Entities
{
    public class User : IdentityUser
    {
        [NotMapped]
        public bool IsAdmin { get; set; }
    }
}
