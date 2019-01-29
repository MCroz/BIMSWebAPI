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
    public class StocksController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Stocks/GetStocksList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStocksList()
        {
            BIMSDb db = new BIMSDb();
            //db.Select("SELECT m.MedicineName as 'Medicine Name', m.Description as 'Description', ");
            DataTable dt = db.Select("SELECT m.ID as 'ID' ,m.MedicineName as 'Medicine Name', m.Description as 'Description',COALESCE(SUM(im.Quantity),0) as 'Stock On Hand' FROM inventorymovements im INNER JOIN stocks s ON s.ID = im.StockID LEFT JOIN medicines m ON s.MedicineID = m.ID GROUP BY m.ID");

            //List<DataRow> rows = dt.Rows.Cast<DataRow>().ToList();
            var tempData = (from DataRow row in dt.Rows
                            select new
                            {
                                ID = row["ID"].ToString(),
                                MedicineName = row["Medicine Name"].ToString(),
                                Description = row["Description"].ToString(),
                                Quantity = Convert.ToInt32(row["Stock On Hand"].ToString())
                            }).ToList();

            dt.Dispose();
            db.CloseConnection();


            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Fetched",
                data = tempData
            });
        }

        [AllowAnonymous]
        [Route("api/Stocks/AddStock")]
        [HttpPost]
        public async Task<IHttpActionResult> AddStock(AddStockClass model)
        {
            using (var context = new BimsContext())
            {
                Medicine medicine = context.Medicines.Find(model.MedicineID);
                if (medicine == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Medicine Found"
                    });
                }
                else
                {
                    Stock newStock = new Models.Stock { ModifiedBy = model.ModifiedBy, CreatedBy = model.CreatedBy, MedicineID = model.MedicineID, ExpirationDate = model.ExpirationDate, Medicine = medicine };
                    context.Stocks.Add(newStock);
                    context.SaveChanges();
                    InventoryMovement im = new InventoryMovement { Quantity = model.Quantity, ModifiedBy = model.ModifiedBy, CreatedBy = model.CreatedBy, StockID = newStock.ID, Stock = newStock};
                    context.InventoryMovement.Add(im);
                    context.SaveChanges();
                }
            }
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Added"
            });
        }

        public class AddStockClass : BaseEntity {
            public int MedicineID { get; set; }
            public int Quantity { get; set; }
            public DateTime ExpirationDate { get; set; }
        }
    }
}