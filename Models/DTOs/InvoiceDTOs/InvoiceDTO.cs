namespace Invoice.Models.DTOs.InvoiceDTOs
{
    public class InvoiceDTO
    {
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public string ClientId { get; set; }
        public List<InvoiceItemDTO> InvoiceItems { get; set; }
    }
}
