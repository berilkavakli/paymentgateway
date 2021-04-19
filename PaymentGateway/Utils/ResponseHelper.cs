using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Utils
{
    public class ResponseHelper
    {
        static ResponseHelper()
        {
        }

        private ResponseHelper()
        {
        }

        public static ResponseHelper Instance { get; } = new ResponseHelper();

        public IActionResult OkResponseCreator(dynamic responseObject)
        {
            return new ObjectResult(responseObject) { StatusCode = StatusCodes.Status200OK };
        }

        public IActionResult BadRequestResponseCreator(dynamic responseObject)
        {
            return new ObjectResult(responseObject) { StatusCode = StatusCodes.Status400BadRequest };
        }

        public IActionResult NotFoundResponseCreator(dynamic responseObject)
        {
            return new ObjectResult(responseObject) { StatusCode = StatusCodes.Status404NotFound };
        }

        public IActionResult InternalServerErrorResponseCreator(dynamic responseObject)
        {
            return new ObjectResult(responseObject) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
