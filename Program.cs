using System;
using System.Text;
using System.Net.Http;
using Aws4RequestSigner;
using System.IO;
using System.Linq;

namespace SigV4
{
    class Program
    {
        static void Main(string[] args)
        {
            test();
        }

        private static async void test()
        {
            var signer = new AWS4RequestSigner("secret", "key");
            //var cont = new StringContent("query=select ?s ?p ?o where {?s ?p ?o} limit 10", Encoding.UTF8);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("address"),
            };

            request = await signer.Sign(request, "neptune-db", "us-east-1", new TimeSpan(0,2,0));
            using StreamWriter sw = new StreamWriter("result.txt", false);
            sw.WriteLine("x-amz-content-sha256:");
            sw.WriteLine(request.Headers.GetValues("x-amz-content-sha256").First().ToString());
            sw.WriteLine("x-amz-date:");
            sw.WriteLine(request.Headers.GetValues("x-amz-date").First().ToString());
            sw.WriteLine("Authorization:");
            sw.WriteLine(request.Headers.GetValues("Authorization").First().ToString());
            sw.Close();
            request.Headers.Add("x-amz-security-token", "token");

            var client = new HttpClient();
            try
            {
                var response = await client.SendAsync(request);
                var responseStr = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
                       

            Console.ReadLine();
        }
    }
}
