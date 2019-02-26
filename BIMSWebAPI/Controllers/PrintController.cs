﻿using BIMSWebAPI.Models;
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
    [AllowAnonymous]
    public class PrintController : ApiController
    {
        //[AllowAnonymous]
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
                    //Fetch who Create it
                    var processingUser = context.Users.Find(printModel.ProcessedByID);
                    string appendingControlNo = BimsHelper.ExtractInitialsFromName(processingUser.FirstName + " " + processingUser.MiddleName + " " + processingUser.LastName);

                    if (printModel.CertificateType == "Barangay Clearance")
                    {
                        int newCount = context.BarangayClearanceTransactions.AsNoTracking().Where(b => b.DateCreated.Value.Year == DateTime.Now.Year).Count();
                        newCount += 1;
                        string controlNo = newCount.ToString().PadLeft(5, '0');
                        string finalControlNo = controlNo + "-C-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + appendingControlNo;
                        bc = new BarangayClearanceTransaction
                        {
                            ResidentID = printModel.ResidentID,
                            Resident = resident,
                            Purpose = printModel.Purpose,
                            ControlNo = finalControlNo,
                            CreatedBy = printModel.ProcessedByID,
                            ModifiedBy = printModel.ProcessedByID,
                            FullAddress = resident.AddressNo + " " + resident.AddressSt + " " + resident.AddressZone,
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
                        string controlNo = newCount.ToString().PadLeft(5, '0');
                        string finalControlNo = controlNo + "-I-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + appendingControlNo;
                        it = new IndigencyTransaction
                        {
                            ResidentID = printModel.ResidentID,
                            Resident = resident,
                            Purpose = printModel.Purpose,
                            ControlNo = finalControlNo,
                            CreatedBy = printModel.ProcessedByID,
                            ModifiedBy = printModel.ProcessedByID,
                            FullAddress = resident.AddressNo + " " + resident.AddressSt + " " + resident.AddressZone,
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

        //[AllowAnonymous]
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

        //[AllowAnonymous]
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

        //[AllowAnonymous]
        [Route("api/Print/GenerateBusinessClearanceTransaction")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateBusinessClearanceTransaction(PrintBusinessClearanceModel printModel)
        {
            using (var context = new BimsContext())
            {
                var business = (from bus in context.Businesses
                                join owner in context.Owners on bus.OwnerID equals owner.ID
                                where bus.ID == printModel.BusinessID
                                select new
                                {
                                    bus.KindOfBusiness,
                                    bus.ID,
                                    bus.BusinessAddress,
                                    bus.BusinessName,
                                    bus.BusinessContactNo,
                                    bus.FloorArea,
                                    bus.DTI_SEC_RegNo,
                                    bus.Capitalization,
                                    Owner = owner
                                    //OwnerName = owner.FirstName + " " + owner.MiddleName + " " + owner.LastName,
                                    //OwnerAddress = owner.Address,
                                    //OwnerContactNo = owner.ContactNo
                                }).FirstOrDefault();
                if (business == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Business Found"
                    });
                }
                else
                {
                    //Create a new Transaction
                    //Fetch who Create it
                    var processingUser = context.Users.Find(printModel.ProcessedByID);
                    string appendingControlNo = BimsHelper.ExtractInitialsFromName(processingUser.FirstName + " " + processingUser.MiddleName + " " + processingUser.LastName);

                    int newCount = context.BusinessClearanceTransactions.AsNoTracking().Where(b => b.DateCreated.Value.Year == DateTime.Now.Year).Count();
                    newCount += 1;
                    string controlNo = newCount.ToString().PadLeft(5, '0');
                    string finalControlNo = controlNo + "-BC-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + "-" + appendingControlNo;

                    var thisBusiness = context.Businesses.Find(business.ID);
                    var bc = new BusinessClearance
                    {
                        BusinessName = business.BusinessName,
                        BusinessAddress = business.BusinessAddress,
                        BusinessContactNo = business.BusinessContactNo,
                        FloorArea = business.FloorArea,
                        Capitalization = business.Capitalization,
                        DTI_SEC_RegNo = business.DTI_SEC_RegNo,
                        KindOfBusiness = business.KindOfBusiness,
                        ControlNo = finalControlNo,
                        Business = thisBusiness,
                        OwnerFullName = business.Owner.FirstName + " " + business.Owner.MiddleName + " " + business.Owner.LastName,
                        OwnerAddress = business.Owner.Address,
                        OwnerContactNo = business.Owner.ContactNo,
                        CreatedBy = printModel.ProcessedByID,
                        ModifiedBy = printModel.ProcessedByID
                    };
                    context.BusinessClearanceTransactions.Add(bc);
                    context.SaveChanges();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Created a New Transaction",
                        data = new 
                        {
                            BusinessName = business.BusinessName,
                            BusinessAddress = business.BusinessAddress,
                            BusinessContactNo = business.BusinessContactNo,
                            FloorArea = business.FloorArea,
                            Capitalization = business.Capitalization,
                            DTI_SEC_RegNo = business.DTI_SEC_RegNo,
                            KindOfBusiness = business.KindOfBusiness,
                            ControlNo = finalControlNo,
                            OwnerFullName = business.Owner.FirstName + " " + business.Owner.MiddleName + " " + business.Owner.LastName,
                            OwnerAddress = business.Owner.Address,
                            OwnerContactNo = business.Owner.ContactNo,
                            OwnerImage = business.Owner.Image
                        }
                    });
                }
            }
        }

        //[AllowAnonymous]
        [Route("api/Print/GetBusinessPrintHistory/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBusinessPrintHistory(int id)
        {
            using (var context = new BimsContext())
            {
                var business = await context.Businesses.FindAsync(id);
                if (business == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Business Found"
                    });
                }
                else
                {
                    var list = (from bc in context.BusinessClearanceTransactions
                                join u in context.Users on bc.CreatedBy equals u.ID
                                where bc.BusinessID == id
                                select new
                                {
                                    ID = bc.ID,
                                    DateCreated = bc.DateCreated,
                                    PrintedBy = u.FirstName + " " + u.MiddleName + " " + u.LastName,
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

        public class PrintBusinessClearanceModel
        {
            public int BusinessID { get; set; }
            public int ProcessedByID { get; set; }
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