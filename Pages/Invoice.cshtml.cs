using DocumentFormat.OpenXml.VariantTypes;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Zlib;
using RoofSafety.Data;
using RoofSafety.Models;
using System.IO;
using System.Net;
using System.Text;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Model.Identity;
using static RoofSafety.Pages.xeroredirModel;

namespace RoofSafety.Pages
{
    public class InvoiceModel : PageModel
    {
        private readonly dbcontext _context;
        public InvoiceModel(dbcontext context)
        {
            _context = context;
        }

        public Guid? XeroQuoteID { get; set; }
        public async Task<ActionResult> OnGet()
        {
            XeroQuoteID = await CreateXeroQuote();
          Stream? stream =  await GetPDFQuote(XeroQuoteID.ToString());
            if (stream==null)
                return Page();
            return File(stream, "application/pdf", "Quote.pdf");

            //
        }
        public async Task<Stream?> GetPDFQuote(string xeroQuoteID)
        {
            try
            {
                
                Token tkn;
                tkn = _context.token.FirstOrDefault();
                var accessToken = tkn.access_token;
                var TenantId = tkn.TenantID.Value;
                var refeshtoken = tkn.refresh_token;
                var DeveloperKey = tkn.client_id;
                var DeveloperSecret = tkn.client_secret;
                var res= await GetPDFQuoteFromXero(tkn.access_token,TenantId,xeroQuoteID);
                if (res.Error== "RefreshedToken")
                {
                    // accessToken = referesh();
                    Token tkref = RefreshAccessToken(refeshtoken, accessToken, DeveloperKey, DeveloperSecret);
                    /* todotim save back to db
                    Token tk = _context.token.FirstOrDefault();
                        tk.access_token = tkn.access_token;
                        ret = tkn.access_token;
                        //tk.DteTme = DateTime.UtcNow;
                        tk.expires_at = tkn.expires_at;
                        tk.jti = tkn.jti;
                        tk.refresh_token = tkn.refresh_token;
                        tk.scope = tkn.scope;
                        tk.token_type = tkn.token_type;
                        // tk.userId = tkn.userId;
                        //                    db.Tokens.InsertOnSubmit(tk);
                        _context.SaveChanges();
                    */

                    res = await GetPDFQuoteFromXero(tkref.access_token,  TenantId, xeroQuoteID);
                }
                return res.Result;
            }
            catch (Exception ex)
            {
                var Error = ex.Message;
            }
            return null;
        }
        public class ResultError
        {
            public Stream? Result { get; set; }
            public string? Error { get; set; }

            public string? accessToken { get; set; }
        }
        private async Task<ResultError> GetPDFQuoteFromXero(string accessToken, Guid TenantId,string XeroQuoteID)
        {
            ResultError ret = new ResultError();
            string uri = "https://api.xero.com/api.xro/2.0/Quotes/"+XeroQuoteID;//"{\r\n  \"object\": {\r\n    \"type\": \"CATEGORY\",\r\n    \"id\": \"3BYSN6PG4ASBQCSBQKKUAFXR\",\r\n    \"updated_at\": \"2024-05-04T04:02:33.851Z\",\r\n    \"created_at\": \"2023-09-20T03:31:36.81Z\",\r\n    \"version\": 1714795353851,\r\n    \"is_deleted\": false,\r\n    \"present_at_all_locations\": true,\r\n    \"category_data\": {\r\n      \"name\": \"Sue Shannon\",\r\n      \"category_type\": \"REGULAR_CATEGORY\",\r\n      \"parent_category\": {\r\n        \"ordinal\": -2250356704673792\r\n      },\r\n      \"is_top_level\": true,\r\n      \"online_visibility\": false\r\n    }\r\n  }\r\n}";
            //string jsn;
            //jsn = "{\r\n\"idempotency_key\": \"" + Guid.NewGuid().ToString() + "\",\r\n    \"object\": {\r\n            \"id\": \"#small_coffee\",\r\n\"type\": \"CATEGORY\",\r\n\"category_data\": {\r\n\"category_type\": \"REGULAR_CATEGORY\",\r\n\"name\": \"" + artistcat + "\" \r\n}\r\n}\r\n}";
            using (var cli = new HttpClient())
            {
                cli.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                cli.DefaultRequestHeaders.Add("Xero-Tenant-Id", $"{TenantId}");
                cli.DefaultRequestHeaders.Add("Accept", "application/pdf");
                //  var jsonContent = new StringContent(jsn, Encoding.UTF8, "application/json");
                var result = await cli.GetAsync(uri);
                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ret.Error = "RefreshedToken";
                    return ret;
                }
                else if(!result.IsSuccessStatusCode)
                {
                    ret.Error = result.ReasonPhrase;
                    return ret;
                }
                 
                var stream = await result.Content.ReadAsStreamAsync();
                //string path = Path.Combine("c:/temp", "test.pdf");

                //using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                //{
                //    stream.CopyTo(outputFileStream);
                //}

                ret.Result = stream;
                return ret;
            }
        }

        //public object ToPDF(Stream stream)
        //{
        //    using PdfSharpCore.Pdf;
        //    using System.IO;

        //    // Assuming you have the 'stream' object containing the PDF data
        //    // Create a new PDF document
        //    PdfDocument document = new PdfDocument();

        //    // Create a new page in the document
        //    PdfPage page = document.AddPage();

        //    // Create a PDF graphics object to draw on the page
        //    XGraphics gfx = XGraphics.FromPdfPage(page);

        //    // Create a PDF document object from the stream
        //    PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

        //    // Iterate through each page in the input document
        //    foreach (PdfPage inputPage in inputDocument.Pages)
        //    {
        //        // Create a new page in the output document
        //        PdfPage outputPage = document.AddPage();

        //        // Create a PDF graphics object for the output page
        //        XGraphics outputGfx = XGraphics.FromPdfPage(outputPage);

        //        // Draw the contents of the input page onto the output page
        //        outputGfx.DrawPdfPage(inputPage);
        //    }

        //    // Save the output document to a new stream
        //    MemoryStream outputStream = new MemoryStream();
        //    document.Save(outputStream);

        //    // Reset the position of the output stream to the beginning
        //    outputStream.Position = 0;

        //    // Now you can use the 'outputStream' to perform further operations with the PDF

        //    // For example, you can save the PDF to a file
        //    string outputPath = "path/to/output.pdf";
        //    using (FileStream outputFile = new FileStream(outputPath, FileMode.Create))
        //    {
        //        outputStream.CopyTo(outputFile);
        //    }

        //    // Remember to dispose the resources
        //    gfx.Dispose();
        //    document.Dispose();
        //    inputDocument.Dispose();
        //    outputStream.Dispose();
        //    ret.Result = stream;
        //    return ret;
        //}
        //public void SaveToOutput(Stream dataStream, HttpContext httpContext)
        //{
        //    dataStream.Seek(0, SeekOrigin.Begin);
        //    FileStream fileout =System.IO.File.Create("somepath/file.pdf");

        //    const int chunk = 512;
        //    byte[] buffer = new byte[512];

        //    int bytesread = dataStream.Read(buffer, 0, chunk);

        //    while (bytesread == chunk)
        //    {
        //        HttpContext.Response.Current.Response.OutputStream.Write(buffer, 0, chunk);
        //        fileout.Write(buffer, 0, chunk);
        //        bytesread = dataStream.Read(buffer, 0, chunk);
        //    }

        //    HttpContext.Current.Response.OutputStream.Write(buffer, 0, bytesread);
        //    fileout.Write(buffer, 0, bytesread);
        //    fileout.Close();

        //    HttpContext.Current.Response.ContentType = "application/pdf";
        //}
        public async Task<Guid?> CreateXeroQuote()
        { 
            int InspID = 998;
           var insp= _context.Inspection.Where(i => i.id == InspID).FirstOrDefault();
            int InvNo = insp.id;
            DateTime dteInv = insp.InspectionDate;
            decimal quoteamount = insp.Quote??0;
            var bd=_context.Building.Where(i => i.id == insp.BuildingID).FirstOrDefault();
            var ClientBuilding = bd.BuildingName;
            var contactXeroid = new Guid("eaa58665-c213-4805-a684-23d3cca1bd19");//thams new Guid("edceb899-5bb0-4253-955d-a998aa649ddf");//the owners of starta

            var apiInstance = new AccountingApi();// PayrollAuApi();
            string accessToken;
            Guid xeroTenantId;

            try
            {
                Token tkn;
                tkn = _context.token.FirstOrDefault();
                accessToken = tkn.access_token;
                xeroTenantId = tkn.TenantID.Value;
                 
                try
                {
                    Quotes sample = await apiInstance.GetQuoteAsync(accessToken, xeroTenantId.ToString(), Guid.Parse("26bac16d-a5fe-49f8-948a-e59c6443c341"));

                    Quotes invoices = await CreateQuote(ClientBuilding,quoteamount, InvNo, dteInv, contactXeroid, apiInstance, accessToken, xeroTenantId);

                    //                    Quotes result = await apiInstance.CreateInvoicesAsync(accessToken, xeroTenantId.ToString(), invoices);
                    Quotes result = await apiInstance.CreateQuotesAsync(accessToken, xeroTenantId.ToString(), invoices);
                    
                    // Guid? XeroID = result._Invoices.FirstOrDefault().InvoiceID;
                    Guid? XeroID = result._Quotes.FirstOrDefault().QuoteID;
                    RequestEmpty re = new RequestEmpty();
                    //await apiInstance.EmailInvoiceAsync(accessToken, xeroTenantId.ToString(), XeroID.Value, re);
                    return XeroID;
                }
                catch (Xero.NetStandard.OAuth2.Client.ApiException ex)
                {
                    try
                    {
                       // tkn = _context.token.FirstOrDefault();
                        accessToken = referesh();
                        // accessToken = tkn.access_token;
                       // xeroTenantId = tkn.TenantID.Value;

                        Quotes sample = await apiInstance.GetQuoteAsync(accessToken, xeroTenantId.ToString(), Guid.Parse("26bac16d-a5fe-49f8-948a-e59c6443c341"));

                        Quotes invoices = await CreateQuote(ClientBuilding,quoteamount, InvNo,dteInv, contactXeroid, apiInstance, accessToken, xeroTenantId);


                        Quotes result = await apiInstance.CreateQuotesAsync(accessToken, xeroTenantId.ToString(), invoices);
                        Guid? XeroID = result._Quotes.FirstOrDefault().QuoteID;
                        RequestEmpty re = new RequestEmpty();
                        //await apiInstance.EmailInvoiceAsync(accessToken, xeroTenantId.ToString(), XeroID.Value, re);
                        return XeroID;
                    }
                    catch (Xero.NetStandard.OAuth2.Client.ApiException e)
                    {
                        return null;//todotim Json(new { Error = "Authorisation Failed - please try again." });// RedirectToAction("Clients", "Home", new { Error = "Error: " + e.Message });
                    }
                }
                // UpdateClients(Emps._Contacts.Where(i => i.Name != null).ToList());//Xero.NetStandard.OAuth2.Model.PayrollAu.Employees Emps
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }// return null;//todotim Json(new { Error = "" });
                                                                          //return RedirectToAction("Clients", "Home", new { Error = "Successfully imported Clients" });
            return null;
        }

        private static async Task<Quotes> CreateQuote(string ClientBuilding, decimal quoteamount,int InspID,DateTime dteInv, Guid contactXeroid, AccountingApi apiInstance, string accessToken, Guid xeroTenantId)
        {
            Quotes invoices = new Quotes();
            invoices._Quotes = new List<Quote>(); // List<Invoice>();
                                                  //                    Xero.NetStandard.OAuth2.Model.Accounting.Invoice XeTS = new Xero.NetStandard.OAuth2.Model.Accounting.Invoice();
            Xero.NetStandard.OAuth2.Model.Accounting.Quote XeTS = new Xero.NetStandard.OAuth2.Model.Accounting.Quote();
            XeTS.Date = dteInv;// DateTime.Now.Date;
            DateTime dte = dteInv.Date.AddMonths(2);
            XeTS.Status = QuoteStatusCodes.SENT;
            XeTS.ExpiryDate /*.DueDate*/ = (new DateTime(dte.Year, dte.Month, 1)).AddDays(-1);// DateTime.Now.Date.AddDays(28);
            XeTS.Reference = "Quote auto added from RSS app";
            // XeTS.BrandingThemeID = Guid.Parse("25513a64-58a3-4f76-ac4a-834dab7733e5");
            var cu = await apiInstance.GetContactAsync(accessToken, xeroTenantId.ToString(), contactXeroid);
            Xero.NetStandard.OAuth2.Model.Accounting.Contact cust = cu._Contacts.FirstOrDefault();
            XeTS.Contact = cust;
            XeTS.Title = ClientBuilding + " Safety Inspection";
            var summary = "Please find our quotation for the Inspection," +
                " Load Testing & Recertification of the above in accordance with the Australian Standards\r\nAS/NZ 1657 & 1891(set) and the W.A. Codes of Practice in regard to Safe Working at Heights.";
            summary = summary + "\r\n";
            summary = summary + "Please note all inspections are carried out by two height safety inspectors and includes a detailed report.";
            summary = summary + "\r\n";
            summary = summary + "Thank you";
            summary = summary + "\r\n";
            summary = summary + "Please note - Mob/Demob not charged as we will be undertaking this inspection while carrying out other works in the region.";
            XeTS.Summary = summary;
            XeTS.CurrencyCode = Xero.NetStandard.OAuth2.Model.Accounting.CurrencyCode.AUD;
            XeTS.CurrencyRate = 1;
            // int lastInv = await LastInv(apiInstance, accessToken, xeroTenantId);
            //                    XeTS.InvoiceNumber = "INV-TestTim";// "INV-" + (lastInv + 1).ToString();
            XeTS.QuoteNumber = "QT-"+ InspID.ToString();// "INV-" + (lastInv + 1).ToString();
                                            //                    XeTS.Status = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.StatusEnum.DRAFT;
            XeTS.Status = QuoteStatusCodes.DRAFT;
            // XeTS.Type = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.TypeEnum.ACCREC;
            
            XeTS.LineAmountTypes = QuoteLineAmountTypes.EXCLUSIVE;
            //                    XeTS.LineAmountTypes = Xero.NetStandard.OAuth2.Model.Accounting.LineAmountTypes.Exclusive;
            XeTS.LineItems = new List<Xero.NetStandard.OAuth2.Model.Accounting.LineItem>();

            XeroHeaderDesc(XeTS, quoteamount);
            //secondline
            //  XeroCrneLine(XeTS);
            //third line
            //  XeroDogmanLine(db, tsh, XeTS);
            //if (contact.XeroID == UnknownClient)
            //{
            //    XeTS.Reference = "Auto added by PCC app - " + tsh.Company + " not found";
            //    ret = ret + " Sent to unmatched client in Xero";
            //}
            //else
            XeTS.Contact.EmailAddress = "timhams@gmail.com";
            XeTS.Reference = "Quote Auto added by RSS app";
            //Xero.NetStandard.OAuth2.Model.Accounting.Invoices invoices = new Xero.NetStandard.OAuth2.Model.Accounting.Invoices();
            //  invoices._Invoices = new List<Xero.NetStandard.OAuth2.Model.Accounting.Invoice>();
            //invoices._Invoices.Add(XeTS);
            invoices._Quotes.Add(XeTS);
          
            //Invoices result;
            return invoices;
        }

        private static void XeroCrneLine(Xero.NetStandard.OAuth2.Model.Accounting.Invoice XeTS)
        {
            bool addnew;
            Xero.NetStandard.OAuth2.Model.Accounting.LineItem lab;
            addnew = false;
            //var crane = db.Cranes.Where(i => i.CraneID == tsh.CraneID).FirstOrDefault();
           // lab = (XeTS.LineItems.Where(i => i.ItemCode == crane.XeroCode)).FirstOrDefault();
           // if (lab == null)
            {
                addnew = true;
                lab = new Xero.NetStandard.OAuth2.Model.Accounting.LineItem();
                lab.ItemCode = "ass";
            }
            lab.Description = "Inspection";
            lab.UnitAmount = 50;
            lab.Quantity = 2;
            lab.LineAmount =100;
            lab.TaxAmount = lab.LineAmount * (decimal)0.1;
            lab.TaxType = "OUTPUT";

            if (addnew)
                XeTS.LineItems.Add(lab);
        }
        /*
         * private static void XeroHeaderDesc( Xero.NetStandard.OAuth2.Model.Accounting.Invoice XeTS)
        {
            Xero.NetStandard.OAuth2.Model.Accounting.LineItem lab;
            bool addnew = false;
            lab = (XeTS.LineItems.Where(i => i.ItemCode == null)).FirstOrDefault();
            if (lab == null)
            {
                addnew = true;
                lab = new Xero.NetStandard.OAuth2.Model.Accounting.LineItem();
            }
            lab.Description ="header";
            lab.TaxType = "OUTPUT";


            if (addnew)
                XeTS.LineItems.Add(lab);
        }
        */
        private static void XeroHeaderDesc( Xero.NetStandard.OAuth2.Model.Accounting.Quote XeTS,decimal quoteamount)
        {
            Xero.NetStandard.OAuth2.Model.Accounting.LineItem lab;
            bool addnew = false;
            lab = (XeTS.LineItems.Where(i => i.ItemCode == null)).FirstOrDefault();
            if (lab == null)
            {
                addnew = true;
                lab = new Xero.NetStandard.OAuth2.Model.Accounting.LineItem();
            }
            lab.Description ="Height and Safety Inspection";
            lab.ItemCode = "inspect";

            //lab.TaxType = "OUTPUT";
            lab.Quantity = 1;
            lab.LineItemID = Guid.Parse("0187fd4e0e7744a6a9e1fe596899212e");
//            lab.LineItemID = Guid.Parse("334f826d-3d82-441f-ad39-1e2ef1067e8f");
            //lab.AccountCode = "41000";
//            lab.ItemCode = "334f826d-3d82-441f-ad39-1e2ef1067e8f";
            lab.UnitAmount = quoteamount;
            lab.LineAmount = quoteamount;
            lab.TaxAmount = 0;
            if (addnew)
                XeTS.LineItems.Add(lab);
        }
        public string RedirectURL = "https://localhost:7017/xeroredir";
        string referesh()
        {


            var tok = _context.token.FirstOrDefault();
            string refeshtoken = tok.refresh_token;
            string accessToken = tok.access_token;
            string DeveloperKey = tok.client_id;
            string DeveloperSecret = tok.client_secret;
            Token tkn = RefreshAccessToken(refeshtoken, accessToken, DeveloperKey, DeveloperSecret);
            return tkn.access_token;
        }

        private Token RefreshAccessToken(string refeshtoken, string accessToken, string DeveloperKey, string DeveloperSecret)
        {
            Token tkn = new Token();
            string ret = accessToken;
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();

                string postData;
                postData = "grant_type=refresh_token&refresh_token=" + refeshtoken + "&redirect_uri=" + RedirectURL;
                byte[] data = encoding.GetBytes(postData);

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://identity.xero.com/connect/token");
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(DeveloperKey + ":" + DeveloperSecret));
                httpWReq.Headers.Add("Authorization", $"Basic {credentials}");

                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;

                Stream postStream = httpWReq.GetRequestStream();
                postStream.Write(data, 0, data.Length);
                postStream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
               
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadLine();
                    try
                    {
                        tkn = JsonConvert.DeserializeObject<Token>(json);

                        
                    }
                    catch (Exception ex)
                    {
                        ;// db.SubmitChanges();

                        ;
                    }
                    Tennants(tkn.access_token);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tkn;
        }

        void Tennants(string access_token)
        {
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://api.xero.com/connections");
            httpWReq.Headers.Add("Authorization", $"Bearer {access_token}");
            httpWReq.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadLine();
                json = json;
                List<Tenant> tens = JsonConvert.DeserializeObject<List<Tenant>>(json);
                Tenant tt = tens.FirstOrDefault();
                if (tt != null)
                {
                    var tokdb = _context.token.Where(i => i.email == "timhams@gmail.com").FirstOrDefault();
                    tokdb.TenantType = tt.tenantType;
                    tokdb.TenantID = tt.tenantId;
                    tokdb.tenantName = tt.tenantName;
                    _context.SaveChanges();
                }
            }
        }
    }
}
