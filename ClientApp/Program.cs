using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace ClientApp
{
    class Program
    {
        static string jwtToken = "";
        static string refreshToken = "";

        private static void GetJwt()
        {
            Uri u = new Uri("http://localhost:55869/v1/Login");
            var payload = "{\"Email\": \"tata@gmail.com\" ,\"Password\": \"Dotvik@987\"}";
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();

            //client.DefaultRequestHeaders.Clear();
            var res2 = client.PostAsync(u, c).Result;
            dynamic jwt = JsonConvert.DeserializeObject(res2.Content.ReadAsStringAsync().Result);
            jwtToken = jwt.jwtToken;
            refreshToken = jwt.refreshToken;
        }

        static void Main(string[] args)
        {
            GetJwt();

            Console.WriteLine(jwtToken);
            Console.WriteLine(refreshToken);

            Console.ReadLine();
        }
    }
}
