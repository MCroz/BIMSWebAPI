using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class BaseEntity
    {
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        //public int? ModifiedByID { get; set; }
        //[ForeignKey("ModifiedByID")]
        //public User ModifiedBy { get; set; }

        //public int? CreatedByID { get; set; }
        //[ForeignKey("CreatedByID")]
        //public User CreatedBy { get; set; }
    }
}