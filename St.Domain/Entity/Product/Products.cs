using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.Product
{
    public class Products : BaseID, IAudit,IDate
    {
        public string Name { get; set; }

        public double? minPrice { get; set; }

        public double maxPrice { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public string Context { get; set; }

        public bool isShow { get; set; }

        public ProductClass productClass { get; set; }

        public List<ProductImages> productImages { get; set; }

        /// <summary>
        /// 类别描述。只有在选择类别后才能选择类别描述下的信息。
        /// </summary>
        public string ClassIntroduction { get; set; }

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
