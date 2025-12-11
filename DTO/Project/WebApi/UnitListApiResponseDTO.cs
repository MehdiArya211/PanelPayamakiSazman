using DTO.Project.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.WebApi
{
    /// <summary>
    /// مپ مستقیم SuccessResponseOfIEnumerableOfUnitDto
    /// {
    ///   "data": [ UnitDto... ],
    ///   "meta": null,
    ///   "traceId": "..."
    /// }
    /// </summary>
    public class UnitListApiResponseDTO
    {
        public List<UnitListDTO>? Data { get; set; }
        public object? Meta { get; set; }
        public string TraceId { get; set; }
    }
}
