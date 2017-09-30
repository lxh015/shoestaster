using St.Domain.Entity.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IProductImagesInterface : IServiceBase<ProductImages>,IInclude<ProductImages>
    {
        List<ProductImages> GetProductImagesIncludeImage(string[] idArray);

        void DeleteImage(int pid, int piid);
    }
}
