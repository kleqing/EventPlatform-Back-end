using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Services.Interfaces.Payment
{
    public interface ISepayQrService
    {
        string GeneratePaymentQrCodeUrl(decimal amount, string description);
    }
}
