using Invoice.Data;
using Invoice.Models.DTOs.InvoiceDTOs;
using Invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;

namespace Invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Invoices
        [HttpPost]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<ActionResult<InvoiceModel>> CreateInvoice(InvoiceDTO invoiceDTO)
        {
            var invoice = new InvoiceModel
            {
                Date = invoiceDTO.Date,
                DueDate = invoiceDTO.DueDate,
                ClientId = invoiceDTO.ClientId,
                VendorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                InvoiceItems = invoiceDTO.InvoiceItems.Select(ii => new InvoiceItem
                {
                    ItemId = ii.ItemId,
                    Quantity = ii.Quantity
                }).ToList()
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, invoice);
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceModel>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Item)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // GET: api/Invoices/vendor
        [HttpGet("vendor")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<IEnumerable<InvoiceModel>>> GetInvoicesAsVendor()
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var invoices = await _context.Invoices
                .Where(i => i.VendorId == vendorId)
                .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Item)
                .ToListAsync();

            return invoices;
        }
        // GET: api/Invoices/client
        [HttpGet("client")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<InvoiceModel>>> GetInvoicesAsClient()
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var invoices = await _context.Invoices
                .Where(i => i.ClientId == clientId)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Item)
                .Include(i => i.Vendor) // Include vendor details
                .Select(i => new
                {
                    i.InvoiceId,
                    i.Date,
                    i.DueDate,
                    i.IsPaid,
                    VendorDetails = new { i.Vendor.FirstName, i.Vendor.LastName, i.Vendor.Email },
                    InvoiceItems = i.InvoiceItems.Select(ii => new
                    {
                        ii.InvoiceItemId,
                        ii.ItemId,
                        ii.Quantity,
                        ItemDetails = new { ii.Item.Name, ii.Item.Price }
                    })
                })
                .ToListAsync();

            return Ok(invoices);
        }



        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceDTO invoiceDTO)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Update invoice details
            invoice.Date = invoiceDTO.Date;
            invoice.DueDate = invoiceDTO.DueDate;
            invoice.ClientId = invoiceDTO.ClientId;

            // Update invoice items
            // Note: This is a simplistic approach. You might need more complex logic for production.
            _context.InvoiceItems.RemoveRange(invoice.InvoiceItems);
            invoice.InvoiceItems = invoiceDTO.InvoiceItems.Select(ii => new InvoiceItem
            {
                ItemId = ii.ItemId,
                Quantity = ii.Quantity
            }).ToList();

            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Invoices/5/markpaid
        [HttpPost("{id}/markpaid")]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<IActionResult> MarkInvoicePaid(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IsPaid = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
