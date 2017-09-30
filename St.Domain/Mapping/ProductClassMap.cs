using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
   public class ProductClassMap:EntityTypeConfiguration<ProductClass>
    {
        public ProductClassMap()
        {
            this.ToTable("Pro_ProductClass");

            this.HasMany(p => p.productClassIntroduction)
                .WithOptional(p => p.productClass)
                .Map(p => p.MapKey("PCID"))
                .WillCascadeOnDelete(true);
        }
    }
}
