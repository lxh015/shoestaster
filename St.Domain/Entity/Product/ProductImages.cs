using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.Product
{
    public class ProductImages : BaseID
    {
        public Products product { get; set; }

        public string Name { get; set; }

        public string Alink { get; set; }

        public Picture.Images Image { get; set; }
    }
}
