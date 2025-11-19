using EventPlatform.Application.Services.Interfaces.Payment;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EventPlatform.Infrastructure.Services.Payment
{
    public class SepayQrService : ISepayQrService
    {
        private readonly string _accountNumber;
        private readonly string _bankName;

        public SepayQrService(IConfiguration configuration)
        {
            _accountNumber = configuration["SEPAY_ACCOUNT_NUMBER"];
            _bankName = configuration["SEPAY_BANK_NAME"];
        }

        public string GeneratePaymentQrCodeUrl(decimal amount, string description)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "acc", _accountNumber },
                { "bank", _bankName },
                { "amount", amount.ToString("0") }, 
                { "des", description },
                { "template", "compact" } 
            };

            var uriBuilder = new UriBuilder("https://qr.sepay.vn/img");
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }
    }
}
