using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentEmail.Core.Models;
using Invoice.Services;
using System.Threading.Tasks;
using Invoice.Models.DTOs;

namespace Invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto emailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _emailSender.SendEmailAsync(emailDto.Email, emailDto.Subject, emailDto.Message);
            return Ok();
        }
    }
}
