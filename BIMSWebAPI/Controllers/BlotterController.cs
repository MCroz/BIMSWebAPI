using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    [AllowAnonymous]
    public class BlotterController : ApiController
    {

        [Route("api/Blotter/AddBlotter")]
        [HttpPost]
        public async Task<IHttpActionResult> AddNewBlotter(Blotter blot)
        {
            using (var context = new BimsContext())
            {
                context.Blotters.Add(blot);
                context.SaveChanges();
            }

            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Fetched"
            });
        }


        //[AllowAnonymous]
        [Route("api/Blotter/GetBlotterList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBlotterList()
        {
            using (var context = new BimsContext())
            {
                var outVal = (from blot in context.Blotters
                              join accused in context.Accuseds on blot.AccusedID equals accused.ID
                              join complainant in context.Complainants on blot.ComplainantID equals complainant.ID
                              join accusedRes in context.Residents on accused.ResidentID equals accusedRes.ID into accusedResident
                              join complainantRes in context.Residents on complainant.ResidentID equals complainantRes.ID into complainantResident
                              select new
                              {
                                  ID = blot.ID,
                                  AddressNo = blot.AddressNo,
                                  AddressSt = blot.AddressSt,
                                  AddressZone = blot.AddressZone,
                                  NarrativeReport = blot.NarrativeReport,
                                  Status = blot.Status,
                                  TypeOfIncident = blot.TypeOfIncident,
                                  IncidentDateTime = blot.IncidentDateTime,
                                  ComplainantName = blot.Complainant.ResidentID == null ? blot.Complainant.FirstName + " " + blot.Complainant.MiddleName + " " + blot.Complainant.LastName : blot.Complainant.Resident.FirstName + " " + blot.Complainant.Resident.MiddleName + " " + blot.Complainant.Resident.LastName,
                                  AccusedName = blot.Accused.ResidentID == null ? blot.Accused.FirstName + " " + blot.Accused.MiddleName + " " + blot.Accused.LastName : blot.Accused.Resident.FirstName + " " + blot.Accused.Resident.MiddleName + " " + blot.Accused.Resident.LastName
                              }).ToList();
                //var outVal2 = (from blot in context.Blotters select blot).ToList();
                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = outVal
                });
            }
        }

        [Route("api/Blotter/GetBlotterInfo/{blotterID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBlotterInfo(int blotterID)
        {
            using (var context = new BimsContext())
            {
                var outVal = context.Blotters.Find(blotterID);
                Complainant comp = context.Complainants.Find(outVal.ComplainantID);
                Accused acc = context.Accuseds.Find(outVal.AccusedID);
                outVal.Accused = acc;
                outVal.Complainant = comp;

                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = outVal
                });
            }

        }


        [Route("api/Blotter/UpdateBlotter")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBlotter(Blotter blot)
        {
            using (var context = new BimsContext())
            {
                var blotter = context.Blotters.Find(blot.ID);
                if (blotter == null)
                {
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Fail,
                        message = "No Blotter Found"
                    });
                }
                else
                {
                    var acc = context.Accuseds.Find(blot.Accused.ID);
                    var comp = context.Complainants.Find(blot.Complainant.ID);

                    acc.Address = blot.Accused.Address;
                    acc.FirstName = blot.Accused.FirstName;
                    acc.MiddleName = blot.Accused.MiddleName;
                    acc.LastName = blot.Accused.LastName;
                    acc.Gender = blot.Accused.Gender;
                    acc.BirthDate = blot.Accused.BirthDate;
                    acc.ResidentID = blot.Accused.ResidentID;

                    comp.Address = blot.Complainant.Address;
                    comp.FirstName = blot.Complainant.FirstName;
                    comp.MiddleName = blot.Complainant.MiddleName;
                    comp.LastName = blot.Complainant.LastName;
                    comp.Gender = blot.Complainant.Gender;
                    comp.BirthDate = blot.Complainant.BirthDate;
                    comp.ResidentID = blot.Complainant.ResidentID;

                    //blotter.AccusedID = blot.Accused.ID;
                    //blotter.ComplainantID = blot.Complainant.ID;
                    blotter.IncidentDateTime = blot.IncidentDateTime;
                    blotter.AddressNo = blot.AddressNo;
                    blotter.AddressSt = blot.AddressSt;
                    blotter.AddressZone = blot.AddressZone;
                    blotter.NarrativeReport = blot.NarrativeReport;
                    blotter.Status = blot.Status;
                    blotter.Complainant = comp;
                    blotter.Accused = acc;
                    context.SaveChanges();

                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Updated"
                    });
                }
            }
        }
    }
}