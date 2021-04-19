using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PaymentGateway.Contract
{
    [Serializable]
    public class PaymentRequest
    {
        [DataMember]
        public CardInformation CardInformation { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public string Currency { get; set; }
    }
}
