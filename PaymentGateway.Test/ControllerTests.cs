using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Contract;
using PaymentGateway.Controllers;

using PaymentGateway.Services.Interfaces;
using System;

namespace PaymentGateway.Test
{
    public class ControllerTests
    {
        readonly IPaymentService _mockPaymentService;
        readonly ILogger<PaymentController> _logger;

        private readonly PaymentRequest _paymentRequest;
        public ControllerTests()
        {
            Mock<IPaymentService> mockPaymentService = new Mock<IPaymentService>(MockBehavior.Default);
            var response = new PaymentResponse()
            {
                PaymentId = Guid.NewGuid(),
                Message = "Successful",
                Status = true
            };
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
            var paymentDetails = new PaymentInformation()
            {
                PaymentId = new Guid("59513B70-6182-48E7-8222-1E14EBF866CF")
            };

            mockPaymentService.Setup(x => x.DoPayment(It.IsAny<PaymentRequest>())).Returns(response);
            mockPaymentService.Setup(x => x.GetPayment(It.IsAny<Guid>())).Returns(paymentDetails);
            _mockPaymentService = mockPaymentService.Object;

            _logger = Mock.Of<ILogger<PaymentController>>();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SendResquest_GetSuccessfulResponse()
        {
            var paymentController = new PaymentController(_mockPaymentService, _logger);
            var response = paymentController.DoPayment(_paymentRequest);
            Assert.IsTrue(((PaymentResponse)((ObjectResult)response).Value).Status);
        }

        [Test]
        [TestCase("59513B70-6182-48EC-8222-1E14EBF866CF")]
        public void GetPaymentInformation(Guid paymentId)
        {
            var paymentController = new PaymentController(_mockPaymentService, _logger);
            var response = paymentController.GetPayment(paymentId);
            Assert.NotNull(((ObjectResult)response).Value);
        }
    }
}