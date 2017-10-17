using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity
{
   public interface IDate
    {
        DateTime AddDateTime { get; set; }

        DateTime UpdateTime { get; set; }
    }
}
