using AYYUAZ.APP.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public string? Material { get; set; }
        public string? AgeGroup { get; set; }
        public string? Size { get; set; }
        public string? Colors { get; set; }
        public Discount? Discount { get; set; }
        public int? DiscountId { get; set; }
        //public decimal? DiscountPercentage { get; set; }
        //public decimal FinalPrice=>DiscountPercentage.HasValue?Price-(Price*DiscountPercentage.Value / 100m ):Price;
        //public Discount? Discount { get; set; }
        //public int? discountid { get; set; }
        //public decimal FinalPrice
        //{
        //    get
        //    {
        //        if (Discount != null && Discount.Percentage.HasValue)
        //        {
        //            var discountamount = (Price * Discount.Percentage.Value) / 100;
        //            return Price - discountamount;
        //        }
        //        return Price;
        //    }
        //}
        public Product()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
