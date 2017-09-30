using St.Domain.Entity.News;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
    public class NewsShowMap : EntityTypeConfiguration<NewsShow>
    {
        public NewsShowMap()
        {
            this.ToTable("Nor_NewsShow");

            this.HasOptional(p => p.images)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}
