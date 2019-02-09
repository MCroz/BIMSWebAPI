using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using BIMSWebAPI.App_Code;

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
                    //Fetch who Create it
                    var processingUser = context.Users.Find(printModel.ProcessedByID);
                    string appendingControlNo = BimsHelper.ExtractInitialsFromName(processingUser.FirstName + " " + processingUser.MiddleName + " " + processingUser.LastName);

                    if (printModel.CertificateType == "Barangay Clearance")
                    {
                        int newCount = context.BarangayClearanceTransactions.AsNoTracking().Where(b => b.DateCreated.Value.Year == DateTime.Now.Year).Count();
                        newCount += 1;
                        string controlNo = newCount.ToString().PadLeft(4, '0');
                        string finalControlNo = controlNo + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + appendingControlNo;
                        bc = new BarangayClearanceTransaction
                        {
                            ResidentID = printModel.ResidentID,
                            Resident = resident,
                            Purpose = printModel.Purpose,
                            ControlNo = finalControlNo,
                            CreatedBy = printModel.ProcessedByID,
                            ModifiedBy = printModel.ProcessedByID,
                            FullAddress = resident.AddressNo + " " + resident.AddressSt,
                            FullName = resident.FirstName + " " + resident.MiddleName + " " + resident.LastName,
                            Image = resident.Image
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
                        string finalControlNo = controlNo + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + appendingControlNo;
                        it = new IndigencyTransaction
                        {
                            ResidentID = printModel.ResidentID,
                            Resident = resident,
                            Purpose = printModel.Purpose,
                            ControlNo = finalControlNo,
                            CreatedBy = printModel.ProcessedByID,
                            ModifiedBy = printModel.ProcessedByID,
                            FullAddress = resident.AddressNo + " " + resident.AddressSt,
                            FullName = resident.FirstName + " " + resident.MiddleName + " " + resident.LastName,
                            Image = resident.Image
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

        [AllowAnonymous]
        [Route("api/Print/GetUserIndigencyTransactions/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserIndigencyTransactions(int id)
        {
            using (var context = new BimsContext())
            {
                var resident = await context.Residents.FindAsync(id);
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
                    var list = (from it in context.IndigencyTransactions
                                join u in context.Users on it.CreatedBy equals u.ID
                                where it.ResidentID == id
                                select new
                                {
                                    ID = it.ID,
                                    DateCreated = it.DateCreated,
                                    PrintedBy = u.FirstName + " " + u.MiddleName + " " + u.LastName,
                                    Purpose = it.Purpose,
                                    ControlNo = it.ControlNo
                                }).ToList();
                    
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = list
                    });
                }
            }
        }

        [AllowAnonymous]
        [Route("api/Print/GetUserBarangayClearanceTransactions/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBarangayClearanceTransactions(int id)
        {
            using (var context = new BimsContext())
            {
                var resident = await context.Residents.FindAsync(id);
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
                    var list = (from bc in context.BarangayClearanceTransactions
                                join u in context.Users on bc.CreatedBy equals u.ID
                                where bc.ResidentID == id
                                select new
                                {
                                    ID = bc.ID,
                                    DateCreated = bc.DateCreated,
                                    PrintedBy = u.FirstName + " " + u.MiddleName + " " + u.LastName,
                                    Purpose = bc.Purpose,
                                    ControlNo = bc.ControlNo
                                }).ToList();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Fetched",
                        data = list
                    });
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