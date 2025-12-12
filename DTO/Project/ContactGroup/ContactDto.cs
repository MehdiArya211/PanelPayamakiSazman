using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ContactGroup
{
    public class ContactDto
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? NationalCode { get; set; }
        public string? BirthDate { get; set; }

        public List<ContactCustomFieldDTO>? CustomFields { get; set; }
    }
}
