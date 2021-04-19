using Microsoft.Extensions.Configuration;
using PaymentGateway.Contract;

using PaymentGateway.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class BankServiceFactory : IBankServiceFactory
    {
        readonly IConfiguration _configuration;
        public BankServiceFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Factory that decides which bank service the request sends to
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns>return bank service instance</returns>
        public IBankService GetBankService(PaymentRequest paymentRequest)
        {
            string cardNumber = paymentRequest.CardInformation.CardNumber;
            if (cardNumber.StartsWith("1111"))
                return new BankServiceA(_configuration);
            else
                return new BankServiceB(_configuration);
        }
    }
}
