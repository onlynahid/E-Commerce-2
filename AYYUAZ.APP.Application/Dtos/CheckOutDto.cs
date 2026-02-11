using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Application.Dtos
{
    public class CheckOutDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        [Required]
        public string Address { get; set; }
        public string? Notes { get; set; }
    }
}
