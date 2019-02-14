using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    //[Authorize]
    public class ReportsController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Reports/GetPreMadeReports")]
        [HttpPost]
        public async Task<IHttpActionResult> GetPreMadeReports(ReportModel rm)
        {

            DateTime startDate = rm.StartDate;
            DateTime endDate = rm.EndDate;
            //DateTime endDate = rm.StartDate.Date.AddDays(1);
            var tae = "";
            using (var context = new BimsContext())
            {
                if (rm.ReportType == "System Logs") 
                {
                    var Report = context.SystemLogs.AsNoTracking().Where(b => b.LogTime >= startDate && b.LogTime <= endDate).ToList();
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = Report
                    });
                }
                if (rm.ReportType == "Dispense Report")
                {
                    //var Report = context.DispenseTransactions.Where(d => d.DateCreated >= startDate && d.DateCreated < endDate).GroupJoin(context.InventoryMovement, DTransaction => DTransaction.ID, IMovement => IMovement.DispenseTransactionID, (x, y) => new { DispenseTransaction = x, InventoryMovements = y })
                    //    .SelectMany(
                    //        xy => xy.InventoryMovements.DefaultIfEmpty(),
                    //        (x, y) => new { DispenseTransaction = x, InventoryMovement = y }
                    //    ).Select(s => s).ToList();

                    var Report = (from finalDt in (from im in context.InventoryMovement
                                                   join dt in context.DispenseTransactions on im.DispenseTransactionID equals dt.ID
                                                   join res in context.Residents on dt.ResidentID equals res.ID
                                                   join stck in context.Stocks on im.StockID equals stck.ID
                                                   join med in context.Medicines on stck.MedicineID equals med.ID
                                                   where im.DateCreated >= startDate && im.DateCreated <= endDate
                                                   group im by im.DispenseTransaction into g
                                                   select new
                                                   {
                                                       Resident = g.Key.Resident.FirstName + " " + g.Key.Resident.MiddleName + " " + g.Key.Resident.LastName,
                                                       DateDispensed = g.Key.DateCreated,
                                                       UserID = g.Key.CreatedBy,
                                                       PrescriptionDescription = g.Key.PrescriptionDescription,
                                                       DispensedMedicines = g.Key.InventoryMovement.Select(me => new { MedicineName = me.Stock.Medicine.MedicineName, MedicineDescription = me.Stock.Medicine.Description, Quantity = me.Quantity * -1 }).ToList()
                                                   }
                                  )
                                  join finalUser in context.Users on finalDt.UserID equals finalUser.ID
                                  select new
                                  {
                                      Resident = finalDt.Resident,
                                      DateDispensed = finalDt.DateDispensed,
                                      User = finalUser.FirstName + " " + finalUser.MiddleName + " " + finalUser.LastName,
                                      PrescriptionDescription = finalDt.PrescriptionDescription,
                                      DispensedMedicines = finalDt.DispensedMedicines
                                  }).ToList();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = Report
                    });
                }

                if (rm.ReportType == "Certificate of Indigency Report")
                {
                    var Report = (from it in context.IndigencyTransactions
                                  join u in context.Users on it.CreatedBy equals u.ID
                                  join res in context.Residents on it.ResidentID equals res.ID
                                  where it.DateCreated >= startDate && it.DateCreated <= endDate
                                  select new
                                  {
                                      ID = it.ID,
                                      DateCreated = it.DateCreated,
                                      PrintedBy = u.FirstName + " " + u.MiddleName + " " + u.LastName,
                                      Purpose = it.Purpose,
                                      ControlNo = it.ControlNo,
                                      Resident = res.FirstName + " " + res.MiddleName + " " + res.LastName
                                  }).ToList();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = Report
                    });
                }

                if (rm.ReportType == "Barangay Clearance Report")
                {
                    var Report = (from it in context.BarangayClearanceTransactions
                                  join u in context.Users on it.CreatedBy equals u.ID
                                  join res in context.Residents on it.ResidentID equals res.ID
                                  where it.DateCreated >= startDate && it.DateCreated <= endDate
                                  select new
                                  {
                                      ID = it.ID,
                                      DateCreated = it.DateCreated,
                                      PrintedBy = u.FirstName + " " + u.MiddleName + " " + u.LastName,
                                      Purpose = it.Purpose,
                                      ControlNo = it.ControlNo,
                                      Resident = res.FirstName + " " + res.MiddleName + " " + res.LastName
                                  }).ToList();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = Report
                    });
                }

                if (rm.ReportType == "Business Clearance Report")
                {
                    var Report = (from bc in context.BusinessClearanceTransactions
                                  join u in context.Users on bc.CreatedBy equals u.ID
                                  join business in context.Businesses on bc.BusinessID equals business.ID
                                  join owner in context.Owners on business.OwnerID equals owner.ID
                                  where bc.DateCreated >= startDate && bc.DateCreated <= endDate
                                  select new
                                  {
                                      ID = bc.ID,
                                      DateCreated = bc.DateCreated,
                                      PrintedBy = u.FirstName + " " + u.MiddleName + " " + u.LastName,
                                      ControlNo = bc.ControlNo,
                                      Owner = owner.FirstName + " " + owner.MiddleName + " " + owner.LastName,
                                      Business = business.BusinessName
                                  }).ToList();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = Report
                    });
                }


                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched"
                });
            }
        }

        public class ReportModel
        {
            public string ReportType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}