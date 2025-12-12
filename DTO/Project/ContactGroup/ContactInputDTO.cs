using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ContactGroup
{
    public class ContactInputDTO
    {
        [Required(ErrorMessage = "شماره تلفن الزامی است.")]
        public string PhoneNumber { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalCode { get; set; }
        public string? BirthDate { get; set; }

        public List<ContactCustomFieldDTO>? CustomFields { get; set; }
    }
}
