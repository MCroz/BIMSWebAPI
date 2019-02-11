using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BIMSWebAPI.Models
{
    public class Business : BaseEntity
    {
        public int ID { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string FloorArea { get; set; }
        public string DTI_SEC_RegNo { get; set; }
        public string BusinessContactNo { get; set; }
        public string KindOfBusiness { get; set; }

        [DataType(DataType.Currency)]
        public decimal Capitalization { get; set; }

        public int? OwnerID { get; set; }
        public Owner Owner { get; set; }

        public IList<BusinessClearance> BusinessClearanceTransactions { get; set; }

        public int isRemoved { get; set; }
    }
}