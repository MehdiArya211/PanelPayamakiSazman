using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderChargeRequestConsumer
{
    public class SenderChargeRequestFormViewModel
    {
        public SenderChargeRequestCreateDTO CreateModel { get; set; }

        // دراپ داون سرشماره (از داخل همین منیجر پر می‌شود)
        public List<SelectListItem> SenderNumberOptions { get; set; }
    }
}
