using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.Product
{
    public class ProductClassIntroduction : BaseID
    {
        public ProductClass productClass { get; set; }

        public string Description { get; set; }
    }
}
