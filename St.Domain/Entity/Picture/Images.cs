using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.Picture
{
    public class Images : BaseID,IDate
    {
        public string Title { get; set;}

        public string Context { get; set; }

        public string Path { get; set; }

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
