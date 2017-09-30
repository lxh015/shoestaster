using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
   public class ProductClassIntroductionMap:EntityTypeConfiguration<ProductClassIntroduction>
    {
        public ProductClassIntroductionMap()
        {
            this.ToTable("Pro_ProdctClassIntroduction");

        }
    }
}
