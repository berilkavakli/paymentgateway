using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Contract;

namespace BankServiceB.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public PaymentResponse ProcessPayment(PaymentRequest paymentRequest)
        {
            var response = new PaymentResponse() { PaymentId = Guid.NewGuid() };
            var isValid = ValidateCard(paymentRequest.CardInformation);
            if (isValid)
            {
                if (paymentRequest.Amount < 2000)
                {
                    response.Status = true;
                    response.Message = "Successfull";
                }
                else
                {
                    response.Status = false;
                    response.Message = "Insufficient card limit!";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Card is invalid";
            }
            SendPayment(paymentRequest, response);
            return response;
        }

        private void SendPayment(PaymentRequest paymentRequest, PaymentResponse paymentResponse)
        {
            //Sends payment to 3rd party applications

        }

        private bool ValidateCard(CardInformation cardInfo)
        {
            if (cardInfo.CardNumber.StartsWith("0"))
                return false;
            return true;
        }
    }
}
