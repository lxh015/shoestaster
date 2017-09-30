using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
    public class ProductImagesMap :EntityTypeConfiguration<ProductImages>
    {
        public ProductImagesMap()
        {
            this.ToTable("Pro_ProductImages");

            this.HasOptional(p => p.Image)
                .WithMany()
                .Map(p => p.MapKey("IID"))
                .WillCascadeOnDelete(false);
        }
    }
}
