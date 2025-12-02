using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace DTO.Base
{
    /// <summary>
    /// مدل برای دراپدون
    /// </summary>
    public class SelectListDTO
    {
        [Display(Name = "شناسه")]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }



        #region سازنده ها
        public SelectListDTO()
        {
        }

        public SelectListDTO(long id, string title)
        {
            Id = id;
            Title = title;
        }
        #endregion




        #region تبدیل نقش ها به مدل دراپدون
        public static Expression<Func<Domain.Entities.Role, SelectListDTO>> RoleSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Title,
                };
            }
        }
        #endregion


        #region تبدیل کاربران به مدل دراپدون
        public static Expression<Func<Domain.Entities.User, SelectListDTO>> UserSelector
        {
            get
            {
                return model => new SelectListDTO()
                {
                    Id = model.Id,
                    Title = model.Name,
                };
            }
        }
        #endregion


    }
}
