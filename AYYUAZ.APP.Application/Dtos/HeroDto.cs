using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Application.Dtos
{
    public class HeroDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public string? DiscountMessage { get; set; }
        public string OrderMessage { get; set; }
    }
    public class CreateHeroDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public IFormFile? ImageUrl { get; set; }
        
        [StringLength(100)]
        public string? DiscountMessage { get; set; }
        
        [Required]
        [StringLength(200)]
        public string OrderMessage { get; set; }
    }
    public class UpdateHeroDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }    
        public IFormFile? ImageUrl { get; set; } 
        [StringLength(100)]
        public string? DiscountMessage { get; set; }
        [Required]
        [StringLength(200)]
        public string OrderMessage { get; set; }
    }

}
