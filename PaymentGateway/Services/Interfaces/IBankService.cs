using PaymentGateway.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services.Interfaces
{
    public interface IBankService
    {

        //bool ValidateCard(CardInformation cardInfo);
        PaymentResponse ProcessPayment(PaymentRequest request);
        //void SendPayment(PaymentInformation paymentInfo);
    }
}
