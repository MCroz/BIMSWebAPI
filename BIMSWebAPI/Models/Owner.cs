using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BIMSWebAPI.Models
{
    public class Owner : BaseEntity
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Image { get; set; }

        public IList<Business> Businesses { get; set; }

        public int isRemoved { get; set; }
    }
}