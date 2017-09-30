using St.Domain.Entity.Picture;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
    public class ImagesMap:EntityTypeConfiguration<Images>
    {
        public ImagesMap()
        {
            this.ToTable("Nor_Images");
        }
    }
}
