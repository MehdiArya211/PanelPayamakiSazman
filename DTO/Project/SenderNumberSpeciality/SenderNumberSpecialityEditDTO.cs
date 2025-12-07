using System.ComponentModel.DataAnnotations;

namespace DTO.Project.SenderNumberSpeciality
{
    public class SenderNumberSpecialityEditDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "وارد کردن کد الزامی است.")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "کد باید دقیقاً دو رقم و فقط عدد باشد.")]
        public string code { get; set; }

        [Required(ErrorMessage = "عنوان الزامی است.")]
        [StringLength(100, ErrorMessage = "عنوان نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string title { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.")]
        public string description { get; set; }
    }
}
