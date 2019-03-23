using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class InventoryMovement : BaseEntity
    {
        public int ID { get; set; }

        public int StockID { get; set; }
        public Stock Stock { get; set; }

        public int Quantity { get; set; }

        public int? Days { get; set; }
        public int? Intakes { get; set; }

        public int? DispenseTransactionID { get; set; }
        public DispenseTransaction DispenseTransaction { get; set; }
    }
}