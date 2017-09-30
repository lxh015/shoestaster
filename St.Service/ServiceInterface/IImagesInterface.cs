using St.Domain.Entity.Picture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface IImagesInterface : IServiceBase<Images>,IQueryForPage<Images>
    {
    }
}
