using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Attributes;
using PaymentGateway.Contract;
using PaymentGateway.Exceptions;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Utils;
using PaymentGateway.Validation;

namespace PaymentGateway.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [GatewayAuthentication]
    public class PaymentController : ControllerBase
    {
        IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Sends payment request to bank service
        /// </summary>
        /// <param name="request">Contains card information, amount and currency</param>
        /// <returns>Returns payment response with status, message and paymentId</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public IActionResult DoPayment(PaymentRequest request)
        {
            try
            {
                var validator = new PaymentValidator();
                var validationResult = validator.Validate(request);
                if (validationResult.IsValid)
                {
                    _logger.LogInformation("Do payment process started.");
                    var paymentResponse = _paymentService.DoPayment(request);
                    if(!paymentResponse.Status)
                        return ResponseHelper.Instance.BadRequestResponseCreator(new List<string>() { paymentResponse.Message });
                    return ResponseHelper.Instance.OkResponseCreator(paymentResponse);
                }
                else
                    return ResponseHelper.Instance.BadRequestResponseCreator(validationResult.Errors.Select(p => p.ErrorMessage).ToList());
            }
            catch(BankServiceException e)
            {
                _logger.LogError(e.ToString());
                return ResponseHelper.Instance.InternalServerErrorResponseCreator(e.Message); 
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ResponseHelper.Instance.BadRequestResponseCreator(new List<string>() { e.Message });
            }
        }

        /// <summary>
        /// Get payment details by paymentId
        /// </summary>
        /// <param name="paymentId"> PaymentId is a unique id that is given by bank service.</param>
        /// <returns>Returns payment details with masked credit card number</returns>
        [HttpGet]
        [Route("{paymentId}")]
        [ProducesResponseType(typeof(PaymentInformation), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public IActionResult GetPayment(Guid paymentId)
        {
            try
            {
                _logger.LogInformation("GetPayment process started.");
                var paymentInfo = _paymentService.GetPayment(paymentId);
                return ResponseHelper.Instance.OkResponseCreator(paymentInfo);
            }
            catch (PaymentNotFoundException e)
            {
                _logger.LogError(e.ToString());
                return ResponseHelper.Instance.NotFoundResponseCreator(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return ResponseHelper.Instance.BadRequestResponseCreator(new List<string>() { e.Message });
            }
            
        }

    }
}

