using St.Domain.Entity;
using St.Domain.Entity.AD;
using St.Domain.Entity.News;
using St.Domain.Entity.Picture;
using St.Domain.Entity.Product;
using St.Domain.Entity.SuperUser;
using St.Domain.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Domain
{
    public class TasterDbContext: DbContext
    {
        public TasterDbContext()
            : base("TasterConnection")
        {
            Database.SetInitializer<TasterDbContext>(null);
            //在应用程序启动时自动升级（MigrateDatabaseToLatestVersion初始化器）
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TasterDbContext, Migrations.Configuration>());

            //延迟加载
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = true;
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Configurations
                .Add(new AdsMap())
                .Add(new ImagesMap())
                .Add(new NewsMainMap())
                .Add(new NewsShowMap())
                .Add(new ProductClassIntroductionMap())
                .Add(new ProductClassMap())
                .Add(new ProductImagesMap())
                .Add(new ProductsMap())
                .Add(new SUserMap());

            base.OnModelCreating(modelBuilder);
        }

        #region DbSet
        public DbSet<Ads> Ads { get; set; }

        public DbSet<Images> Images { get; set; }

        public DbSet<NewsMain> NewsMain { get; set; }

        public DbSet<NewsShow> NewsShow { get; set; }

        public DbSet<ProductClassIntroduction> ProductClassIntroduction { get; set; }

        public DbSet<ProductClass> ProductClass { get; set; }

        public DbSet<ProductImages> ProductImages { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<SUser> SUser { get; set; }
        #endregion
    }
}
