using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Contract;
using PaymentGateway.Entities;
using PaymentGateway.Exceptions;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services;
using PaymentGateway.Services.Interfaces;
using System;

namespace PaymentGateway.Test
{
    public class PaymentServiceTests
    {
        private readonly IPaymentRepository _mockPaymentRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IBankServiceFactory _mockBankServiceFactory;
        private PaymentRequest _paymentRequest;
        private PaymentRequest _paymentRequest2;
        private PaymentResponse _successfulPaymentResponse;
        private PaymentResponse _unsuccessfulPaymentResponse;        
        private PaymentEntity _paymentResponseFromRepo;
        private CardInformationEntity _cardInformation;
        public PaymentServiceTests()
        {
            CreateTestObject();            


            Mock<IBankServiceFactory> bankServiceFactory = new Mock<IBankServiceFactory>(MockBehavior.Default);
            Mock<IBankService> mockBankService = new Mock<IBankService>();
            mockBankService.Setup(x => x.ProcessPayment(_paymentRequest)).Returns(_successfulPaymentResponse);
            mockBankService.Setup(x => x.ProcessPayment(_paymentRequest2)).Returns(_unsuccessfulPaymentResponse);
            bankServiceFactory.Setup(x => x.GetBankService(_paymentRequest)).Returns(mockBankService.Object);
            bankServiceFactory.Setup(x => x.GetBankService(_paymentRequest2)).Returns(mockBankService.Object);
            Mock<IPaymentRepository> mockPaymentRepository = new Mock<IPaymentRepository>(MockBehavior.Default);
                       

            mockPaymentRepository.Setup(x => x.SavePayment(It.IsAny<PaymentEntity>()));
            mockPaymentRepository.Setup(x => x.SaveCardInformation(It.IsAny<CardInformationEntity>()));
            mockPaymentRepository.Setup(x => x.GetPayment(It.IsAny<Guid>())).Returns(_paymentResponseFromRepo);
            mockPaymentRepository.Setup(x => x.GetCardInformation(It.IsAny<long>())).Returns(_cardInformation);
            _mockPaymentRepository = mockPaymentRepository.Object;
            _mockBankServiceFactory = bankServiceFactory.Object;

            _logger = Mock.Of<ILogger<PaymentService>>();
        }

        private void CreateTestObject()
        {
            _paymentRequest = new PaymentRequest()
            {
                Amount = 900,
                Currency = "EUR",
                CardInformation = new CardInformation()
                {
                    CardNumber = "1111222233334444",
                    ExpiryMonth = 12,
                    ExpiryYear = 2021,
                    Cvv = 123
                }
            };

            _paymentRequest2 = new PaymentRequest()
            {
                Amount = 1500,
                Currency = "EUR",
                CardInformation = new CardInformation()
                {
                    CardNumber = "1111222233334444",
                    ExpiryMonth = 12,
                    ExpiryYear = 2021,
                    Cvv = 123
                }
            };

            _successfulPaymentResponse = new PaymentResponse()
            {
                PaymentId = new Guid("10013B70-6182-48E7-8222-1E14EBF86000"),
                Message = "Successful",
                Status = true
            };

            _unsuccessfulPaymentResponse = new PaymentResponse()
            {
                PaymentId = new Guid("10013B70-6182-48E7-8222-1E14EBF86111"),
                Message = "Unsuccessful",
                Status = false
            };

            _paymentResponseFromRepo = new PaymentEntity()
            {
                Amount = 899,
                CardId = 1,
                Code = new Guid("10013B70-6182-48E7-8222-1E14EBF866CF"),
                Currency = "EUR",
                Message = "Successful",
                Status = true
            };

            _cardInformation = new CardInformationEntity()
            {
                CardNumber = "1111222233334444",
                Cvv = 123,
                ExpiryMonth = 12,
                ExpiryYear = 2022
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DoPayment_GetSuccess()
        {
            var paymentService = new PaymentService(_mockBankServiceFactory, _mockPaymentRepository, _logger);
            var paymentResponse = paymentService.DoPayment(_paymentRequest);
            Assert.IsNotNull(paymentResponse);
            Assert.AreEqual(paymentResponse.PaymentId, new Guid("10013B70-6182-48E7-8222-1E14EBF86000"));
        }

        [Test]
        public void DoPayment_GetBankServiceException()
        {
            var paymentService = new PaymentService(_mockBankServiceFactory, _mockPaymentRepository, _logger);
            try
            {
                var paymentResponse = paymentService.DoPayment(_paymentRequest2);
            }
            catch (BankServiceException e)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void GetPayment_GetSuccess()
        {
            var paymentService = new PaymentService(_mockBankServiceFactory, _mockPaymentRepository, _logger);
            var paymentResponse = paymentService.GetPayment(new Guid("10013B70-6182-48E7-8222-1E14EBF866CF"));
            Assert.IsNotNull(paymentResponse);
            Assert.AreEqual(paymentResponse.PaymentId, new Guid("10013B70-6182-48E7-8222-1E14EBF866CF"));
        }

    }
}