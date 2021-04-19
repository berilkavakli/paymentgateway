using Microsoft.Extensions.Configuration;
using PaymentGateway.Contract;

using PaymentGateway.Services.Interfaces;
using PaymentGateway.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class BankServiceB : IBankService
    {
        private readonly IConfiguration _configuration;
        public BankServiceB(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get related API address from appsetting.json file and send payment request to bankAPI.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns payment response</returns>
        public PaymentResponse ProcessPayment(PaymentRequest request)
        {
            var baseUrl = _configuration.GetValue<string>("AppSettings:BankServiceBUrl");
            var response = BankServiceHelper.CallBankService(baseUrl, request);
            return response;
        }
    }
}
