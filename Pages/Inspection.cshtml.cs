using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoofSafety.Data;
using RoofSafety.Models;
using static RoofSafety.Pages.xeroredirModel;
using Xero.NetStandard.OAuth2.Api;

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
                    XeTS.Reference = "Auto added by PCC app - " + client.name + " not found";
                    // ret = ret + " Sent to unmatched client in Xero";
                }
                else
                    XeTS.Reference = "Auto added by PCC app";
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

        //////
        ///
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Invoice> GenerateXeroQuote(AccountingApi apiInstance, string accessToken, Guid xeroTenantId, Inspection job, Client client)
        {
            var quote = new Xero.NetStandard.OAuth2.Model.Accounting.Invoice();
            quote.Date = DateTime.Now.Date;
            DateTime dueDate = DateTime.Now.Date.AddMonths(2);
            quote.DueDate = (new DateTime(dueDate.Year, dueDate.Month, 1)).AddDays(-1);
            quote.Reference = "auto added from RSS Web app";
            quote.Contact = new Xero.NetStandard.OAuth2.Model.Accounting.Contact { ContactID = client.XeroID.Value };
            quote.CurrencyCode = Xero.NetStandard.OAuth2.Model.Accounting.CurrencyCode.AUD;
            quote.CurrencyRate = 1;
            quote.InvoiceNumber = await GetNextInvoiceNumber(apiInstance, accessToken, xeroTenantId);
            quote.Status = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.StatusEnum.DRAFT;
            quote.Type = Xero.NetStandard.OAuth2.Model.Accounting.Invoice.TypeEnum.ACCREC;
            quote.LineAmountTypes = Xero.NetStandard.OAuth2.Model.Accounting.LineAmountTypes.Exclusive;
            quote.LineItems = new List<Xero.NetStandard.OAuth2.Model.Accounting.LineItem>();
            AddQuoteLineItem(quote, job.Building.BuildingName);
            return quote;
        }

        private async Task<string> GetNextInvoiceNumber(AccountingApi apiInstance, string accessToken, Guid xeroTenantId)
        {
            var invoices = await apiInstance.GetInvoicesAsync(accessToken, xeroTenantId.ToString());
            var lastInvoice = invoices._Invoices.OrderByDescending(i => i.InvoiceNumber).FirstOrDefault();
            if (lastInvoice != null)
            {
                var lastInvoiceNumber = lastInvoice.InvoiceNumber;
                var invoiceNumberPrefix = lastInvoiceNumber.Substring(0, lastInvoiceNumber.IndexOf("-") + 1);
                var invoiceNumberSuffix = lastInvoiceNumber.Substring(lastInvoiceNumber.IndexOf("-") + 1);
                var nextInvoiceNumber = int.Parse(invoiceNumberSuffix) + 1;
                return $"{invoiceNumberPrefix}{nextInvoiceNumber}";
            }
            else
            {
                return "INV-1";
            }
        }

        private void AddQuoteLineItem(Xero.NetStandard.OAuth2.Model.Accounting.Invoice quote, string description)
        {
            var lineItem = new Xero.NetStandard.OAuth2.Model.Accounting.LineItem();
            lineItem.Description = description;
            lineItem.TaxType = "OUTPUT";
            quote.LineItems.Add(lineItem);
        }
    }
}
