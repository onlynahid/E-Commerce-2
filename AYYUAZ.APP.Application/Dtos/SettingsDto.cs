using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Dtos
{
    public class SettingsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
    }

    public class CreateSettingsDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }      
        [Phone]
        public string? PhoneNumber { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        public string? Email { get; set; }
        [Url]
        public string? FacebookUrl { get; set; }
        [Url]
        public string? InstagramUrl { get; set; }
        [Url]
        public string? TwitterUrl { get; set; }
    }

    public class UpdateSettingsDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }  
        [Required]
        [StringLength(500)]
        public string Address { get; set; } 
        [EmailAddress]
        public string? Email { get; set; } 
        [Url]
        public string? FacebookUrl { get; set; }  
        [Url]
        public string? InstagramUrl { get; set; }
        [Url]
        public string? TwitterUrl { get; set; }
    }
}