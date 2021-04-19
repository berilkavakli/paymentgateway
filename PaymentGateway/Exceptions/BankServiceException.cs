using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Exceptions
{
    public class BankServiceException : Exception
    {
        public override string Message => "External bank service can not be reached!";
    }
}
