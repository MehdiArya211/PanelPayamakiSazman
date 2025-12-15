using DTO.Base;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTO.Project.SystemMenu
{
    /// <summary>
    /// اطلاعات منو که از وب‌سرویس دریافت می‌شود
    /// </summary>
    public class SystemMenuSessionDTO 
    {
        public string Id { get; set; }

        [Display(Name = "عنوان منو")]
        public string Title { get; set; }

        /// <summary>
        /// شناسه منوی والد
        /// </summary>
        [Display(Name = "سر دسته")]
        public string ParentId { get; set; }  

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        [Display(Name = "ترتیب")]
        public int Order { get; set; }

        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Display(Name = "نمایش در منو")]
        public bool ShowInMenu { get; set; }

        [Display(Name = "لینک دارد")]
        public bool HasLink { get; set; }

        [Display(Name = "نیاز به احراز هویت مجدد")]
        public bool NeedReAuthorize { get; set; }

        #region مشخصات لینک

        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        /// <summary>
        /// پارامترهای لینک به صورت JSON string
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// پارامترها به صورت شیء
        /// </summary>
        public object? Params =>
            string.IsNullOrWhiteSpace(Parameters)
                ? null
                : JsonConvert.DeserializeObject<object>(Parameters);

        #endregion

        /// <summary>
        /// زیرمنوها (در UI ساخته می‌شود، از API نمی‌آید)
        /// </summary>
        public List<SystemMenuSessionDTO> SubMenus { get; set; } = new();
    }
}
