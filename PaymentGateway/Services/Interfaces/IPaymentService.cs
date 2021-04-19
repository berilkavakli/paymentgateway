using PaymentGateway.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services.Interfaces
{
    public interface IPaymentService
    {
        PaymentResponse DoPayment(PaymentRequest request);
        PaymentInformation GetPayment(Guid paymentId);
    }
}
