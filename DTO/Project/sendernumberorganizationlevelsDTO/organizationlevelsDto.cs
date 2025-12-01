using DTO.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.sendernumberorganizationlevelsDTO
{
    class organizationlevelsDto :  EntityDTO
    {
        public string Code { get; set; }



        [MaxLength(200, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string Title { get; set; }



        [MaxLength(5000, ErrorMessage = "{0} حداکثر می تواند {1} کاراکتر باشد.")]
        public string description { get; set; }

    }
}
