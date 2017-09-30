using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity
{
    public interface IAudit
    {
        AuditState Stata { get; set; }
    }

    public enum AuditState
    {
        待审核 = 0,
        审核成功 = 1,
        审核失败 = 2,
    }
}
