using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.SuperUser;

namespace St.Service.Implementations
{
    public class SUserImplementation : ServiceBase<SUser>, ISUserInterface
    {
        public List<SUser> QueryForPage(int Page, QueryExpression<SUser> Query)
        {
            using (var db = base.NewDB())
            {
                var pageQuery = db.Set<SUser>().AsNoTracking().AsQueryable();
                pageQuery = pageQuery.Where(Query.QueryExpressions.GetExpression());
                Query.PageSumCount = pageQuery.Count();

                int skip = Page * Query.PageCountNumber;
                if (skip > Query.PageSumCount)
                    return new List<SUser>();

                return pageQuery.OrderBy(p => p.ID).Skip(skip).Take(Query.PageCountNumber).ToList();
            }
        }

        public SUser UserLogin(string name, string password)
        {
            try
            {
                using (var db = base.NewDB())
                {
                    var userArray = db.Set<SUser>().AsNoTracking().Where(p => p.Name == name);
                    var count = userArray.Count();
                    if (count == 0 || count > 1)
                        return null;

                    string enPassword = GetEnDesPassWord(name, password);
                    if (userArray.First().PassWord == enPassword)
                        return userArray.First();
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取加密后的密码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected string GetEnDesPassWord(string name, string password)
        {
            return DesHandle.EnDes($"{name}|{password}", this.desKey);
        }

        /// <summary>
        /// 获取原密码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected string GetDeDesPassWord(string name,string password)
        {
            string temp = DesHandle.DeDes(password, this.desKey);
            string[] tempSplit = temp.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (tempSplit.Length != 2)
                throw new Exception("用户密码信息不正确，请管理员确认是否从网站注册。");

            string result = string.Empty;
            foreach (var item in tempSplit)
            {
                if (item == name)
                    continue;

                result = item;
            }

            return result;
        }

        private readonly string desKey = "11P1*2f?";

        new public void Add(SUser user)
        {
            using (var db = base.NewDB())
            {
                user.PassWord = GetEnDesPassWord(user.Name, user.PassWord);
                SetData(user);
                db.Entry(user).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        new public void Modify(SUser user)
        {
            user.PassWord = GetEnDesPassWord(user.Name, user.PassWord);
            base.Modify(user);
        }

        new public SUser GetByID(int id)
        {
            var temp = base.GetByID(id);
            temp.PassWord = GetDeDesPassWord(temp.Name, temp.PassWord);
            return temp;
        }

        public void SetData(SUser entity, DateType type = DateType.Add)
        {
            if (type == DateType.Add)
                entity.AddDateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
        }
    }
}
