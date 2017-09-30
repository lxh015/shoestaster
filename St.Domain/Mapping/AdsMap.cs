using St.Domain.Entity.AD;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
    public class AdsMap:EntityTypeConfiguration<Ads>
    {
        public AdsMap()
        {
            this.ToTable("Nor_Ads");

            this.HasOptional(p => p.image)
                 .WithMany()
                 .Map(p => p.MapKey("ImageID"))
                 .WillCascadeOnDelete(false);

            this.Property(p => p.LinkUrl)
                .IsOptional();
        }
    }
}
