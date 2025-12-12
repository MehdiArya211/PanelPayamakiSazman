using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumberAssignment
{
    public class SenderNumberAssignmentFormViewModel
    {
        public SenderNumberAssignmentCreateDTO CreateModel { get; set; }
            = new SenderNumberAssignmentCreateDTO();

        public SenderNumberAssignmentEditDTO EditModel { get; set; }

        public List<SelectListItem> SenderNumberOptions { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> UserOptions { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> ClientOptions { get; set; }
            = new List<SelectListItem>();
    }
}
