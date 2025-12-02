using DAL.Interface;
using Infrastructure.Data;
using Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationContext applicationContext;
        protected readonly LogContext logContext;

        public UnitOfWork(DbContexts contexts)
        {
            applicationContext = contexts[DbContextType.ApplicationContext.ToString()] as ApplicationContext;
            logContext = contexts[DbContextType.LogContext.ToString()] as LogContext;
        }



        #region private properties
        #region Auth System
        private MenuRepository _menus;
        private RoleRepository _roles;
        private RoleMenuRepository _roleMenus;
        private UserRepository _users;
        #endregion

        #region LogSystem
        private UserLogRepository _userLogs;
        #endregion

        #region Shared
        private ConstantRepository _constants;
        #endregion

        #endregion


        #region Repository Getters 

        #region Auth System
        public IMenuRepository Menus
        {
            get
            {
                if (_menus == null)
                    _menus = new MenuRepository(applicationContext);
                return _menus;
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                if (_roles == null)
                    _roles = new RoleRepository(applicationContext);
                return _roles;
            }
        }

        public IRoleMenuRepository RoleMenus
        {
            get
            {
                if (_roleMenus == null)
                    _roleMenus = new RoleMenuRepository(applicationContext);
                return _roleMenus;
            }
        }

        public IUserRepository Users
        {
            get
            {
                if (_users == null)
                    _users = new UserRepository(applicationContext);
                return _users;
            }
        }

        #endregion

        #region LogSystem
        public IUserLogRepository UserLogs
        {
            get
            {
                if (_userLogs == null)
                    _userLogs = new UserLogRepository(logContext);
                return _userLogs;
            }
        }

        #endregion


        #region Shared
        public IConstantRepository Constants
        {
            get
            {
                if (_constants == null)
                    _constants = new ConstantRepository(applicationContext);
                return _constants;
            }
        }

        #endregion
        #endregion


        #region Commit
        /// <summary>
        /// ذخیره تغییرات انجام شده
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            try
            {
                if (applicationContext.ChangeTracker.HasChanges())
                    applicationContext.SaveChanges();

                if (logContext.ChangeTracker.HasChanges())
                    logContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        /// <summary>
        /// ذخیره تغییرات انجام شده
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CommitAsync()
        {
            try
            {
                if (applicationContext.ChangeTracker.HasChanges())
                    await applicationContext.SaveChangesAsync();

                if (logContext.ChangeTracker.HasChanges())
                    await logContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion


        #region ExecuteNonQuery

        /// <summary>
        /// اجرای یک کوئری بدون مقدار بازگشتی
        /// </summary>
        /// <param name="Query">متن کوئری</param>
        /// <param name="Parameters">پارامتر های کوئری</param>
        /// <returns></returns>
        public bool ExecuteNonQuery<TContext>(string Query, params object[] Parameters)
        {
            try
            {
                if (typeof(TContext) == typeof(LogContext))
                {
                    int NumberOfRowEffected = logContext.Database.ExecuteSqlRaw(Query, Parameters);
                    return true;
                }
                else
                {
                    int NumberOfRowEffected = applicationContext.Database.ExecuteSqlRaw(Query, Parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }



        /// <summary>
        /// اجرای یک کوئری بدون مقدار بازگشتی
        /// </summary>
        /// <param name="Query">متن کوئری</param>
        /// <param name="Parameters">پارامتر های کوئری</param>
        /// <returns></returns>
        public async Task<bool> ExecuteNonQueryAsync<TContext>(string Query, params object[] Parameters)
        {
            try
            {
                if (typeof(TContext) == typeof(LogContext))
                {
                    int NumberOfRowEffected = await logContext.Database.ExecuteSqlRawAsync(Query, Parameters);
                    return true;
                }
                else
                {
                    int NumberOfRowEffected = await applicationContext.Database.ExecuteSqlRawAsync(Query, Parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion


        /// <summary>
        /// پاک کردن آبجکت unit of work و Context از رم
        /// </summary>
        public void Dispose()
        {
            applicationContext.Dispose();
            logContext.Dispose();
        }


    }
}
