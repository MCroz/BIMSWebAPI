using BIMSWebAPI.App_Code;
using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    [Authorize]
    public class MedicinesController : ApiController
    {
        //[AllowAnonymous]
        [Route("api/Medicines/GetMedicineList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMedicineList()
        {
            List<Medicine> medicines = new List<Medicine>();
            using (var context = new BimsContext())
            {
                medicines = context.Medicines.ToList();
            }

            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Fetched",
                data = medicines
            });
        }

        //[AllowAnonymous]
        [Route("api/Medicines/AddMedicine")]
        [HttpPost]
        public async Task<IHttpActionResult> AddMedicine(Medicine medicine)
        {
            using (var context = new BimsContext())
            {
                context.Medicines.Add(medicine);

                //For Logging
                var currentUser = context.Users.Find(medicine.ModifiedBy);
                string logChanges = "";
                foreach (PropertyInfo pi in medicine.GetType().GetProperties())
                {
                    //pi.GetValue(myClass, null)?.ToString();
                    if (pi.Name != "ModifiedBy" && pi.Name != "CreatedBy" && pi.Name != "DateModified" && pi.Name != "DateCreated" && pi.Name != "ID")
                    {
                        if (logChanges != "")
                        {
                            logChanges += ", ";
                        }
                        logChanges += pi.GetValue(medicine, null)?.ToString() != "" ? pi.Name + " = " + pi.GetValue(medicine, null)?.ToString() : "";
                    }
                }
                string changes = currentUser.Username + " Added A Medicine and Set: " + logChanges;
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
        [Route("api/Medicines/DeleteMedicine/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteMedicine(int id)
        {
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var medicine = context.Medicines.Find(id);
                if (medicine == null)
                {
                    message = "No Medicine Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    context.Medicines.Remove(medicine);
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
        [Route("api/Medicines/GetMedicineInfo/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMedicineInfo(int id)
        {
            string message;
            ResponseStatus resp;
            Medicine medicine;
            using (var context = new BimsContext())
            {
                medicine = context.Medicines.Find(id);
                if (medicine == null)
                {
                    message = "No Medicine Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    message = "Successfully Fetched";
                    resp = ResponseStatus.Success;
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message,
                data = medicine
            });
        }

        //[AllowAnonymous]
        [Route("api/Medicines/UpdateMedicine")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateMedicine(Medicine medicine)
        {
            ResponseStatus thisStatus;
            string message;
            using (var context = new BimsContext())
            {
                Medicine selectedMedicine = context.Medicines.Find(medicine.ID);
                if (selectedMedicine != null)
                {
                    selectedMedicine.MedicineName = medicine.MedicineName;
                    selectedMedicine.Description = medicine.Description;

                    var currentUser = context.Users.Find(medicine.ModifiedBy);
                    string changes = currentUser.Username + " Updated Medicine: " + AuditLogHelper.FetchChanges(context);
                    AuditLogHelper.GenerateLog(context, "Update", changes);

                    context.SaveChanges();
                    message = "Successfully Updated";
                    thisStatus = ResponseStatus.Success;
                }
                else
                {
                    thisStatus = ResponseStatus.Fail;
                    message = "No Medicine Found";
                }
            }

            return Ok(new ResponseModel()
            {
                status = thisStatus,
                message = message
            });
        }

        //[AllowAnonymous]
        [Route("api/Medicines/GetMedicinesListDropdown")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMedicinesListDropdown()
        {
            using (var context = new BimsContext())
            {
                var temp = context.Medicines.AsNoTracking().Select(x => new { ID = x.ID, MedicineName = x.MedicineName, Description = x.Description }).ToList();

                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = temp
                });
            }
        }
    }
}