using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AYYUAZ.APP.Domain.Entities
{
    public class User : IdentityUser
    {
       
        // We'll determine admin status through Identity roles in AspNetUserRoles table
        [NotMapped]
        public bool IsAdmin { get; set; }
    }
}
