using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Contract;
using PaymentGateway.Controllers;
using PaymentGateway.Exceptions;
using PaymentGateway.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Test
{
    public class ControllerTests
    {
        readonly IPaymentService _mockPaymentService;
        readonly ILogger<PaymentController> _logger;

        private PaymentRequest _successPaymentRequest;
        private PaymentRequest _badPaymentRequest;
        private PaymentResponse _successResponse;
        private PaymentResponse _badResponse;
        private PaymentInformation _successPaymentInformation;

        public ControllerTests()
        {
            CreateTestObject();

            Mock<IPaymentService> mockPaymentService = new Mock<IPaymentService>(MockBehavior.Default);

            mockPaymentService.Setup(x => x.DoPayment(_successPaymentRequest)).Returns(_successResponse);
            mockPaymentService.Setup(x => x.DoPayment(_badPaymentRequest)).Returns(_badResponse);
            mockPaymentService.Setup(x => x.GetPayment(new Guid("59513B70-6182-48EC-8222-1E14EBF866CF"))).Returns(_successPaymentInformation);
            mockPaymentService.Setup(x => x.GetPayment(new Guid("59513B70-6182-48EC-8222-1E14EBF86600"))).Throws(new PaymentNotFoundException());
            _mockPaymentService = mockPaymentService.Object;

            _logger = Mock.Of<ILogger<PaymentController>>();
        }

        private void CreateTestObject()
        {
            _successResponse = new PaymentResponse()
            {
                PaymentId = Guid.NewGuid(),
                Message = "Successful",
                Status = true
            };

            _badResponse = new PaymentResponse()
            {
                PaymentId = Guid.NewGuid(),
                Message = "Unsuccessful",
                Status = false
            };

            _successPaymentRequest = new PaymentRequest()
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

            _badPaymentRequest = new PaymentRequest()
            {
                Amount = 900,
                Currency = "EUR",
                CardInformation = new CardInformation()
                {
                    CardNumber = "1111222233334444",
                    ExpiryMonth = 12,
                    ExpiryYear = 20219,
                    Cvv = 123
                }
            };

            _successPaymentInformation = new PaymentInformation()
            {
                PaymentId = new Guid("59513B70-6182-48E7-8222-1E14EBF866CF")
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SendResquest_GetSuccessfulResponse()
        {
            var paymentController = new PaymentController(_mockPaymentService, _logger);
            var response = paymentController.DoPayment(_successPaymentRequest);
            var castedResponse = (ObjectResult)response;
            Assert.AreEqual(castedResponse.StatusCode, 200);
            Assert.IsTrue(((PaymentResponse)((ObjectResult)response).Value).Status);
        }

        [Test]
        public void SendResquest_GetBadRequest()
        {
            var paymentController = new PaymentController(_mockPaymentService, _logger);
            var response = paymentController.DoPayment(_badPaymentRequest);
            var castedResponse = (ObjectResult)response;
            var errorList = (List<string>)castedResponse.Value;
            Assert.AreEqual(castedResponse.StatusCode, 400);
            Assert.AreEqual(errorList[0], _badResponse.Message);
        }

        [Test]
        [TestCase("59513B70-6182-48EC-8222-1E14EBF866CF")]
        public void GetPayment_GetSuccessfulResponse(Guid paymentId)
        {
            var paymentController = new PaymentController(_mockPaymentService, _logger);
            var response = paymentController.GetPayment(paymentId);
            var castedResponse = (ObjectResult)response;
            var payment = (PaymentInformation)castedResponse.Value;
            Assert.AreEqual(castedResponse.StatusCode, 200);
            Assert.AreEqual(payment.PaymentId, _successPaymentInformation.PaymentId);
        }

        [Test]
        [TestCase("59513B70-6182-48EC-8222-1E14EBF86600")]
        public void GetPayment_GetNotFound(Guid paymentId)
        {
            var paymentController = new PaymentController(_mockPaymentService, _logger);
            var response = paymentController.GetPayment(paymentId);
            var castedResponse = (ObjectResult)response;
            var error = (string)castedResponse.Value;
            Assert.AreEqual(castedResponse.StatusCode, 404);
            Assert.AreEqual(error, "Payment not found");
        }
    }
}
