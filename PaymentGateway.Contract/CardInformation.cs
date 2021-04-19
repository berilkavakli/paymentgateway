using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PaymentGateway.Contract
{
    [Serializable]
    public class CardInformation
    {
        [DataMember]
        public string CardNumber { get; set; }
        [DataMember]
        public int ExpiryMonth { get; set; }
        [DataMember]
        public int ExpiryYear { get; set; }
        [DataMember]
        public int Cvv { get; set; }
    }
}
