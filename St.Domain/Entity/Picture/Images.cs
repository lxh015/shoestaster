using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.Picture
{
    public class Images : BaseID
    {
        public string Title { get; set;}

        public string Context { get; set; }

        public string Path { get; set; }
    }
}
