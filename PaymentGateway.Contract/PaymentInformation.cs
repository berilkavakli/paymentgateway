using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PaymentGateway.Contract
{
    [Serializable]
    public class PaymentInformation
    {
        [DataMember]
        public Guid PaymentId { get; set; }
        [DataMember]
        public CardInformation CardInfo { get; set; }
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string Message { get; set; }

    }
}
