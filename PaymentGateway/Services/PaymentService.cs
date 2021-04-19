using Microsoft.Extensions.Logging;
using PaymentGateway.Contract;
using PaymentGateway.Entities;
using PaymentGateway.Exceptions;
//
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBankServiceFactory _bankServiceFactory;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(IBankServiceFactory bankService, IPaymentRepository paymentRepo, ILogger<PaymentService> logger)
        {
            _bankServiceFactory = bankService;
            _paymentRepo = paymentRepo;
            _logger = logger;
        }

        /// <summary>
        /// Sends request to bank service and saves card information and payment into postgres db
        /// </summary>
        /// <param name="request">Contains amount, currency and card information</param>
        /// <returns>Returns payment response that contains PaymentId(unique value), Status and Message</returns>
        public PaymentResponse DoPayment(PaymentRequest request)
        {
            _logger.LogInformation("PaymentService -> DoPayment started.");
            PaymentResponse paymentResponse;
            try
            {
                paymentResponse = _bankServiceFactory.GetBankService(request).ProcessPayment(request);
            }
            catch (Exception e)
            {
                _logger.LogError("PaymentService -> DoPayment External bank service throwed exception! : " + e.Message);
                throw new BankServiceException();
            }      
            var cardId = SaveCardInfo(request);
            SavePayment(request, paymentResponse, cardId);
            _logger.LogInformation("PaymentService -> DoPayment ends and return.");
            return paymentResponse;
        }

        /// <summary>
        /// Get payment detail by PaymentId (unique value for payment)
        /// </summary>
        /// <param name="paymentId">PaymentId (unique value for payment)</param>
        /// <returns>Returns payment information details</returns>
        public PaymentInformation GetPayment(Guid paymentId)
        {
            _logger.LogInformation("PaymentService -> GetPayment started.");
            var paymentEntity = _paymentRepo.GetPayment(paymentId);
            CardInformationEntity cardInfoEntity = null;

            if (paymentEntity != null)
            {
                cardInfoEntity = _paymentRepo.GetCardInformation(paymentEntity.CardId);
            }
            else
                throw new PaymentNotFoundException();
            var paymentInfo = MapToPaymentInformation(paymentEntity, cardInfoEntity);
            _logger.LogInformation("PaymentService -> GetPayment ends.");
            return paymentInfo;
        }

        #region [Private Methods]

        /// <summary>
        /// Creates PaymentEntity by parameters and saves entity into postgres db.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="paymentResponse"></param>
        /// <param name="cardId"></param>
        /// <returns>Returns auto-incremented row id from db</returns>
        private long SavePayment(PaymentRequest request, PaymentResponse paymentResponse, long cardId)
        {
            _logger.LogInformation("PaymentService -> SavePayment started.");
            var paymentEntity = MapToPaymentEntity(request, paymentResponse, cardId);
            _paymentRepo.SavePayment(paymentEntity);
            _logger.LogInformation("PaymentService -> SavePayment ends.");
            return paymentEntity.Id;
        }

        /// <summary>
        /// Creates PaymentEntity instance and map properties by parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="paymentResponse"></param>
        /// <param name="cardId"></param>
        /// <returns>Returns PaymentEntity object</returns>
        private PaymentEntity MapToPaymentEntity(PaymentRequest request, PaymentResponse paymentResponse, long cardId)
        {
            _logger.LogInformation("PaymentService -> MapToPaymentEntity started.");
            var paymentEntity = new PaymentEntity()
            {
                CardId = cardId,
                Code = paymentResponse.PaymentId,
                Amount = request.Amount,
                Currency = request.Currency,
                Message = paymentResponse.Message,
                Status = paymentResponse.Status
            };
            _logger.LogInformation("PaymentService -> MapToPaymentEntity ends.");
            return paymentEntity;
        }
        /// <summary>
        /// Saves card information into postgres db.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns auto-incremented row id from db</returns>
        private long SaveCardInfo(PaymentRequest request)
        {
            _logger.LogInformation("PaymentService -> SaveCardInfo started.");
            var cardInfoEntity= MapToCardInfoEntity(request);
            _paymentRepo.SaveCardInformation(cardInfoEntity);
            _logger.LogInformation("PaymentService -> SaveCardInfo ends.");
            return cardInfoEntity.Id;
        }

        /// <summary>
        /// Creates CardInformationEntity instance and map properties by parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="paymentResponse"></param>
        /// <param name="cardId"></param>
        /// <returns>Returns CardInformationEntity object</returns>
        private CardInformationEntity MapToCardInfoEntity(PaymentRequest request)
        {
            _logger.LogInformation("PaymentService -> MapToCardInfoEntity started.");
            var cardInfo = new CardInformationEntity()
            {
                CardNumber = request.CardInformation.CardNumber,
                ExpiryMonth = request.CardInformation.ExpiryMonth,
                ExpiryYear = request.CardInformation.ExpiryYear,
                Cvv = request.CardInformation.Cvv,
            };
            _logger.LogInformation("PaymentService -> MapToCardInfoEntity ends.");
            return cardInfo;
        }

        /// <summary>
        /// Creates PaymentInformation instance and map properties by parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="paymentResponse"></param>
        /// <param name="cardId"></param>
        /// <returns>Returns PaymentInformation object</returns>
        private PaymentInformation MapToPaymentInformation(PaymentEntity paymentEntity, CardInformationEntity cardInfoEntity)
        {
            _logger.LogInformation("PaymentService -> MapToPaymentInformation started.");
            var paymentInformation = new PaymentInformation()
            {
                CardInfo = new CardInformation()
                {
                    CardNumber = MaskCardNumber(cardInfoEntity.CardNumber),
                    Cvv = cardInfoEntity.Cvv,
                    ExpiryMonth = cardInfoEntity.ExpiryMonth,
                    ExpiryYear = cardInfoEntity.ExpiryYear
                },
                Amount = paymentEntity.Amount,
                Currency = paymentEntity.Currency,
                PaymentId = paymentEntity.Code,
                Status = paymentEntity.Status,
                Message = paymentEntity.Message
            };
            _logger.LogInformation("PaymentService -> MapToPaymentInformation ends.");
            return paymentInformation;
        }

        /// <summary>
        /// Creates a masked string for credit card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>Returns masked credit card</returns>
        private string MaskCardNumber(string cardNumber)
        {
            _logger.LogInformation("PaymentService -> MaskCardNumber started.");
            return new StringBuilder().Append(cardNumber.Substring(0, 4)).Append("-****-****-**").Append(cardNumber.Substring(14, 2)).ToString();
        }
        #endregion
    }
}
