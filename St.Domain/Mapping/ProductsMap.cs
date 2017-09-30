using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
    public class ProductsMap : EntityTypeConfiguration<Products>
    {
        public ProductsMap()
        {
            this.ToTable("Pro_Products");

            this.HasRequired(p => p.productClass)
                .WithMany()
                .Map(p => p.MapKey("PCID"))
                .WillCascadeOnDelete(false);

            this.HasMany(p => p.productImages)
                .WithOptional(p => p.product)
                .Map(p => p.MapKey("PID"))
                .WillCascadeOnDelete(true);
        }
    }
}
