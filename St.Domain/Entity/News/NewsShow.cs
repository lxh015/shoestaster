using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain.Entity.News
{
    /// <summary>
    /// 新闻展示，用于主页简单信息展示
    /// </summary>
    public class NewsShow : BaseID
    {
        public NewsMain newsMain { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图片列表
        /// </summary>
        public Picture.Images images { get; set; }
    }
}
