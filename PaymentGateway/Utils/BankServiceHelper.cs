using Newtonsoft.Json;
using PaymentGateway.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PaymentGateway.Utils
{
    public static class BankServiceHelper
    {
        public static PaymentResponse CallBankService(string serviceAddress, PaymentRequest paymentRequest)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serviceAddress);
                client.DefaultRequestHeaders.Clear();

                var requestContent = JsonConvert.SerializeObject(paymentRequest);
                var buffer = System.Text.Encoding.UTF8.GetBytes(requestContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var postTask = client.PostAsync("payment", byteContent);
                postTask.Wait();

                var result = postTask.Result;
                var response = new PaymentResponse();
                if (result.IsSuccessStatusCode)
                {
                    string jsonContent = result.Content.ReadAsStringAsync().Result;
                    response = JsonConvert.DeserializeObject<PaymentResponse>(jsonContent);
                }
                else
                {
                    response.Status = false;
                }
                return response;
            }

        }
    }
}
