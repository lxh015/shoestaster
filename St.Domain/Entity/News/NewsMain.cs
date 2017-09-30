using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.News
{
    public class NewsMain : BaseID, IAudit
    {
        private AuditState _stata { get; set; }
        public AuditState Stata
        {
            get
            {
                return this._stata;
            }

            set
            {
                this._stata = value;
            }
        }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Context { get; set; }

        public bool IsShow { get; set; }

        public NewsShow newShow { get; set; }
    }
}
