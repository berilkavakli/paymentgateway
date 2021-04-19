using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentGateway.Contract;

using PaymentGateway.Services.Interfaces;
using PaymentGateway.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class BankServiceA : IBankService
    {
        private readonly IConfiguration _configuration;
        public BankServiceA(IConfiguration configuration)
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
            var baseUrl = _configuration.GetValue<string>("AppSettings:BankServiceAUrl");
            var response = BankServiceHelper.CallBankService(baseUrl, request);            
            return response;
        }
    }
}
