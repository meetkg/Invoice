using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Invoice.Models
{
    public class InvoiceModel
    {
        [Key]
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public string VendorId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser Vendor { get; set; }
        public string ClientId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser Client { get; set; }
        public bool IsPaid { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new HashSet<InvoiceItem>();
    }

}
