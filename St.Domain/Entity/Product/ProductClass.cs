using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.Product
{
    public class ProductClass : BaseID, IAudit
    {
        public string Name { get; set; }

        public bool isShow { get; set; }

        public IList<ProductClassIntroduction> productClassIntroduction { get; set; }

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
    }
}
