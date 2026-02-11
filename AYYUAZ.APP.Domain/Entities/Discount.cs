using AYYUAZ.APP.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AYYUAZ.APP.Domain.Entities
{
    public class Discount : BaseEntity
    {
        public decimal? Percentage { get; set; }
        public Product Product { get; set; }
    }
}