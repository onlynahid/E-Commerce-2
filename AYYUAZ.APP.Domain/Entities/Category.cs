using AYYUAZ.APP.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Domain.Entities
{
    public class Category:BaseEntity
    {
        [Required]
        public string Name { get; set; }   
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public DateTime CreatedAt { get; set; }
        public Category()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
