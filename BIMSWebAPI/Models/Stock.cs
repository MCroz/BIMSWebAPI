using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class Stock : BaseEntity
    {
        public int ID { get; set; }

        public int MedicineID { get; set; }
        public Medicine Medicine { get; set;}

        [Column(TypeName = "Date")]
        public DateTime ExpirationDate { get; set; }

        public IList<InventoryMovement> InventoryMovement { get; set; }
    }
}