using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoofSafety.Data;
using RoofSafety.Models;
using static RoofSafety.Pages.xeroredirModel;
using Xero.NetStandard.OAuth2.Api;
using System.Net;
using System.Text;

namespace RoofSafety.Pages
{
    public class InspectionModel : PageModel
    {
        private readonly dbcontext _context;
        public InspectionModel(dbcontext context)
        {
            _context = context;
        }
        public async Task<ActionResult> OnGet()
        {
            return await CreateXeroInvoice();
        }
        // function to get date yesterday
        //  write a function to add two parameters
        


        public async Task<ActionResult> CreateXeroInvoice()
        {
            try
            {

               

                var job = await _context.Inspection.Where(i => i.id == 112).Include(i=>i.Building).FirstOrDefaultAsync();
                //var building = await _context.Building.Where(i => i.id == job.BuildingID).FirstOrDefaultAsync();
                var client = await _context.Client.Where(i => i.id == job.Building.ClientID).FirstOrDefaultAsync();

                var apiInstance = new AccountingApi();
                string accessToken; Guid xeroTenantId;
                var tkn = _context.token.FirstOrDefault();

                accessToken = tkn.access_token;
                xeroTenantId = tkn.TenantID.Value;
                Xero.NetStandard.OAuth2.Model.Accounting.Invoice XeTS;
                try { 
                GenerateXeroQuote(apiInstance, accessToken, xeroTenantId, job, client);
                }
                catch (Xero.NetStandard.OAuth2.Client.ApiException ex)
                {
                    try
                    {
                        tkn = _context.token.FirstOrDefault();
                        tkn.access_token = referesh(tkn.client_id, tkn.client_secret);
                        // accessToken = tkn.access_token;
                       // tkn.TenantID = tkn.TenantID.Value;
                        xeroTenantId = tkn.TenantID.Value;
                        GenerateXeroQuote(apiInstance, accessToken, xeroTenantId, job, client);

                        //                    Emps = await apiInstance.GetContactsAsync(accessToken, xeroTenantId.ToString(),null, null, null);
                    }
                    catch (Xero.NetStandard.OAuth2.Client.ApiException e)
                    {
                        return Page();//todotim Json(new { Error = "Authorisation Failed - please try again." });// RedirectToAction("Clients", "Home", new { Error = "Error: " + e.Message });
                    }
                }
                return Page();

                XeTS = new Xero.NetStandard.OAuth2.Model.Accounting.Invoice();
                XeTS.Date = DateTime.Now.Date;// DateTime.Now.Date;
                DateTime dte = DateTime.Now.Date.AddMonths(2);

                XeTS.DueDate = (new DateTime(dte.Year, dte.Month, 1)).AddDays(-1);// DateTime.Now.Date.AddDays(28);
                XeTS.Reference = "auto added from RSS Web app";
                //XeTS.BrandingThemeID = Guid.Parse("25513a64-58a3-4f76-ac4a-834dab7733e5");

                var cu = await apiInstance.GetContactAsync(accessToken, xeroTenantId.ToString(), client.XeroID.Value);
                Xero.NetStandard.OAuth2.Model.Accounting.Contact cust = cu._Contacts.FirstOrDefault();
                XeTS.Contact = cust;
                XeTS.CurrencyCode = Xero.NetStandard.OAuth2.Model.Accounting.CurrencyCode.AUD;
                XeTS.CurrencyRate = 1;
                int lastInv = 999;
                /* todotim            int lastInv = await LastInv(apiInstance, accessToken, xeroTenantId);*/
                XeTS.InvoiceNumber = "INV-" + (lastInv + 1).ToString();
                XeTS.Status = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.StatusEnum.DRAFT;
                XeTS.Type = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.TypeEnum.ACCREC;
                XeTS.LineAmountTypes = Xero.NetStandard.OAuth2.Model.Accounting.LineAmountTypes.Exclusive;
                XeTS.LineItems = new List<Xero.NetStandard.OAuth2.Model.Accounting.LineItem>();
                XeroHeaderDesc(job, XeTS);
                //secondline
                //     XeroCrneLine(db, tsh, XeTS);
                //third line
                //       XeroDogmanLine(db, tsh, XeTS);
                if (client.XeroID == UnknownClient)
                {
                    XeTS.Reference = "Auto added by RSS app - " + client.name + " not found";
                    // ret = ret + " Sent to unmatched client in Xero";
                }
                else
                    XeTS.Reference = "Auto added by RSS app";
                Xero.NetStandard.OAuth2.Model.Accounting.Invoices invoices = new Xero.NetStandard.OAuth2.Model.Accounting.Invoices();
                invoices._Invoices = new List<Xero.NetStandard.OAuth2.Model.Accounting.Invoice>();
                invoices._Invoices.Add(XeTS);
                var result = await apiInstance.CreateInvoicesAsync(accessToken, xeroTenantId.ToString(), invoices);
                job.XeroID = result._Invoices.FirstOrDefault().InvoiceID;
                job.InvoiceDate = DateTime.Now;

            }
            catch (Exception ex)
            {
                string xx = ex.Message.Replace("Xero API 400 error calling ", "");
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>("{" + xx + "}");
                //  ret = myDeserializedClass.CreateInvoices.Elements.FirstOrDefault().ValidationErrors.FirstOrDefault().Message;
            }

            //     tsh.XeroExportError = ret;

            _context.SaveChanges();
            return Page();
        }
        static Guid UnknownClient = Guid.Parse("6640b0a2-440a-4389-a92a-00a5a46cbcc0");//todotim
        private static void XeroHeaderDesc(Inspection tsh, Xero.NetStandard.OAuth2.Model.Accounting.Invoice XeTS)
        {
            Xero.NetStandard.OAuth2.Model.Accounting.LineItem lab;
            bool addnew = false;
            lab = (XeTS.LineItems.Where(i => i.ItemCode == null)).FirstOrDefault();
            if (lab == null)
            {
                addnew = true;
                lab = new Xero.NetStandard.OAuth2.Model.Accounting.LineItem();
            }
            lab.Description = tsh.Building.BuildingName;
            lab.TaxType = "OUTPUT";


            if (addnew)
                XeTS.LineItems.Add(lab);
        }
        string referesh(string DeveloperKey, string DeveloperSecret)
        {
            string ret = "";
            try
            {
                //DataClasses1DataContext db = new DataClasses1DataContext();
                var tok = _context.token.FirstOrDefault();
                string refeshtoken = tok.refresh_token;
                ret = tok.access_token;

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
                Token tkn = new Token();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadLine();
                    try
                    {
                        tkn = JsonConvert.DeserializeObject<Token>(json);

                        //db.Tokens.DeleteOnSubmit(db.Tokens.FirstOrDefault());
                        //db.SubmitChanges();
                        Token tk = _context.token.FirstOrDefault();
                        tk.access_token = tkn.access_token;
                        ret = tkn.access_token;
                        //tk.DteTme = DateTime.UtcNow;
                        //todotim     tk.expires_at = tkn.expires_in;
                        tk.jti = tkn.jti;
                        tk.refresh_token = tkn.refresh_token;
                        tk.scope = tkn.scope;
                        tk.token_type = tkn.token_type;
                        // tk.userId = tkn.userId;
                        //                    db.Tokens.InsertOnSubmit(tk);
                        _context.SaveChanges();
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
            return ret;
        }
        public string RedirectURL = "https://localhost:7017/xeroredir";
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
        //////
        ///
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Quote> GenerateXeroQuote(AccountingApi apiInstance, string accessToken, Guid xeroTenantId, Inspection job, Client client)
        {
            var quote = new Xero.NetStandard.OAuth2.Model.Accounting.Quote();
            quote.Date = DateTime.Now.Date;
            DateTime dueDate = DateTime.Now.Date.AddMonths(2);
           // quote.DueDate = (new DateTime(dueDate.Year, dueDate.Month, 1)).AddDays(-1);
            quote.Reference = "auto added from RSS Web app";
            quote.Contact = new Xero.NetStandard.OAuth2.Model.Accounting.Contact { ContactID = client.XeroID.Value };
            quote.CurrencyCode = Xero.NetStandard.OAuth2.Model.Accounting.CurrencyCode.AUD;
            quote.CurrencyRate = 1;
            int qn = _context.settings.FirstOrDefault().XeroQuoteNo.Value + 1; ;
            quote.QuoteNumber = "Quote-" + qn;// await GetNextInvoiceNumber(apiInstance, accessToken, xeroTenantId);
            quote.Status = Xero.NetStandard.OAuth2.Model.Accounting.QuoteStatusCodes.DRAFT;// Xero.NetStandard.OAuth2.Model.Accounting.Quote..StatusEnum.DRAFT;
            //quote.Type = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.TypeEnum.ACCREC;
            quote.LineAmountTypes = Xero.NetStandard.OAuth2.Model.Accounting.QuoteLineAmountTypes.EXCLUSIVE;// Xero.NetStandard.OAuth2.Model.Accounting.LineAmountTypes.Exclusive;
            quote.LineItems = new List<Xero.NetStandard.OAuth2.Model.Accounting.LineItem>();
            AddQuoteLineItem(quote, job.Building.BuildingName);

            Xero.NetStandard.OAuth2.Model.Accounting.Quotes quotes = new Xero.NetStandard.OAuth2.Model.Accounting.Quotes();
            quotes._Quotes = new List<Xero.NetStandard.OAuth2.Model.Accounting.Quote>();
            quotes._Quotes.Add(quote);
            try
            {
                var result = await apiInstance.CreateQuotesAsync(accessToken, xeroTenantId.ToString(), quotes);
                job.XeroID = result._Quotes.FirstOrDefault().QuoteID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _context.settings.FirstOrDefault().XeroQuoteNo = qn;
            _context.SaveChanges();
            return quote;
        }

        //private async Task<int> GetNextInvoiceNumber(AccountingApi apiInstance, string accessToken, Guid xeroTenantId)
        //{
        //    return
        //  //  var invoices = await apiInstance.GetQuotesAsync(accessToken, xeroTenantId.ToString());

        //  ////  var invoices = await apiInstance.GetInvoicesAsync(accessToken, xeroTenantId.ToString());
        //  //  var lastInvoice = invoices._Quotes.OrderByDescending(i => i.QuoteNumber).FirstOrDefault();
        //  //  if (lastInvoice != null)
        //  //  {
        //  //      var lastInvoiceNumber = lastInvoice.QuoteNumber;
        //  //      var invoiceNumberPrefix = lastInvoiceNumber.Substring(0, lastInvoiceNumber.IndexOf("-") + 1);
        //  //      var invoiceNumberSuffix = lastInvoiceNumber.Substring(lastInvoiceNumber.IndexOf("-") + 1);
        //  //      var nextInvoiceNumber = int.Parse(invoiceNumberSuffix) + 1;
        //  //      return $"{invoiceNumberPrefix}{nextInvoiceNumber}";
        //  //  }
        //  //  else
        //  //  {
        //  //      return "INV-1";
        //  //  }
        //}

        private void AddQuoteLineItem(Xero.NetStandard.OAuth2.Model.Accounting.Quote quote, string description)
        {
            var lineItem = new Xero.NetStandard.OAuth2.Model.Accounting.LineItem();
            lineItem.Description = description;
            lineItem.TaxType = "OUTPUT";
            quote.LineItems.Add(lineItem);
        }
    }
}
