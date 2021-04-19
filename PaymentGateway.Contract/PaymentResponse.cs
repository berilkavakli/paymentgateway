using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PaymentGateway.Contract
{
    [Serializable]
    public class PaymentResponse
    {
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public Guid PaymentId { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
