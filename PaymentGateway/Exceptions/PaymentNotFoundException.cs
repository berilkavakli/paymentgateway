using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Exceptions
{
    public class PaymentNotFoundException: Exception
    {
        public override string Message => "Payment not found";
    }
}
