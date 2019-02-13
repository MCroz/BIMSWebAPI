using BIMSWebAPI.App_Code;
using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    [Authorize]
    public class StocksController : ApiController
    {
        //[AllowAnonymous]
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

        //[AllowAnonymous]
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

                    //For Logging
                    var currentUser = context.Users.Find(newStock.ModifiedBy);
                    string logChanges = "";
                    foreach (PropertyInfo pi in newStock.GetType().GetProperties())
                    {
                        //pi.GetValue(myClass, null)?.ToString();
                        if (pi.Name != "ModifiedBy" && pi.Name != "CreatedBy" && pi.Name != "DateModified" && pi.Name != "DateCreated" && pi.Name != "ID")
                        {
                            if (logChanges != "")
                            {
                                logChanges += ", ";
                            }
                            logChanges += pi.GetValue(newStock, null)?.ToString() != "" ? pi.Name + " = " + pi.GetValue(newStock, null)?.ToString() : "";
                        }
                    }
                    string changes = currentUser.Username + " Added A New Stock and Set: " + logChanges;
                    AuditLogHelper.GenerateLog(context, "Create", changes);
                    //For Logging


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

        //[AllowAnonymous]
        [Route("api/Stocks/GetMedicineStocks/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMedicineStocks(int id)
        {
            using (BimsContext context = new BimsContext())
            {
                var res = (from im in context.InventoryMovement
                           join stock in context.Stocks on im.StockID equals stock.ID
                           join medicine in context.Medicines on stock.MedicineID equals medicine.ID
                           where medicine.ID == id
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
                    data = res
                });
            }
        }

        public class AddStockClass : BaseEntity {
            public int MedicineID { get; set; }
            public int Quantity { get; set; }
            public DateTime ExpirationDate { get; set; }
        }
    }
}