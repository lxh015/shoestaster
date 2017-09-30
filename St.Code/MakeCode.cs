using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace St.Code
{
    /// <summary>
    /// 生成验证码图片类
    /// </summary>
    public class MakeCode
    {
        public MakeCode(int length = 4) : base()
        {
            this.Length = length;
        }

        private int Length { get; set; }

        /// <summary>
        /// 记录当前生成的验证码信息
        /// </summary>
        public string vcodeLog { get; private set; }

        /// <summary>
        /// 随机数
        /// </summary>
        private static Random random = new Random();

        #region 生成图片
        public void getImageValidate(CodeType cType)
        {
            string strCode = getCodeString(cType, this.Length);
            this.vcodeLog = strCode;
            //颜色列表，用于验证码，噪线，噪点
            Color[] color = { Color.FromArgb(200, 191, 231), Color.FromArgb(239, 228, 176), Color.FromArgb(239, 228, 176), Color.FromArgb(149, 223, 255), Color.FromArgb(169, 243, 160) };
            int width = 0;
            if (cType == CodeType.chinese)
            {
                width = Convert.ToInt32(strCode.Length * 26);    //计算图像宽度
            }
            else
            {
                width = Convert.ToInt32(strCode.Length * 17);    //计算图像宽度
            }

            Bitmap img = new Bitmap(width, 30);
            Graphics gfc = Graphics.FromImage(img);           //产生Graphics对象，进行画图

            //绘制渐变背景
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            Brush brushBack = new LinearGradientBrush(rect, Color.FromArgb(random.Next(120, 256), 255, 255), Color.FromArgb(255, random.Next(120, 256), 255), random.Next(180));
            gfc.FillRectangle(brushBack, rect);

            var valuelength = Convert.ToInt32(img.Width / strCode.Length);
            var valueheight = (img.Height / 3) * 2;

            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            //字体列表，用于验证码
            string[] font_arr = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };

            //进行单个字符位置随机
            foreach (var item in strCode)
            {
                Font font = new Font(font_arr[random.Next(font_arr.Length)], 16, FontStyle.Bold);
                System.Drawing.Drawing2D.LinearGradientBrush brush =
                    new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, img.Width, img.Height), Color.DarkOrchid, Color.Blue, 1.5f, true);

                Point dot = new Point(14, 14);
                //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的
                float angle = random.Next(-45, 45);//转动的度数

                gfc.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                gfc.RotateTransform(angle);
                gfc.DrawString(item.ToString(), font, brush, 1, 1, format);
                //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                gfc.RotateTransform(-angle);//转回去
                gfc.TranslateTransform(-2, -dot.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别
            }

            //将图像添加到页面
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            //更改Http头
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/gif";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //Dispose
            gfc.Dispose();
            img.Dispose();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 画噪点
        /// </summary>
        /// <param name="img"></param>
        private void drawPoint(Bitmap img)
        {
            for (int i = 0; i < 100; i++)
            {
                int col = random.Next();//在一次的图片中杂店颜色相同
                int x = random.Next(img.Width);
                int y = random.Next(img.Height);
                img.SetPixel(x, y, Color.FromArgb(col));
            }
        }

        /// <summary>
        /// 画噪线
        /// </summary>
        /// <param name="gfc"></param>
        /// <param name="img"></param>
        /// <param name="color"></param>
        private void drawLine(Graphics gfc, Bitmap img, Color[] color)
        {
            //选择画10条线,也可以增加，也可以不要线，只要随机杂点即可
            for (int i = 0; i < 10; i++)
            {
                int x1 = random.Next(img.Width);
                int y1 = random.Next(img.Height);
                int x2 = random.Next(img.Width);
                int y2 = random.Next(img.Height);
                gfc.DrawLine(new Pen(color[random.Next(color.Length)]), x1, y1, x2, y2);      //注意画笔一定要浅颜色，否则验证码看不清楚
            }
        }
        #endregion

        #region 生成验证码
        private string getCodeString(CodeType cType, int length = 4)
        {
            var array = new ArrayList();
            switch (cType)
            {
                case CodeType.number:
                    getNumCodeString(ref array, length);
                    break;
                case CodeType.english:
                    getEnglistCodeString(ref array, length);
                    break;
                case CodeType.chinese:
                    getChineseCodeString(ref array, length);
                    break;
                case CodeType.all:
                    getAllCodeString(ref array, length);
                    break;
                default:
                    getNumCodeString(ref array, length);
                    break;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in array)
            {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }

        private void getAllCodeString(ref ArrayList array, int length)
        {
            for (int i = 0; i < length; i++)
            {
                var type = random.Next(0, 100);
                if (type % 2 == 0)
                    getEnglistCodeString(ref array, 1);
                else
                    getNumCodeString(ref array, 1);
            }
        }

        private void getChineseCodeString(ref ArrayList array, int length)
        {
            Encoding gb = Encoding.GetEncoding("gb2312");
            //调用函数产生4个随机中文汉字编码 柯乐义
            object[] bytes = CreateRegionCode(length);
            //根据汉字编码的字节数组解码出中文汉字 
            for (int i = 0; i < length; i++)
            {
                string str = gb.GetString((byte[])Convert.ChangeType(bytes[i], typeof(byte[])));
                array.Add(str);
            }
        }

        private object[] CreateRegionCode(int length)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            string[] rBase = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            var rnd = random;
            //定义一个object数组用来 
            object[] bytes = new object[length];

            /*每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bject数组中 
            每个汉字有四个区位码组成 
            区位码第1位和区位码第2位作为字节数组第一个元素 
            区位码第3位和区位码第4位作为字节数组第二个元素 
            */
            for (int i = 0; i < length; i++)
            {
                //区位码第1位 
                int r1 = rnd.Next(11, 14);
                string str_r1 = rBase[r1].Trim();

                //区位码第2位 
                rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的

                //种子避免产生重复值 
                int r2;
                if (r1 == 13)
                {
                    r2 = rnd.Next(0, 7);
                }
                else
                {
                    r2 = rnd.Next(0, 16);
                }
                string str_r2 = rBase[r2].Trim();

                //区位码第3位 
                rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                int r3 = rnd.Next(10, 16);
                string str_r3 = rBase[r3].Trim();

                //区位码第4位 
                rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                {
                    r4 = rnd.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = rnd.Next(0, 15);
                }
                else
                {
                    r4 = rnd.Next(0, 16);
                }
                string str_r4 = rBase[r4].Trim();

                //定义两个字节变量存储产生的随机汉字区位码
                byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                //将两个字节变量存储在字节数组中 
                byte[] str_r = new byte[] { byte1, byte2 };

                //将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(str_r, i);
            }
            return bytes;
        }

        private void getEnglistCodeString(ref ArrayList array, int length)
        {
            var total = DateTime.Now.Millisecond;
            for (int i = 0; i < length; i++)
            {
                var randNum = random.Next(0, total * random.Next(1, 8));
                if (randNum % 2 == 0)
                    array.Add((char)random.Next(65, 90));
                else
                    array.Add((char)random.Next(97, 122));
            }
        }

        private void getNumCodeString(ref ArrayList array, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Random rd = random;
                array.Add(rd.Next(-1, 10));
            }
        }
        #endregion
    }

    /// <summary>
    /// 验证码类型
    /// </summary>
    public enum CodeType
    {
        /// <summary>
        /// 数字
        /// </summary>
        [Description("数字")]
        number,
        /// <summary>
        /// 英文字母
        /// </summary>
        [Description("英文字母")]
        english,
        /// <summary>
        /// 中文汉字
        /// </summary>
        [Description("中文汉字")]
        chinese,
        /// <summary>
        /// 数字加英文字母
        /// </summary>
        [Description("数字加英文字母")]
        all,
    }
}
