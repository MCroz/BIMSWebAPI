﻿using BIMSWebAPI.Models;
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
                var resident = context.Residents.Find(model.ResidentID);
                DispenseTransaction dispense = new DispenseTransaction { Resident = resident, ResidentID = resident.ID, PrescriptionDescription = model.PrescriptionDescription, CreatedBy = model.CreatedBy, ModifiedBy = model.ModifiedBy };
                context.DispenseTransactions.Add(dispense);
                context.SaveChanges();
                foreach (CustomDispenseMedicineItem itm in model.Items)
                {
                    int qty = -1 * itm.Quantity;
                    var stock = context.Stocks.Find(itm.StockID);
                    InventoryMovement im = new InventoryMovement { CreatedBy = model.CreatedBy, ModifiedBy = model.ModifiedBy, StockID = itm.StockID, Quantity =  qty, DispenseTransactionID = dispense.ID, DispenseTransaction = dispense, Stock = stock};
                    context.InventoryMovement.Add(im);
                    dispense.InventoryMovement.Add(im);
                }
                context.SaveChanges();
            }
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Added"
            });
        }

        [AllowAnonymous]
        [Route("api/Dispense/ListDispenseTransaction/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> ListDispenseTransaction(int id)
        {
            using (var context = new BimsContext())
            {
                var resident = context.Residents.Find(id);
                if (resident == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No User Found"
                    });
                }
                else
                {
                    //Get All Dispense Transaction
                    var dispenseTransactions = (from dt in context.DispenseTransactions
                                                join res in context.Residents on dt.ResidentID equals resident.ID
                                                join user in context.Users on dt.CreatedBy equals user.ID
                                                select new {
                                                    ID = dt.ID,
                                                    CreatedBy = user.FirstName,
                                                    PreparationDescription = dt.PrescriptionDescription,
                                                    DateDispensed = dt.DateCreated
                                                }).ToList();
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = dispenseTransactions
                    });
                }
            }
        }

        //public class FetchDispenseData {
        //    public int ResidentID { get; set; }
        //}


        public class CustomDispenseData {
            public int ResidentID { get; set; }
            public List<CustomDispenseMedicineItem> Items { get; set; }
            public string PrescriptionDescription { get; set; }
            public int CreatedBy { get; set; }
            public int ModifiedBy { get; set; }
        }

        public class CustomDispenseMedicineItem {
            public int StockID { get; set; }
            public int Quantity { get; set; }
        }

    }
}