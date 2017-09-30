using St.Domain.Entity.SuperUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Service
{
    public interface ISUserInterface : IServiceBase<SUser>,IQueryForPage<SUser>
    {
        SUser UserLogin(string name,string password);
    }
}
