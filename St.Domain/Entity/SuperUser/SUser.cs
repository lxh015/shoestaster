using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.SuperUser
{
    public class SUser : BaseID, IAudit, UserLevel,IDate
    {
        public string Name { get; set; }

        public string NickName { get; set; }

        public string PassWord { get; set; }

        public bool isUse { get; set; }

        private LevelInfo _level { get; set; }
        public LevelInfo Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        private AuditState _stata { get; set; }
        public AuditState Stata
        {
            get
            {
                return _stata;
            }

            set
            {
                _stata = value;
            }
        }

        private DateTime _addDateTime { get; set; }

        public DateTime AddDateTime
        {
            get
            {
                return this._addDateTime;
            }

            set
            {
                this._addDateTime = value;
            }
        }

        private DateTime _updateTime { get; set; }
        public DateTime UpdateTime
        {
            get
            {
                return this._updateTime;
            }

            set
            {
                this._updateTime = value;
            }
        }
    }
}
