using St.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    public class WebContextInfo
    {
        /// <summary>
        /// 基础返回结果
        /// </summary>
        public class BaseJsonResult
        {
            public BaseJsonResult()
            {

            }

            public void SetResult(bool issuccess, string message)
            {
                this.isSuccess = issuccess;
                this.returnMessage = message;
            }

            /// <summary>
            /// 操作是否成功
            /// </summary>
            public bool isSuccess { get; set; }
            /// <summary>
            /// 返回结果消息
            /// </summary>
            public string returnMessage { get; set; }
        }

        /// <summary>
        /// 权限类
        /// </summary>
        public class WebSet
        {
            /// <summary>
            /// 初始化一个最基础控制权限
            /// </summary>
            public WebSet()
            {
                this.AdsLevel = LevelInfo.区域管理;
                this.ImagesLevel = LevelInfo.区域管理;
                this.NewsLevel = LevelInfo.区域管理;
                this.ProductsLevel = LevelInfo.区域管理;
                this.SettingLevel = LevelInfo.区域管理;
            }

            public WebSet(string context) : this()
            {
                if (string.IsNullOrEmpty(context))
                    return;
                else
                {
                    string[] cSplit = context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var properties = this.GetType().GetProperties();

                    for (int i = 0; i < cSplit.Length; i++)
                    {
                        var info = cSplit[i];
                        string[] infoSplit = info.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in properties)
                        {
                            if (info.Contains(item.Name))
                            {
                                var level = (LevelInfo)Convert.ToInt32(infoSplit[1]);
                                item.SetValue(this, level);
                                break;
                            }
                        }
                    }
                }
            }

            public LevelInfo ProductsLevel { get; set; }

            public LevelInfo NewsLevel { get; set; }

            public LevelInfo AdsLevel { get; set; }

            public LevelInfo ImagesLevel { get; set; }

            public LevelInfo SettingLevel { get; set; }
        }

        public class BaseListResult<T> where T : BaseID, new()
        {
            public void SetError()
            {
                this.Result = false;
                this.Data = null;
            }
            public void SetData(List<T> data)
            {
                this.Result = true;
                this.Data = data;
            }

            /// <summary>
            /// 总页数
            /// </summary>
            public int DataSumCount { get; set; }

            /// <summary>
            /// 结果信息
            /// </summary>
            public bool Result { get; set; }

            /// <summary>
            /// 结果数据组
            /// </summary>
            public List<T> Data { get; set; }
        }
    }
}
