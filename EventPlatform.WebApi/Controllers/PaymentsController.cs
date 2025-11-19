using EventPlatform.Application.Contracts;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Payment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace EventPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("initiate-sepay/{registrationId}")]
        public async Task<IActionResult> InitiateSepayPayment(Guid registrationId)
        {
            try
            {
                var response = await _paymentService.InitiateSepayPaymentAsync(registrationId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message }); 
            }
        }

        [HttpPost("sepay")]
        public async Task<IActionResult> HandleSepayWebhook([FromBody] SepayWebhookPayload payload)
        {
            try
            {
                Request.Headers.TryGetValue("X-Sepay-Key", out var apiKeyHeader);
                await _paymentService.HandleSepayWebhookAsync(payload, apiKeyHeader.FirstOrDefault());

                return Ok();
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }
    }
}