using St.Domain.Entity.News;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
    public class NewsMainMap:EntityTypeConfiguration<NewsMain>
    {
        public NewsMainMap()
        {
            this.ToTable("Nor_NewsMain");

            this.HasOptional(p => p.newShow)
                .WithOptionalPrincipal(p => p.newsMain)
                .Map(p => p.MapKey("MainID"))
                .WillCascadeOnDelete(true);
        }
    }
}
