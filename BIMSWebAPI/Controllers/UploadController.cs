using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BIMSWebAPI.Models;

namespace BIMSWebAPI.Controllers
{
    [AllowAnonymous]
    public class UploadController : ApiController
    {
        //[AllowAnonymous]
        [Route("api/Upload/UploadImages1")]
        [HttpPost]
        //public async Task<HttpResponseMessage> UploadImages()
        public async Task<IHttpActionResult> UploadImages1()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Ok(new ResponseModel {
                    status= ResponseStatus.Fail,
                    message = "Invalid Form Data."
                });
                //throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/Images/ResidentImages");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                }
                return Ok();
                //return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Ok(new ResponseModel
                {
                    status = ResponseStatus.Fail,
                    message = "Invalid Form Data."
                });
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            //return Ok();
        }

        //[AllowAnonymous]
        [Route("api/Upload/UploadImages")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadImages()
        {
            string newFilename = "";
            string serverFileName = "";
            //Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 20; //Size = 1 MB  

                        //IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            return Ok(new ResponseModel
                            {
                                status = ResponseStatus.Fail,
                                message = "Please Upload image of type .jpg,.gif,.png."
                            });
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            //var message = string.Format("Please Upload a file upto 1 mb.");
                            return Ok(new ResponseModel
                            {
                                status = ResponseStatus.Fail,
                                message = "Please Upload a file upto 20 mb."
                            });
                        }
                        else
                        {
                            //var filePath = HttpContext.Current.Server.MapPath("~/Images/ResidentImages/" + postedFile.FileName);
                            newFilename = GenerateNewFileName() + extension;
                            var filePath = HttpContext.Current.Server.MapPath("~/Images/ResidentImages/" + newFilename);
                            postedFile.SaveAs(filePath);
                            //serverFileName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Images/ResidentImages/" + newFilename;
                        }
                    }
                    return Ok(new ResponseModel
                    {
                        status = ResponseStatus.Success,
                        message = "Images Successfully Uploaded",
                        data = newFilename
                    });
                }
                return Ok(new ResponseModel
                {
                    status = ResponseStatus.Fail,
                    message = "Please Select an Image"
                });
            }
            catch (Exception ex)
            {
                //var res = string.Format("some Message");
                //dict.Add("error", res);
                //return Request.CreateResponse(HttpStatusCode.NotFound, dict);
                return Ok(new ResponseModel {
                    status = ResponseStatus.Fail,
                    message = ex.Message
                });
            }
        }

        private string GenerateNewFileName(string prefix = "IMG")
        {
            return prefix + "_" + DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
        }

        //[AllowAnonymous]
        [Route("api/Upload/UploadOwnerImage")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadOwnerImage()
        {
            string newFilename = "";
            //Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 20; //Size = 1 MB  

                        //IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            return Ok(new ResponseModel
                            {
                                status = ResponseStatus.Fail,
                                message = "Please Upload image of type .jpg,.gif,.png."
                            });
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            //var message = string.Format("Please Upload a file upto 1 mb.");
                            return Ok(new ResponseModel
                            {
                                status = ResponseStatus.Fail,
                                message = "Please Upload a file upto 20 mb."
                            });
                        }
                        else
                        {
                            //var filePath = HttpContext.Current.Server.MapPath("~/Images/ResidentImages/" + postedFile.FileName);
                            newFilename = GenerateNewFileName() + extension;
                            var filePath = HttpContext.Current.Server.MapPath("~/Images/OwnerImages/" + newFilename);
                            postedFile.SaveAs(filePath);
                        }
                    }

                    return Ok(new ResponseModel
                    {
                        status = ResponseStatus.Success,
                        message = "Images Successfully Uploaded",
                        data = newFilename
                    });
                }
                return Ok(new ResponseModel
                {
                    status = ResponseStatus.Fail,
                    message = "Please Select an Image"
                });
            }
            catch (Exception ex)
            {
                //var res = string.Format("some Message");
                //dict.Add("error", res);
                //return Request.CreateResponse(HttpStatusCode.NotFound, dict);
                return Ok(new ResponseModel
                {
                    status = ResponseStatus.Fail,
                    message = ex.Message
                });
            }
        }
    }
}