using PaymentGateway.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        CardInformationEntity GetCardInformation(long id);
        PaymentEntity GetPayment(Guid code);
        void SavePayment(PaymentEntity payment);
        void SaveCardInformation(CardInformationEntity cardInfo);
    }
}
