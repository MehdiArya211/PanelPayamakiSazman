using System.Reflection;
using Domain.Entities;
using Infrastracture.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Fluent API - لود کردن از کلاس های جانبی
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #endregion


            #region DeleteBehavior - رفتار در هنگام حذف دیتا
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            #endregion


            #region اعمال حذف نرم افزاری در کوئری ها
            //modelBuilder.Entity<Post>().HasQueryFilter(x => EF.Property<bool>(x, "IsDeleted") == false);
            #endregion


            #region seed data
            SeedData.InitialSeedData(ref modelBuilder);
            #endregion

        }


        #region جداول دیتابیسی

        #region AuthSystem
        
        /// <summary>
        /// منو ها
        /// </summary>
        public DbSet<Menu> Menus { get; set; }

        /// <summary>
        /// نقش ها
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// دسترسی منو ها
        /// </summary>
        public DbSet<RoleMenu> RoleMenus { get; set; }

        /// <summary>
        /// پرسنل
        /// </summary>
        public DbSet<User> Users { get; set; }
        #endregion

        #region Shared
        /// <summary>
        /// مقادیر ثابت
        /// </summary>
        public DbSet<Constant> Constants { get; set; }
        #endregion
        #endregion




    }
}
