using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SmsManager
{
    /// <summary>
    /// ساختار خاص خروجی تاریخچه پیامک‌ها:
    /// { page, pageSize, totalCount, totalPages, items: [...] }
    /// </summary>
    public class AdminSmsHistoryResponseDTO<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public List<T> Items { get; set; }
    }
}
