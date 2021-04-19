using PaymentGateway.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services.Interfaces
{
    public interface IBankServiceFactory
    {
        IBankService GetBankService(PaymentRequest paymentRequest);
    }
}
