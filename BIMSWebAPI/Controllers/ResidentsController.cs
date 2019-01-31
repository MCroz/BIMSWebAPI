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
    public class ResidentsController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Residents/GetResidentList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetResidentList()
        {
            List<Resident> residents = new List<Resident>();
            using (var context = new BimsContext())
            {
                residents = context.Residents.ToList();
            }

            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Fetched",
                data = residents
            });
        }

        [AllowAnonymous]
        [Route("api/Residents/AddResident")]
        [HttpPost]
        public async Task<IHttpActionResult> AddResident(Resident resident)
        {
            using (var context = new BimsContext())
            {
                context.Residents.Add(resident);
                context.SaveChanges();
            }
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Added"
            });
        }

        [AllowAnonymous]
        [Route("api/Residents/DeleteResident/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteResident(int id)
        {
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var resident = context.Residents.Find(id);
                if (resident == null)
                {
                    message = "No Resident Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    context.Residents.Remove(resident);
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

        [AllowAnonymous]
        [Route("api/Residents/GetResidentInfo/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetResidentInfo(int id)
        {
            string message;
            ResponseStatus resp;
            Resident resident;
            using (var context = new BimsContext())
            {
                resident = context.Residents.Find(id);
                if (resident == null)
                {
                    message = "No Resident Found";
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
                data = resident
            });
        }

        [AllowAnonymous]
        [Route("api/Residents/UpdateResident")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateResident(Resident resident)
        {
            ResponseStatus thisStatus;
            string message;
            using (var context = new BimsContext())
            {
                Resident selectedResident = context.Residents.Find(resident.ID);
                if (selectedResident != null)
                {
                    selectedResident.FirstName = resident.FirstName;
                    selectedResident.MiddleName = resident.MiddleName;
                    selectedResident.LastName = resident.LastName;
                    selectedResident.Gender = resident.Gender;
                    selectedResident.CivilStatus = resident.CivilStatus;
                    selectedResident.BirthDate = resident.BirthDate;
                    selectedResident.AddressNo = resident.AddressNo;
                    selectedResident.AddressSt = resident.AddressSt;
                    selectedResident.AddressZone = resident.AddressZone;
                    selectedResident.BirthPlace = resident.BirthPlace;
                    selectedResident.Citizenship = resident.Citizenship;
                    selectedResident.ModifiedBy = resident.ModifiedBy;
                    context.SaveChanges();
                    message = "Successfully Updated";
                    thisStatus = ResponseStatus.Success;
                }
                else
                {
                    thisStatus = ResponseStatus.Fail;
                    message = "No Resident Found";
                }
            }

            return Ok(new ResponseModel()
            {
                status = thisStatus,
                message = message
            });
        }

        [AllowAnonymous]
        [Route("api/Residents/SearchResident")]
        [HttpPost]
        public async Task<IHttpActionResult> SearchResident(SearchModel search)
        {
            using (var context = new BimsContext())
            {
                List<Resident> query;
                if (search.FilterType == "Zone")
                {
                    query = (from r in context.Residents
                             where r.AddressZone.Contains(search.SearchString)
                             select r).ToList();
                }
                else if (search.FilterType == "Name")
                {
                    query = (from r in context.Residents
                             where r.MiddleName.Contains(search.SearchString) || r.LastName.Contains(search.SearchString)
                             || r.FirstName.Contains(search.SearchString)
                             select r).ToList();
                }
                else
                {
                    query = (from r in context.Residents
                             where r.MiddleName.Contains(search.SearchString) || r.LastName.Contains(search.SearchString)
                             || r.FirstName.Contains(search.SearchString) || r.AddressZone.Contains(search.SearchString)
                             select r).ToList();
                }


                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Added",
                    data = query
                });
            }

        }

        public class SearchModel {
            public string SearchString { get; set; }
            public string FilterType { get; set; }
        }
    }
}