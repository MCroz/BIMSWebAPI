using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    public class PrintController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Print/InitialPrint")]
        [HttpPost]
        public async Task<IHttpActionResult> InitialPrint(PrintModel printModel)
        {
            Resident resident;
            BarangayClearanceTransaction bc;
            IndigencyTransaction it;
            using (var context = new BimsContext())
            {
                resident = await context.Residents.FindAsync(printModel.ResidentID);
                if (resident == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Resident Found"
                    });
                }
                else
                {
                    //Create a new Transaction
                    //User user = context.Users.Find(printModel.ProcessedByID);
                    if (printModel.CertificateType == "Barangay Clearance")
                    {
                        int newCount = context.BarangayClearanceTransactions.AsNoTracking().Where(b => b.DateCreated.Value.Year == DateTime.Now.Year).Count();
                        newCount += 1;
                        string controlNo = newCount.ToString().PadLeft(4, '0');
                        string finalControlNo = controlNo + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-DDL";
                        bc = new BarangayClearanceTransaction
                        {
                            ResidentID = printModel.ResidentID,
                            Resident = resident,
                            Purpose = printModel.Purpose,
                            ControlNo = finalControlNo,
                            CreatedBy = printModel.ProcessedByID,
                            ModifiedBy = printModel.ProcessedByID
                        };
                        context.BarangayClearanceTransactions.Add(bc);
                        context.SaveChanges();

                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Success,
                            message = "Successfully Created a New Transaction",
                            data = bc
                        });
                    }
                    else
                    {
                        int newCount = context.IndigencyTransactions.AsNoTracking().Where(b => b.DateCreated.Value.Year == DateTime.Now.Year).Count();
                        newCount += 1;
                        string controlNo = newCount.ToString().PadLeft(4, '0');
                        string finalControlNo = controlNo + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-CJRT";
                        it = new IndigencyTransaction
                        {
                            ResidentID = printModel.ResidentID,
                            Resident = resident,
                            Purpose = printModel.Purpose,
                            ControlNo = finalControlNo,
                            CreatedBy = printModel.ProcessedByID,
                            ModifiedBy = printModel.ProcessedByID
                        };
                        context.IndigencyTransactions.Add(it);
                        context.SaveChanges();

                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Success,
                            message = "Successfully Created a New Transaction",
                            data = it
                        });
                    }

                }
            }
        }


        public class PrintModel
        {
            public int ResidentID { get; set; }
            public string Purpose { get; set; }
            public int ProcessedByID { get; set; }
            public string CertificateType { get; set; }
        }
    }
}