using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BIMSWebAPI.Models
{
    public class BusinessClearance : BaseEntity
    {
        public int ID { get; set; }

        public string OwnerFullName { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerContactNo { get; set; }

        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string FloorArea { get; set; }
        public string DTI_SEC_RegNo { get; set; }
        public string BusinessContactNo { get; set; }
        public string KindOfBusiness { get; set; }
        [DataType(DataType.Currency)]
        public decimal Capitalization { get; set; }


        public int? BusinessID { get; set; }
        public Business Business { get; set; }

        public string ControlNo { get; set; }

        public int isRemoved { get; set; }
    }
}