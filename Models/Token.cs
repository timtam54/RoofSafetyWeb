using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class Tenant
    {
        public Guid id { get; set; }
        public Guid authEventId { get; set; }
        public Guid tenantId { get; set; }
        public string tenantType { get; set; }
        public string tenantName { get; set; }
        public string createdDateUtc { get; set; }
        public string updatedDateUtc { get; set; }
    }
    public class Token
    {
        public string? access_token { get; set; }
        public DateTime? DteTme { get; set; }
        public DateTime? expires_at { get; set; }
        public string? refresh_token { get; set; }

        public string? scope { get; set; }
        public string? TenantType { get; set; }
        public string? tenantName { get; set; }
        [Key]
        public string email { get; set; }

        public string client_secret { get; set; }
        public string client_id { get; set; }

        public Guid? TenantID { get; set; }

        public string? token_type { get; set; }

        public string? jti { get; set; }
    }

}
