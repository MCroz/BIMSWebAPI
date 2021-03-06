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
    [AllowAnonymous]
    public class DispenseController : ApiController
    {
        //[AllowAnonymous]
        [Route("api/Dispense/GetMedicineStocksList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMedicineStocksList()
        {
            using (var context = new BimsContext())
            {
                var outVal = (from im in context.InventoryMovement
                            join stock in context.Stocks on im.StockID equals stock.ID
                            join medicine in context.Medicines on stock.MedicineID equals medicine.ID
                            where stock.ExpirationDate > DateTime.Now
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

        //[AllowAnonymous]
        [Route("api/Dispense/DispenseTransaction")]
        [HttpPost]
        public async Task<IHttpActionResult> DispenseTransaction(CustomDispenseData model)
        {
            using (var context = new BimsContext())
            {
                var resident = context.Residents.Find(model.ResidentID);
                DispenseTransaction dispense = new DispenseTransaction { Resident = resident, ResidentID = resident.ID, PrescriptionDescription = model.PrescriptionDescription, CreatedBy = model.CreatedBy, ModifiedBy = model.ModifiedBy, DoctorName = model.DoctorName, DoctorLicenseNo = model.DoctorLicenseNo };
                context.DispenseTransactions.Add(dispense);
                foreach (CustomDispenseMedicineItem itm in model.Items)
                {
                    int qty = -1 * itm.Quantity;
                    var stock = context.Stocks.Find(itm.StockID);
                    InventoryMovement im = new InventoryMovement { CreatedBy = model.CreatedBy, ModifiedBy = model.ModifiedBy, StockID = itm.StockID, Quantity =  qty, DispenseTransaction = dispense, Stock = stock};
                    context.InventoryMovement.Add(im);
                    //dispense.InventoryMovement.Add(im);
                }
                context.SaveChanges();
            }
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Added"
            });
        }

        //[AllowAnonymous]
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
                    //var dispenseTransactions = (from dt in context.DispenseTransactions
                    //                            join res in context.Residents on dt.ResidentID equals resident.ID
                    //                            join user in context.Users on dt.CreatedBy equals user.ID
                    //                            where dt.ResidentID == id
                    //                            select new {
                    //                                ID = dt.ID,
                    //                                CreatedBy = user.FirstName,
                    //                                PreparationDescription = dt.PrescriptionDescription,
                    //                                DateDispensed = dt.DateCreated
                    //                            }).ToList();

                    var dispenseTransactions = (from finalDt in (from im in context.InventoryMovement
                                                   join dt in context.DispenseTransactions on im.DispenseTransactionID equals dt.ID
                                                   group im by im.DispenseTransaction into g
                                                   select new
                                                   {
                                                       ID = g.Key.ID,
                                                       DateDispensed = g.Key.DateCreated,
                                                       UserID = g.Key.CreatedBy,
                                                       PrescriptionDescription = g.Key.PrescriptionDescription
                                                   }
                                  )
                                  join finalUser in context.Users on finalDt.UserID equals finalUser.ID
                                  select new
                                  {
                                      DateDispensed = finalDt.DateDispensed,
                                      CreatedBy = finalUser.FirstName + " " + finalUser.MiddleName + " " + finalUser.LastName,
                                      PreparationDescription = finalDt.PrescriptionDescription,
                                      ID = finalDt.ID
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

        //[AllowAnonymous]
        [Route("api/Dispense/ListDispenseMedicines/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> ListDispenseMedicines(int id)
        {
            using (var context = new BimsContext())
            {
                var dispenseTransaction = await context.DispenseTransactions.FindAsync(id);
                if (dispenseTransaction == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Dispense Transaction Found"
                    });
                }
                else
                {
                    //Get All Dispense Transaction Medicines
                    var dispenseTransactions = (from im in context.InventoryMovement
                                                join dt in context.DispenseTransactions on im.DispenseTransactionID equals dt.ID
                                                join s in context.Stocks on im.StockID equals s.ID
                                                join m in context.Medicines on s.MedicineID equals m.ID
                                                where im.DispenseTransactionID == id
                                                select new
                                                {
                                                    ID = im.ID,
                                                    Medicine = m.MedicineName,
                                                    Description = m.Description,
                                                    ExpirationDate = s.ExpirationDate,
                                                    Quantity = im.Quantity * -1
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
            public string DoctorName { get; set; }
            public string DoctorLicenseNo { get; set; }
        }

        public class CustomDispenseMedicineItem {
            public int StockID { get; set; }
            public int Quantity { get; set; }
        }


        //[AllowAnonymous]
        [Route("api/Dispense/NewDispenseTransaction")]
        [HttpPost]
        public async Task<IHttpActionResult> NewDispenseTransaction(DispenseTransaction model)
        {
            using (var context = new BimsContext())
            {
                var resident = context.Residents.Find(model.ResidentID);
                DispenseTransaction dispense = new DispenseTransaction { Resident = resident, ResidentID = resident.ID, CreatedBy = model.CreatedBy, ModifiedBy = model.ModifiedBy, Prescriptive = model.Prescriptive, DoctorName = model.DoctorName, DoctorLicenseNo = model.DoctorLicenseNo };
                context.DispenseTransactions.Add(dispense);
                foreach (InventoryMovement itm in model.InventoryMovement)
                {
                    int qty = -1 * itm.Quantity;
                    var stock = context.Stocks.Find(itm.StockID);
                    InventoryMovement im = new InventoryMovement { CreatedBy = model.CreatedBy, ModifiedBy = model.ModifiedBy, StockID = itm.StockID, Quantity = qty, DispenseTransaction = dispense, Stock = stock, Days = itm.Days, Intakes = itm.Intakes };
                    context.InventoryMovement.Add(im);
                    //dispense.InventoryMovement.Add(im);
                }
                context.SaveChanges();
            }
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Added"
            });
        }

        //[AllowAnonymous]
        [Route("api/Dispense/ListDispenseTransactionV2/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> ListDispenseTransactionV2(int id)
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
                    var dispenseTransactions = (from finalDt in (from im in context.InventoryMovement
                                                                 join dt in context.DispenseTransactions on im.DispenseTransactionID equals dt.ID
                                                                 group im by im.DispenseTransaction into g
                                                                 select new
                                                                 {
                                                                     ID = g.Key.ID,
                                                                     DateDispensed = g.Key.DateCreated,
                                                                     UserID = g.Key.CreatedBy,
                                                                     Prescriptive = g.Key.Prescriptive,
                                                                     DoctorName = g.Key.DoctorName,
                                                                     DoctorLicenseNo = g.Key.DoctorLicenseNo
                                                                 })
                                                join finalUser in context.Users on finalDt.UserID equals finalUser.ID
                                                select new
                                                {
                                                    DateDispensed = finalDt.DateDispensed,
                                                    CreatedBy = finalUser.FirstName + " " + finalUser.MiddleName + " " + finalUser.LastName,
                                                    Prescriptive = finalDt.Prescriptive == 1 ? "Yes" : "No",
                                                    ID = finalDt.ID,
                                                    Prescriptions = "",
                                                    DoctorName = finalDt.DoctorName,
                                                    DoctorLicenseNo = finalDt.DoctorLicenseNo
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


        //[AllowAnonymous]
        [Route("api/Dispense/ListDispenseMedicinesV2/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> ListDispenseMedicinesV2(int id)
        {
            using (var context = new BimsContext())
            {
                var dispenseTransaction = await context.DispenseTransactions.FindAsync(id);
                if (dispenseTransaction == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Dispense Transaction Found"
                    });
                }
                else
                {
                    //Get All Dispense Transaction Medicines
                    var dispenseTransactions = (from im in context.InventoryMovement
                                                join dt in context.DispenseTransactions on im.DispenseTransactionID equals dt.ID
                                                join s in context.Stocks on im.StockID equals s.ID
                                                join m in context.Medicines on s.MedicineID equals m.ID
                                                where im.DispenseTransactionID == id
                                                select new
                                                {
                                                    ID = im.ID,
                                                    Medicine = m.MedicineName,
                                                    Description = m.Description,
                                                    ExpirationDate = s.ExpirationDate,
                                                    Quantity = im.Quantity * -1,
                                                    Intakes = im.Intakes,
                                                    Days = im.Days
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

    }
}