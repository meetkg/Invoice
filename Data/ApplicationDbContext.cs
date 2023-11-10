using Invoice.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<InvoiceModel> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the InvoiceModel entity
            modelBuilder.Entity<InvoiceModel>()
                .HasOne(invoice => invoice.Vendor)
                .WithMany()
                .HasForeignKey(invoice => invoice.VendorId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            modelBuilder.Entity<InvoiceModel>()
                .HasOne(invoice => invoice.Client)
                .WithMany()
                .HasForeignKey(invoice => invoice.ClientId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete
        }
    }
}
