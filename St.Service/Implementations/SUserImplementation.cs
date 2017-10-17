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
                int skip = Page * Query.PageCountNumber;
                if (skip > pageQuery.Count())
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

                    string enPassword = GetPassWord(name, password);
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

        protected string GetPassWord(string name, string password)
        {
            return DesHandle.EnDes($"{name}|{password}", this.desKey);
        }

        private readonly string desKey = "11P1*2f?";

        new public void Add(SUser user)
        {
            using (var db = base.NewDB())
            {
                user.PassWord = GetPassWord(user.Name, user.PassWord);
                SetData(user);
                db.Entry(user).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        public void SetData(SUser entity, DateType type = DateType.Add)
        {
            if (type == DateType.Add)
                entity.AddDateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
        }
    }
}
