using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.AD
{
    /// <summary>
    /// 广告类
    /// </summary>
    public class Ads : BaseID, IAudit, IDate
    {
        public string LinkUrl { get; set; }

        public string Title { get; set; }

        public AdsArea Area { get; set; }

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

        public Picture.Images image { get; set; }

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

    public enum AdsArea
    {
        None = 1024,
        Top = 0,
        Low = 1,
        Central = 2,
        Left = 4,
        Right = 8,
        LowLeft = 16,
        LowRight = 32,
        TopLeft = 64,
        TopRight = 128,
        CentralLeft = 256,
        CentralRight = 512,
    }
}
