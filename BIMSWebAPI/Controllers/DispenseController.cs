using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    public class DispenseController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Dispense/GetMedicineStocksList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMedicineStocksList()
        {
            using (var context = new BimsContext())
            {
                var outVal = (from im in context.InventoryMovement
                            join stock in context.Stocks on im.StockID equals stock.ID
                            join medicine in context.Medicines on stock.MedicineID equals medicine.ID
                            group im by new { medicine.MedicineName, stock.ID, medicine.Description, stock.ExpirationDate } into g
                            select new
                            {
                                MedicineName = g.Key.MedicineName,
                                Description = g.Key.Description,
                                Total = g.Sum(test => test.Quantity),
                                ExpirationDate = g.Key.ExpirationDate,
                                StockID = g.Key.ID
                            }).ToList();



                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = outVal
                });
            }

        }

        [AllowAnonymous]
        [Route("api/Dispense/DispenseTransaction")]
        [HttpPost]
        public async Task<IHttpActionResult> DispenseTransaction(CustomDispenseData model)
        {
            using (var context = new BimsContext())
            {
                

                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Added",
                    data = model
                });
            }

        }

        public class CustomDispenseData {
            public int ResidentID { get; set; }
            public List<CustomDispenseMedicineItem> Items { get; set; }
            public string PrescriptionDescription { get; set; }
        }

        public class CustomDispenseMedicineItem {
            public int StockID { get; set; }
            public int Quantity { get; set; }
        }

    }
}