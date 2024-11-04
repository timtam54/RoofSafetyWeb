using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoofSafety.Data;
using RoofSafety.Models;
using System.Net;
using System.Text;
using Xero.NetStandard.OAuth2.Api;

namespace RoofSafety.Pages
{
    public class xeroredirModel : PageModel
    {
        [BindProperty]

        public Token Token { get; set; }

        private readonly dbcontext _context;
        public xeroredirModel(dbcontext context)
        {
            _context = context;
        }
        public string Error { get; set; }

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

       
        public ActionResult OnGet(string? code)
        {
            var Email = "timhams@gmail.com";// User.Identity.Name;// _context.Token.FirstOrDefault().email;
            if (code!=null)
            {
                ////

                bool newrec = true;
                try
                {
                    if (Email == null)
                        return RedirectToPage("/AccessDenied");
                    Token = _context.token.Where(i => i.email == Email).FirstOrDefault();//.Where(i => i.email == Email)
                    if (Token != null)
                        newrec = false;
                    else
                    {
                        Token = new Token();
                        Token.scope = scope;
                    }
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string postData;
                    postData = "grant_type=authorization_code&code=" + code + "&redirect_uri=" + RedirectURL;
                    byte[] data = encoding.GetBytes(postData);

                    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://identity.xero.com/connect/token");
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Token.client_id + ":" + Token.client_secret));
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
                            Token tkn = JsonConvert.DeserializeObject<Token>(json);
                            Token.access_token = tkn.access_token;
                            Token.TenantID = tkn.TenantID;
                            Token.refresh_token = tkn.refresh_token;
                            Token.scope = tkn.scope;
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Error = ex.Message;

                            ;
                        }
                        Tennants(Token.access_token);
                    }
                    return RedirectToAction("XeroPage");// View("Xero");


                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    if (!newrec && Token != null)
                    {
                        try
                        {
                            //Token.Error = Error;
                            _context.SaveChanges();
                        }
                        catch (Exception x)
                        {

                        }
                    }

                }

                return Page();// View("Xero");
            }
            ///


        
           // var Email = "timhams@gmail.com";// User.Identity.Name;// _context.Token.FirstOrDefault().email;
            if (Email == null)
                return RedirectToPage("/AccessDenied");

            Token = _context.token.Where(i => i.email == Email).FirstOrDefault();//.client_id;//.Where(i => i.email == Email)
            if (Token == null)
            {

                Token = new Token();
                Token.client_id = ClientID;
                Token.client_secret = Secret;
                Token.scope = scope;
            }
            return Page();
        }

        public  string scope = "all";
        public  string ClientID = "A679D5B413084FBCBEF4A06E23788956";
        public  string Secret = "zAIaVq-M94Ftu0wi9NIERuko6U4lDRDszcuCfUYygLqMfAal";
        public  string RedirectURL = "https://localhost:7017/xeroredir";

        public ActionResult OnPost()
        {
            var Email = "timhams@gmail.com";// User.Identity.Name;// _c
            if (Email == null)
                return RedirectToPage("/AccessDenied");
            bool newrec = false;
            var DBToken = _context.token.Where(i => i.email == Email).FirstOrDefault();
            if (DBToken == null)
            {
                newrec = true;
                DBToken = new Token();
                DBToken.email = Email;
            }
            //DBToken.email = Token.email;
            DBToken.client_secret = Token.client_secret;
            DBToken.client_id = Token.client_id;
            if (DBToken.access_token == null)
                DBToken.access_token = "populated on submit";
            if (DBToken.expires_at == null)
                DBToken.expires_at = new DateTime(2000, 1, 1);
            DBToken.scope = Token.scope;
            //     DBToken.refresh_token = Token.refresh_token;
            //DBToken.access_token = Token.access_token;
            //     DBToken.expires_at = Token.expires_at;
            //    DBToken.token_type = Token.token_type;
            //  DBToken.merchant_id = Token.merchant_id;
            //  DBToken.short_lived = Token.short_lived;
            if (newrec)
                _context.token.Add(DBToken);
            _context.SaveChanges();
            return Page();
        }

        public async Task<ActionResult> OnPostCustImport()
        {
            return Page();
        }
    
        

       

        string MaxLength(int len, string instring)
        {
            if (instring == null)
                return null;
            try
            {
                if (instring.Length <= len)
                    return instring;
                return instring.Substring(0, len);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return instring;
        }
        string referesh(string DeveloperKey,string DeveloperSecret)
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
                Token tkn=new Token();
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
                       // tk.expires_at = tkn.expires_in;
                        tk.jti = tkn.jti;
                        tk.refresh_token = tkn.refresh_token;
                        tk.scope = tkn.scope;
                        tk.token_type = tkn.token_type;
                       // tk.userId = tkn.userId;
                        //_context.Add(token.InsertOnSubmit(tk);
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
        
        //static DateTime? _LastInvoiceDate;
        //static DateTime LastInvoiceDateFromDB
        //{
        //    get
        //    {
        //        if (_LastInvoiceDate != null)
        //            return _LastInvoiceDate.Value;

        //        _LastInvoiceDate = db.Settings.FirstOrDefault().LastInvoice;
        //        return _LastInvoiceDate.Value;
        //    }
        //    set
        //    {
        //        if (LastInvoiceDateFromDB.Date == value.Date)
        //            return;
        //        _LastInvoiceDate = value;
        //        DataClasses1DataContext db = new DataClasses1DataContext();
        //        db.Settings.FirstOrDefault().LastInvoice = _LastInvoiceDate;
        //        db.SubmitChanges();
        //    }
        //}
        //static async Task<int> LastInv(AccountingApi apiInstance, string accessToken, Guid xeroTenantId)
        //{

        //    Xero.NetStandard.OAuth2.Model.Accounting.Invoices result = await apiInstance.GetInvoicesAsync(accessToken, xeroTenantId.ToString(), LastInvoiceDateFromDB, "Date >= DateTime(" + LastInvoiceDateFromDB.Year.ToString() + ", " + LastInvoiceDateFromDB.Month.ToString() + ", " + LastInvoiceDateFromDB.Day.ToString() + ")");// "ContactID=GUID(\"" + contact.XeroID.ToString() + "\")"); //  "ContactID=GUID(\"" + contact.XeroID.ToString() + "\")"); // "Date=DateTime(" + per.PeriodEnd.Year.ToString() + "," + per.PeriodEnd.Month.ToString() + "," + per.PeriodEnd.Day.ToString() + ") && EmployeeID=GUID(\"" + emp.XeroID.ToString() + "\")");
        //    //Xero.NetStandard.OAuth2.Model.Accounting.Invoices resultAll = await apiInstance.GetInvoicesAsync(accessToken, xeroTenantId.ToString(), LastInvoiceDateFromDB,"");// "ContactID=GUID(\"" + contact.XeroID.ToString() + "\")"); //  "ContactID=GUID(\"" + contact.XeroID.ToString() + "\")"); // "Date=DateTime(" + per.PeriodEnd.Year.ToString() + "," + per.PeriodEnd.Month.ToString() + "," + per.PeriodEnd.Day.ToString() + ") && EmployeeID=GUID(\"" + emp.XeroID.ToString() + "\")");
        //    var maxinvno = result._Invoices.Where(i => i.InvoiceNumber.ToLower().Contains("inv-")).OrderByDescending(i => i.InvoiceNumber).Take(1).FirstOrDefault().InvoiceNumber;
        //    //var maxinvnoal = resultAll._Invoices.Where(i => i.InvoiceNumber.ToLower().Contains("inv-")).OrderByDescending(i => i.InvoiceNumber).Take(1).FirstOrDefault().InvoiceNumber;
        //    maxinvno = maxinvno.ToLower().Replace("inv-", "");
        //    return Convert.ToInt32(maxinvno);
        //}

        public class Address
        {
            public string AddressType { get; set; }
            public string AddressLine1 { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
        }

        public class BatchPayments
        {
            public string BankAccountNumber { get; set; }
        }

        public class Contact
        {
            public string ContactID { get; set; }
            public string ContactStatus { get; set; }
            public string Name { get; set; }
            public string FirstName { get; set; }
            public string EmailAddress { get; set; }
            public string BankAccountDetails { get; set; }
            public string TaxNumber { get; set; }
            public List<Address> Addresses { get; set; }
            public List<Phone> Phones { get; set; }
            public DateTime UpdatedDateUTC { get; set; }
            public List<object> ContactGroups { get; set; }
            public bool IsSupplier { get; set; }
            public bool IsCustomer { get; set; }
            public List<object> SalesTrackingCategories { get; set; }
            public List<object> PurchasesTrackingCategories { get; set; }
            public BatchPayments BatchPayments { get; set; }
            public List<ContactPerson> ContactPersons { get; set; }
            public bool HasValidationErrors { get; set; }
        }

        public class ContactPerson
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public bool IncludeInEmails { get; set; }
        }

        public class CreateInvoices
        {
            public int ErrorNumber { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public List<Element> Elements { get; set; }
        }

        public class Element
        {
            public string Type { get; set; }
            public string InvoiceID { get; set; }
            public string InvoiceNumber { get; set; }
            public string Reference { get; set; }
            public List<object> Prepayments { get; set; }
            public List<object> Overpayments { get; set; }
            public double AmountDue { get; set; }
            public double AmountPaid { get; set; }
            public bool SentToContact { get; set; }
            public double CurrencyRate { get; set; }
            public bool IsDiscounted { get; set; }
            public bool HasErrors { get; set; }
            public List<object> InvoicePaymentServices { get; set; }
            public Contact Contact { get; set; }
            public DateTime DateString { get; set; }
            public DateTime Date { get; set; }
            public DateTime DueDateString { get; set; }
            public DateTime DueDate { get; set; }
            public string BrandingThemeID { get; set; }
            public string Status { get; set; }
            public string LineAmountTypes { get; set; }
            public List<LineItem> LineItems { get; set; }
            public double SubTotal { get; set; }
            public double TotalTax { get; set; }
            public double Total { get; set; }
            public string CurrencyCode { get; set; }
            public List<ValidationError> ValidationErrors { get; set; }
        }

        public class Item
        {
            public string ItemID { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
        }

        public class LineItem
        {
            public string Description { get; set; }
            public List<object> Tracking { get; set; }
            public string LineItemID { get; set; }
            public List<object> ValidationErrors { get; set; }
            public string ItemCode { get; set; }
            public double? UnitAmount { get; set; }
            public string TaxType { get; set; }
            public double? TaxAmount { get; set; }
            public double? LineAmount { get; set; }
            public Item Item { get; set; }
            public double? Quantity { get; set; }
        }

        public class Phone
        {
            public string PhoneType { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class Root
        {
            public CreateInvoices CreateInvoices { get; set; }
        }

        public class ValidationError
        {
            public string Message { get; set; }
        }



        public class XeroInvoiceResult
        {
            public List<Xero.NetStandard.OAuth2.Model.Accounting.Invoice> Invoices { get; set; }
            public string Error { get; set; }
        }

        public class XeroInvoiceDisplay
        {
            public List<InvoiceDet> Invoices { get; set; }
            public string Error { get; set; }
        }
        public class InvoiceDet
        {
            public string Description { get; set; }
            public string InvoiceNumber { get; set; }
            public DateTime? Date { get; set; }
            public decimal? Amount { get; set; }
            public Guid XeroInvID { get; set; }
        }

    }
}
