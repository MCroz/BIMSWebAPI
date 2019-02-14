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
    public class OwnerController : ApiController
    {
        //[AllowAnonymous]
        [Route("api/Owner/GetOwnerList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOwnerList()
        {
            using (var context = new BimsContext())
            {
                var outVal = context.Owners.Where(o => o.isRemoved == 0).ToList();



                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = outVal
                });
            }
        }

        //[AllowAnonymous]
        [Route("api/Owner/AddOwner")]
        [HttpPost]
        public async Task<IHttpActionResult> AddOwner(Owner owner)
        {
            using (var context = new BimsContext())
            {
                owner.isRemoved = 0;
                context.Owners.Add(owner);
                bool existingDTI = false;
                foreach (Business bus in owner.Businesses)
                {
                    bus.ModifiedBy = owner.CreatedBy;
                    bus.CreatedBy = owner.CreatedBy;
                    context.Businesses.Add(bus);
                    if (context.Businesses.Where(b => b.DTI_SEC_RegNo == bus.DTI_SEC_RegNo).ToList().Count() > 0)
                    {
                        existingDTI = true;
                    }
                }
                if (existingDTI)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "The Business You Are Trying to Add Has A Duplicate DTR/SEC Reg. No on the Database."
                    });
                }

                //For Logging
                var currentUser = context.Users.Find(owner.ModifiedBy);
                string logChanges = "";
                foreach (PropertyInfo pi in owner.GetType().GetProperties())
                {
                    //pi.GetValue(myClass, null)?.ToString();
                    if (pi.Name != "ModifiedBy" && pi.Name != "CreatedBy" && pi.Name != "DateModified" && pi.Name != "DateCreated" && pi.Name != "ID")
                    {
                        if (logChanges != "")
                        {
                            logChanges += ", ";
                        }
                        logChanges += pi.GetValue(owner, null)?.ToString() != "" ? pi.Name + " = " + pi.GetValue(owner, null)?.ToString() : "";
                    }
                }
                string changes = currentUser.Username + " Added A New Owner and Set: " + logChanges;
                AuditLogHelper.GenerateLog(context, "Create", changes);
                //For Logging

                context.SaveChanges();
            }
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Added"
            });
        }

        //[AllowAnonymous]
        [Route("api/Owner/GetOwnerInfo/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOwnerInfo(int id)
        {
            string message;
            ResponseStatus resp;
            Owner owner;
            using (var context = new BimsContext())
            {
                owner = context.Owners.Find(id);
                if (owner == null)
                {
                    message = "No Owner Found";
                    resp = ResponseStatus.Fail;
                    return Ok(new ResponseModel()
                    {
                        status = resp,
                        message = message
                    });
                }
                else
                {
                    var businesses = (from b in context.Businesses
                                      join o in context.Owners on b.OwnerID equals o.ID
                                      where b.OwnerID == owner.ID
                                      select new { b.BusinessAddress, b.BusinessName,
                                      b.BusinessContactNo, b.Capitalization, b.DTI_SEC_RegNo, b.ID,             b.KindOfBusiness,b.FloorArea}).ToList();
                    message = "Successfully Fetched";
                    resp = ResponseStatus.Success;
                    return Ok(new ResponseModel()
                    {
                        status = resp,
                        message = message,
                        data = new CustomOwnerResponse
                        {
                            Owner = owner,
                            OwnerBusiness = businesses
                        }
                    });
                }
            }

        }

        public class CustomOwnerResponse
        {
            public Owner Owner;
            public object OwnerBusiness;
        }

        //[AllowAnonymous]
        [Route("api/Owner/DeleteOwner/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteOwner(int id)
        {
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var owner = context.Owners.Find(id);
                if (owner == null)
                {
                    message = "No Owner Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    owner.isRemoved = 1;
                    context.SaveChanges();
                    message = "Successfully Removed";
                    resp = ResponseStatus.Success;
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message
            });
        }

        //[AllowAnonymous]
        [Route("api/Owner/UpdateOwner")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateOwner(Owner owner)
        {
            ResponseStatus thisStatus;
            string message;
            using (var context = new BimsContext())
            {
                Owner selectedOwner = context.Owners.Find(owner.ID);
                if (selectedOwner != null)
                {
                    selectedOwner.FirstName = owner.FirstName;
                    selectedOwner.MiddleName = owner.MiddleName;
                    selectedOwner.LastName = owner.LastName;
                    selectedOwner.ModifiedBy = owner.ModifiedBy;
                    selectedOwner.Image = owner.Image;
                    selectedOwner.ContactNo = owner.ContactNo;
                    selectedOwner.Address = owner.Address;

                    var currentUser = context.Users.Find(owner.ModifiedBy);
                    string changes = currentUser.Username + " Updated Owner: " + AuditLogHelper.FetchChanges(context);
                    AuditLogHelper.GenerateLog(context, "Update", changes);

                    bool isExistingDTI = false;
                    foreach (Business bus in owner.Businesses)
                    {
                        if (bus.ID != null && bus.ID != 0)
                        {
                            if (context.Businesses.Where(b => b.DTI_SEC_RegNo == bus.DTI_SEC_RegNo && b.ID != bus.ID).ToList().Count() > 0)
                            {
                                isExistingDTI = true;
                            }
                            else
                            {
                                var selectedBus = context.Businesses.Find(bus.ID);

                                //FieldInfo[] fi = selectedBus.GetType().GetFields();
                                //string logChanges = "";
                                //foreach (PropertyInfo f in bus.GetType().GetProperties())
                                //{
                                //    if (!f.GetValue(selectedBus).Equals(f.GetValue(bus)))
                                //    {
                                //        if (f.Name != "ModifiedBy" && f.Name != "CreatedBy" && f.Name != "DateModified" && f.Name != "DateCreated")
                                //        {
                                //            if (logChanges != "")
                                //            {
                                //                logChanges += ", ";
                                //            }
                                //            logChanges += f.GetValue(bus, null)?.ToString() != "" ? f.Name + " = " + f.GetValue(bus, null)?.ToString() : "";
                                //        }
                                //    }
                                //}
                                //string changes1 = currentUser.Username + " Updated Business and Set: " + logChanges;
                                //AuditLogHelper.GenerateLog(context, "Update", changes1);

                                //TODO Add Logging

                                selectedBus.BusinessName = bus.BusinessName;
                                selectedBus.BusinessAddress = bus.BusinessAddress;
                                selectedBus.BusinessContactNo = bus.BusinessContactNo;
                                selectedBus.KindOfBusiness = bus.KindOfBusiness;
                                selectedBus.FloorArea = bus.FloorArea;
                                selectedBus.Capitalization = bus.Capitalization;
                                selectedBus.DTI_SEC_RegNo = bus.DTI_SEC_RegNo;
                                selectedBus.ModifiedBy = owner.ModifiedBy;
                            }
                            
                        }
                        else
                        {
                            if (context.Businesses.Where(b => b.DTI_SEC_RegNo == bus.DTI_SEC_RegNo).ToList().Count() > 0)
                            {
                                isExistingDTI = true;
                            }
                            else
                            {
                                bus.ModifiedBy = owner.ModifiedBy;
                                bus.CreatedBy = owner.ModifiedBy;
                                bus.Owner = selectedOwner;
                                selectedOwner.Businesses.Add(bus);

                                //var curUser = context.Users.Find(owner.ModifiedBy);
                                //string logChanges = "";
                                //foreach (PropertyInfo pi in bus.GetType().GetProperties())
                                //{
                                //    //pi.GetValue(myClass, null)?.ToString();
                                //    if (pi.Name != "ModifiedBy" && pi.Name != "CreatedBy" && pi.Name != "DateModified" && pi.Name != "DateCreated" && pi.Name != "ID")
                                //    {
                                //        if (logChanges != "")
                                //        {
                                //            logChanges += ", ";
                                //        }
                                //        logChanges += pi.GetValue(bus, null)?.ToString() != "" ? pi.Name + " = " + pi.GetValue(bus, null)?.ToString() : "";
                                //    }
                                //}
                                //string changes1 = currentUser.Username + " Added A Business and Set: " + logChanges;
                                //AuditLogHelper.GenerateLog(context, "Create", changes1);
                            }
                        }
                    }
                    if (isExistingDTI)
                    {
                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Fail,
                            message = "The Business You Are Trying to Add Has A Duplicate DTR/SEC Reg. No. on the Database."
                        });
                    }

                    context.SaveChanges();
                    message = "Successfully Updated";
                    thisStatus = ResponseStatus.Success;
                }
                else
                {
                    thisStatus = ResponseStatus.Fail;
                    message = "No Owner Found";
                }
            }

            return Ok(new ResponseModel()
            {
                status = thisStatus,
                message = message
            });
        }
    }
}