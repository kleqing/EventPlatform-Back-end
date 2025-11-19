using EventPlatform.Application.Contracts;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                string apiKey = "";
                if (Request.Headers.TryGetValue("Authorization", out var apiKeyHeader))
                {
                    string headerValue = apiKeyHeader.FirstOrDefault();
                    // Header sẽ có dạng: "Apikey P"
                    // Cần cắt bỏ chữ "Apikey " để lấy chữ "P"
                    if (!string.IsNullOrEmpty(headerValue) && headerValue.StartsWith("Apikey "))
                    {
                        apiKey = headerValue.Substring(7); // Lấy từ ký tự thứ 7 trở đi
                    }
                }
                await _paymentService.HandleSepayWebhookAsync(payload, apiKey);

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
                return Ok(new
                {
                    success = false,
                    error_type = "Exception (Server Error)",
                    message = ex.Message,
                    stack_trace = ex.ToString() // Cái này sẽ cho bạn biết lỗi ở dòng nào
                });
            }
        }

        [HttpGet("check-status/{registrationId}")]
        public async Task<IActionResult> CheckPaymentStatus(Guid registrationId)
        {
            var status = await _paymentService.GetPaymentStatusAsync(registrationId);

            if (status == null)
            {
                return NotFound(new { message = "Không tìm thấy giao dịch." });
            }

            // Trả về status hiện tại (Pending hoặc Success)
            return Ok(new { status = status });
        }
    }
}