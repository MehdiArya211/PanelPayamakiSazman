using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminUser
{
    /// <summary>
    /// مدل ایجاد کاربر مدیر جدید
    /// </summary>
    public class AdminUserCreateDTO
    {
        [Required(ErrorMessage = "نام کاربری الزامی است. ")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین 3 تا 50 کاراکتر باشد.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور اولیه الزامی است. ")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد.")]
        public string InitialPassword { get; set; }

        [Required(ErrorMessage = "نام الزامی است.")]
        [StringLength(100, ErrorMessage = "نام نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "نام خانوادگی الزامی است.")]
        [StringLength(100, ErrorMessage = "نام خانوادگی نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "کد ملی الزامی است.")]
        [StringLength(10, ErrorMessage = "کد ملی باید 10 کاراکتر باشد.")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "شماره موبایل الزامی است.")]
        [StringLength(11, ErrorMessage = "شماره موبایل باید 11 کاراکتر باشد.")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل باید با 09 شروع شود.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "انتخاب واحد سازمانی الزامی است.")]
        public string UnitId { get; set; }

        public List<string> RoleIds { get; set; } = new List<string>();

        public List<AdminSecurityQuestionAnswerDTO> SecurityQuestions { get; set; } = new List<AdminSecurityQuestionAnswerDTO>();
    }
}
