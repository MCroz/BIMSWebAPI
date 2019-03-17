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
using IronPdf;
using System.Web;
using Spire.Doc;
using System.Drawing;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

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

                        string filename = "BCC_" + DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
                        Dictionary<string, string> myReplacements = new Dictionary<string, string>();
                        myReplacements.Add("{{FullName}}", resident.FirstName + ' ' + resident.MiddleName + ' ' + resident.LastName);
                        Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/Images/ResidentImages/" + resident.Image));
                        string templatePath = "~/PDFS/Templates/BarangayClearance.docx";
                        string newFilename = GeneratePDF(templatePath, myReplacements, image, filename, 153.6f, 150.72f);

                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Success,
                            message = "Successfully Created a New Transaction",
                            //data = bc
                            data = newFilename + ".pdf"
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

                        string filename = "IC_" + DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
                        Dictionary<string, string> myReplacements = new Dictionary<string, string>();
                        myReplacements.Add("{{FullName}}", resident.FirstName + ' ' + resident.MiddleName + ' ' + resident.LastName);
                        Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/Images/ResidentImages/" + resident.Image));
                        string templatePath = "~/PDFS/Templates/Indigency.docx";
                        string newFilename = GeneratePDF(templatePath, myReplacements, image, filename, 153.6f, 150.72f);

                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Success,
                            message = "Successfully Created a New Transaction",
                            //data = it
                            data = newFilename + ".pdf"
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

                    //Start Generating
                    string filename = "BC_" + DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
                    Dictionary<string, string> myReplacements = new Dictionary<string, string>();
                    myReplacements.Add("{{OwnerFullName}}", bc.OwnerFullName);
                    myReplacements.Add("{{Date}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                    myReplacements.Add("{{OwnerAddress}}", bc.OwnerAddress);
                    myReplacements.Add("{{OwnerContactNo}}", bc.OwnerContactNo);
                    myReplacements.Add("{{BusinessName}}", bc.BusinessName);
                    myReplacements.Add("{{BusinessContactNo}}", bc.BusinessContactNo);
                    myReplacements.Add("{{BusinessAddress}}", bc.BusinessAddress);
                    myReplacements.Add("{{FloorArea}}", bc.FloorArea);
                    myReplacements.Add("{{DTI_SEC_RegNo}}", bc.DTI_SEC_RegNo);
                    myReplacements.Add("{{KindOfBusiness}}", bc.KindOfBusiness);
                    myReplacements.Add("{{Capitalization}}", bc.Capitalization.ToString("C").Replace('$', '₱'));
                    myReplacements.Add("{{Year}}", DateTime.Now.Year.ToString());
                    myReplacements.Add("{{ControlNo}}", bc.ControlNo);
                    Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/Images/OwnerImages/" + business.Owner.Image));
                    string templatePath = "~/PDFS/Templates/BusinessClearance.docx";
                    string newFilename = GeneratePDF(templatePath,myReplacements, image,filename,113.04f, 115.2f);

                    return Ok(new ResponseModel() {
                        status = ResponseStatus.Success,
                        message = "Successfully Created a New Transaction",
                        data = newFilename + ".pdf"
                    });
                    //return Ok(new ResponseModel()
                    //{
                    //    status = ResponseStatus.Success,
                    //    message = "Successfully Created a New Transaction",
                    //    data = new 
                    //    {
                    //        BusinessName = business.BusinessName,
                    //        BusinessAddress = business.BusinessAddress,
                    //        BusinessContactNo = business.BusinessContactNo,
                    //        FloorArea = business.FloorArea,
                    //        Capitalization = business.Capitalization,
                    //        DTI_SEC_RegNo = business.DTI_SEC_RegNo,
                    //        KindOfBusiness = business.KindOfBusiness,
                    //        ControlNo = finalControlNo,
                    //        OwnerFullName = business.Owner.FirstName + " " + business.Owner.MiddleName + " " + business.Owner.LastName,
                    //        OwnerAddress = business.Owner.Address,
                    //        OwnerContactNo = business.Owner.ContactNo,
                    //        OwnerImage = business.Owner.Image
                    //    }
                    //});
                }
            }
        }

        public string GeneratePDF(string templatePath,Dictionary<string,string> replacements, Image img, string FileName, float imgH, float imgW)
        {
            Document document = new Document();
            //document.LoadFromFile(HttpContext.Current.Server.MapPath("~/PDFS/Templates/BusinessClearance/BusinessClearance.docx"));
            document.LoadFromFile(HttpContext.Current.Server.MapPath(templatePath));

            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                document.Replace(replacement.Key, replacement.Value, false, true);
            }
            if (img != null)
            {
                TextSelection[] selections = document.FindAllString("{{Image}}", true, true);
                int index = 0;
                TextRange range = null;

                foreach (TextSelection selection in selections)
                {
                    DocPicture pic = new DocPicture(document);
                    pic.LoadImage(img);
                    pic.Height = imgH;
                    pic.Width = imgW;

                    range = selection.GetAsOneRange();
                    index = range.OwnerParagraph.ChildObjects.IndexOf(range);
                    range.OwnerParagraph.ChildObjects.Insert(index, pic);
                    range.OwnerParagraph.ChildObjects.Remove(range);
                }
            }
            //document.Replace("{{FullAddress}}", bc.OwnerAddress, false, true);

            
            //document.SaveToFile(HttpContext.Current.Server.MapPath("~/PDFS/Outputs/" + newFilename + ".docx"), FileFormat.Docx);

            document.SaveToFile(HttpContext.Current.Server.MapPath("~/PDFS/Outputs/" + FileName + ".pdf"), FileFormat.PDF);
            return FileName;
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