using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using St.Code;
using St.Domain.Entity.Product;
using System.Data.Entity;

namespace St.Service.Implementations
{
    public class ProductImagesImplementation : ServiceBase<ProductImages>, IProductImagesInterface
    {
        new public void Add(ProductImages entity)
        {
            using (var db = base.NewDB())
            {
                entity.product = db.Products.Single(p => p.ID == entity.product.ID);
                entity.Image = db.Images.Single(p => p.ID == entity.Image.ID);

                db.Entry(entity.product).State = System.Data.Entity.EntityState.Unchanged;
                db.Entry(entity.Image).State = System.Data.Entity.EntityState.Unchanged;
                db.Entry(entity).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
        }

        public void DeleteImage(int pid, int piid)
        {
            using (var db=base.NewDB())
            {
                var product = db.Products.Where(p => p.ID == pid).Include(p => p.productImages).Single();
                db.Entry(product.productImages.Single(p=>p.ID==piid)).State = EntityState.Deleted;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ProductImages GetByIDForInclude(int id, string include)
        {
            using (var db=base.NewDB())
            {
                return db.ProductImages.AsNoTracking().Where(p => p.ID == id).Include(include).Single();
            }
        }

        public List<ProductImages> GetProductImagesIncludeImage(string[] idArray)
        {
            using (var db = base.NewDB())
            {
                Specification.ISpecification<ProductImages> specification = new Specification.NoneSpecification<ProductImages>();
                for (int i = 0; i < idArray.Length; i++)
                {
                    var item = Convert.ToInt32(idArray[i]);
                    specification.And(new Specification.ExpressionSpecification<ProductImages>(p => p.ID == item));
                }

                return db.ProductImages.AsNoTracking().Where(specification.GetExpression()).Include(p => p.Image).ToList();
            }
        }

        new public void Modify(ProductImages entity)
        {
            using (var db = base.NewDB())
            {
                var old = db.ProductImages.Single(p => p.ID == entity.ID);
                var oldPreproties = old.GetType().GetProperties();
                var newPreproties = entity.GetType().GetProperties();
                foreach (var olditem in oldPreproties)
                {
                    foreach (var item in newPreproties)
                    {
                        if (olditem.Name == item.Name)
                        {
                            olditem.SetValue(old, item.GetValue(entity));
                            break;
                        }
                    }
                }
                old.product = db.Products.Single(p => p.ID == entity.product.ID);
                old.Image = db.Images.Single(p => p.ID == entity.Image.ID);

                db.Entry(old.Image).State = EntityState.Unchanged;
                db.Entry(old).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
