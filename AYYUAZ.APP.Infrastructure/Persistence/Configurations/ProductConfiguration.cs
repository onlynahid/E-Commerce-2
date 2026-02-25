using AYYUAZ.APP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
             builder
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder
              .HasOne(a => a.Discount)
              .WithOne(a => a.Product)
              .HasForeignKey<Product>(a => a.DiscountId);
        }
    }
    }
