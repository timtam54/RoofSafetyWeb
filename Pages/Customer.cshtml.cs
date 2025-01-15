using EficazFramework.Validation.DataAnnotations;
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
    public class CustomerModel : PageModel
    {
        private readonly dbcontext _context;
        public CustomerModel(dbcontext context)
        {
            _context = context;
        }
        public async Task<ActionResult> OnGet()
        {
            var apiInstance = new AccountingApi();// PayrollAuApi();
            string accessToken;
            Guid xeroTenantId;
            Xero.NetStandard.OAuth2.Model.Accounting.Contacts Emps = null;

            Token tkn;
            tkn = _context.token.FirstOrDefault();
            accessToken = tkn.access_token;
            xeroTenantId = tkn.TenantID.Value;
            try
            {
                Emps = await apiInstance.GetContactsAsync(accessToken, xeroTenantId.ToString(), null, null, null);
            }
            catch (Xero.NetStandard.OAuth2.Client.ApiException ex)
            {
                try
                {
                    tkn = _context.token.FirstOrDefault();
                    accessToken = referesh(tkn.client_id, tkn.client_secret);
                    // accessToken = tkn.access_token;
                    xeroTenantId = tkn.TenantID.Value;
                    Emps = await apiInstance.GetContactsAsync(accessToken, xeroTenantId.ToString(), null, null, null);

//                    Emps = await apiInstance.GetContactsAsync(accessToken, xeroTenantId.ToString(),null, null, null);
                }
                catch (Xero.NetStandard.OAuth2.Client.ApiException e)
                {
                    return Page();//todotim Json(new { Error = "Authorisation Failed - please try again." });// RedirectToAction("Clients", "Home", new { Error = "Error: " + e.Message });
                }
            }
            UpdateClients(Emps._Contacts.Where(i=>i.Name!=null).ToList());//Xero.NetStandard.OAuth2.Model.PayrollAu.Employees Emps
                                          // return null;//todotim Json(new { Error = "" });
                                          //return RedirectToAction("Clients", "Home", new { Error = "Successfully imported Clients" });
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
        public string RedirectURL = "https://localhost:7017/xeroredir";
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
                        tk.expires_at = tkn.expires_at;
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
        public List<Client> clis { get; set; }
        void UpdateClients(List<Xero.NetStandard.OAuth2.Model.Accounting.Contact> Custs)
        {

            //DataClasses1DataContext db = new DataClasses1DataContext();
            clis = _context.Client.ToList();
            foreach (Xero.NetStandard.OAuth2.Model.Accounting.Contact item in Custs)
            {
                if (item.Name.ToLower().Contains("broome") && item.Name.ToLower().Contains("project"))
                {
                    Console.WriteLine(item.Name);
                }
                if (item.ContactStatus == Xero.NetStandard.OAuth2.Model.Accounting.Contact.ContactStatusEnum.ACTIVE) { 
                    try
                    {
                        
                        Client cliMatch = null;
                        if (item.Name != null)
                            cliMatch = clis.Where(i => i.name.ToLower() == item.Name.ToLower()).FirstOrDefault();
                        //if (cliMatch == null)
                        //{
                        //    if (item.EmailAddress != null)
                        //    {
                        //        var cliMatch2 = clis.Where(i => i.EmailAddress != null && i.EmailAddress!="").ToList();
                        //        cliMatch = cliMatch2.Where(i => i.EmailAddress.ToLower() == item.EmailAddress.ToLower()).FirstOrDefault();

                        //    }
                        //}

                        if (cliMatch == null)
                        {
                            cliMatch = clis.Where(i => i.XeroID == item.ContactID).FirstOrDefault();
                            if (cliMatch != null)
                            {
                                cliMatch.name=item.Name  ;
                                _context.SaveChanges();
                               //return;
                            }
                        }
                        bool newrec = false;
                        if (cliMatch == null)
                        {
                            newrec = true;
                            cliMatch = new Client();
                            cliMatch.XeroID = item.ContactID;
                            cliMatch.EmailAddress = item.EmailAddress;
                            cliMatch.source = "AddedFromXero";
                            cliMatch.name = item.Name;
                            cliMatch.PhoneNumber = item.Phones.FirstOrDefault().PhoneNumber;
                            var cont = item.ContactPersons.FirstOrDefault();
                            if (cont != null)
                            {
                                cliMatch.ContactName = cont.FirstName + " " + cont.LastName;
                                if (cliMatch.EmailAddress==null || cliMatch.EmailAddress=="")

                                    cliMatch.EmailAddress = cont.EmailAddress;
                                //cliMatch.ContactName= cont.FirstName + " " + cont.LastName;
                            }
                            //add new
                        }
                        

                        if (cliMatch.XeroID != item.ContactID || newrec || (cliMatch.ContactName =="" && item.ContactPersons.Count()>0))
                        {
                            var cont = item.ContactPersons.FirstOrDefault();
                            if (cont != null)
                            {
                                cliMatch.ContactName = cont.FirstName + " " + cont.LastName;
                                if (cliMatch.EmailAddress == null || cliMatch.EmailAddress == "")

                                    cliMatch.EmailAddress = cont.EmailAddress;
                                //cliMatch.ContactName= cont.FirstName + " " + cont.LastName;
                            }
                            cliMatch.XeroID = item.ContactID;
                            if (item.Addresses != null)
                            {
                                if (item.Addresses.Count > 0)
                                {
                                    cliMatch.Address = MaxLength(150, item.Addresses.FirstOrDefault().AddressLine1 + " " + item.Addresses.FirstOrDefault().AddressLine2 + " " + item.Addresses.FirstOrDefault().AddressLine3) + " " + MaxLength(150, item.Addresses.FirstOrDefault().City) + " " + MaxLength(4, item.Addresses.FirstOrDefault().PostalCode);
                                    //                          cliMatch.ClientName = MaxLength(255, item.Addresses.FirstOrDefault().AttentionTo);
                                    //cliMatch.PostCode = MaxLength(4, item.Addresses.FirstOrDefault().PostalCode);
                                }
                            }
                            cliMatch.ABN = item.TaxNumber;
                            cliMatch.ACN = item.CompanyNumber;

                            if (newrec)
                                _context.Add(cliMatch);//.Client.InsertOnSubmit(cliMatch);
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
            }
            }

        }
    }
}
