using BLL.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DTO.Base;
using Infrastructure.Data;

namespace BLL
{
    /// <summary>
    /// مدیریت منوهای قابل دسترسی هر نقش
    /// </summary>
    public class RoleMenuManager : Manager<RoleMenu, ApplicationContext>, IRoleMenuManager
    {
        public RoleMenuManager(DbContexts _Context) : base(_Context)
        {
        }



        /// <summary>
        /// گرفتن همه دسترسی های یک نقش خاص
        /// </summary>
        /// <param name="RoleId">شناسه نقش</param>
        /// <returns></returns>
        public List<RoleMenu> GetByRoleId(long? RoleId)
        {
            if (RoleId == null) return null;
            return UOW.RoleMenus.GetByRoleId(RoleId).ToList();
        }




        /// <summary>
        /// حذف دسترسی های قبلی و افزودن دسترسی های جدید
        /// </summary>
        /// <param name="RoleId">شناسه نقش</param>
        /// <param name="MenuIds">شناسه منو ها</param>
        /// <returns></returns>
        public BaseResult UpdateRoleMenus(long RoleId, List<long> MenuIds)
        {
            var oldMenus = UOW.RoleMenus.Get(x => x.RoleId == RoleId);
            UOW.RoleMenus.RemoveRange(oldMenus);

            var newMenus = MenuIds.Select(x => new RoleMenu { RoleId = RoleId, MenuId = x });
            UOW.RoleMenus.AddRange(newMenus);

            var isSuccess = UOW.Commit();
            if (!isSuccess)
                return new BaseResult(false, "خطا در بروزرسانی اطلاعات.");
            return new BaseResult(true, "بروزرسانی با موفقیت انجام شد.");
        }


    }
}
