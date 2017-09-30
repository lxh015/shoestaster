using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity
{
    public interface UserLevel
    {
        LevelInfo Level { get; set; }
    }

    public enum LevelInfo
    {
        超级管理员 = 0,
        管理员 = 1,
        区域管理 = 2,
        普通 = 4,
        游客 = 16,
    }
}
