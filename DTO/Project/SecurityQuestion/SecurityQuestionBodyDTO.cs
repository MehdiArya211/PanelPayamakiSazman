using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SecurityQuestion
{
    /// <summary>
    /// مدل ارسال سؤال امنیتی در ساخت کاربر
    /// </summary>
    public class SecurityQuestionBodyDTO
    {
        /// <summary>
        /// شناسه سؤال امنیتی
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// پاسخ سؤال امنیتی
        /// </summary>
        public string Answer { get; set; }
    }
}
