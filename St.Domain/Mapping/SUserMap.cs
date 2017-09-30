using St.Domain.Entity.SuperUser;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Mapping
{
   public class SUserMap:EntityTypeConfiguration<SUser>
    {
        public SUserMap()
        {
            this.ToTable("Sys_SUser");

            this.Property(p => p.PassWord)
                .IsRequired();
        }
    }
}
