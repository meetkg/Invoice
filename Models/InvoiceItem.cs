using System.Text.Json.Serialization;

namespace Invoice.Models
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public virtual InvoiceModel Invoice { get; set; }
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
