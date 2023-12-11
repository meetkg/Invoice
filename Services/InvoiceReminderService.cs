using Invoice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Invoice.Services
{
    public class InvoiceReminderService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public InvoiceReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }
        /*    public Task StartAsync(CancellationToken cancellationToken)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                return Task.CompletedTask;
            }*/

        private async void DoWork(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var unpaidInvoices = await context.Invoices
                .Include(i => i.Client)  // Include the Client
                .Where(i => !i.IsPaid)
                .ToListAsync();

            foreach (var invoice in unpaidInvoices)
            {
                if (invoice.Client != null && !string.IsNullOrEmpty(invoice.Client.Email))
                {
                    var message = $"Reminder: Your invoice #{invoice.InvoiceId} is still unpaid.";
                    await emailSender.SendEmailAsync(invoice.Client.Email, "Invoice Reminder", message);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
