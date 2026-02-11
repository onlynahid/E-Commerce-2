using AYYUAZ.APP.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Entities
{
    public class Hero:BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public string? DiscountMessage { get; set; }
        public string OrderMessage { get; set; }

    }
}
