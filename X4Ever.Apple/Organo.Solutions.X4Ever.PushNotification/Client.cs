using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using static System.Console;
    using System.Configuration;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

namespace Organo.Solutions.XChallenge.PushNotification
{
    class Client
    {
        //private string Url => "https://api.push.apple.com/3/device/<your token>";
        private string APNs_Request_Url => ConfigurationManager.AppSettings[Keys.APNS_REQUEST_URL];
        private string CertificatePath => ConfigurationManager.AppSettings[Keys.CERTIFICATE_PATH];
        private string CertificatePass => ConfigurationManager.AppSettings[Keys.CERTIFICATE_PASSWORD];
        HttpContent _content;
        private X509CertificateCollection certificate;

        public void SendNotification()
        {
            // Create the token source.
            CancellationTokenSource cts = new CancellationTokenSource();

            // Pass the token to the cancelable operation.
            ThreadPool.QueueUserWorkItem(new WaitCallback(Request), cts.Token);
            Thread.Sleep(2000);

            // Request cancellation.
            WriteLine("Cancellation set in token source...");
            Thread.Sleep(2500);

            // Cancellation should have happened, so call Dispose.
            cts.Dispose();
        }

        async void Request(object obj)
        {
            CancellationToken token = (CancellationToken) obj;


            /*{
          "agent": {                             
            "name": "Agent Name",                
            "version": 1                                                          
          },
          "username": "Username",                                   
          "password": "User Password",
          "token": "xxxxxx"
        }*/

            JObject payLoad = new JObject(
                new JProperty("agent",
                    new JObject(
                        new JProperty("name", "Agent Name"),
                        new JProperty("version", 1)
                    ),
                    new JProperty("username", "Username"),
                    new JProperty("password", "User Password"),
                    new JProperty("token", "xxxxxx")
                )
            );

            using (HttpClient client = new HttpClient(new Http2CustomHandler()))
            {
                var httpContent = new StringContent(payLoad.ToString(), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await client.PostAsync(new Uri(string.Format(APNs_Request_Url)), httpContent, token))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jObject = JObject.Parse(responseBody);
                }
            }

            if (!token.IsCancellationRequested)
            {
                WriteLine("In iteration {0}, cancellation has been requested...");


                //client.PostAsync()

                // Perform cleanup if necessary.
                //...
                // Terminate the operation.
            }
        }

        void NewMethod()
        {
            X509Certificate2 cert = new X509Certificate2(CertificatePath, CertificatePass);
            certificate = new X509CertificateCollection();
            certificate.Add(cert);


            //For distribution mode, the host is gateway.push.apple.com    
            //For development mode, the host is gateway.sandbox.push.apple.com
            TcpClient client = new TcpClient("gateway.push.apple.com", 2195);

            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ServerCertificateValidationCallback), null);

            //The method AuthenticateAsClient() may cause an exception, so we need to try..catch.. it
            try
            {
                //Reference of SslStream 
                //https://msdn.microsoft.com/en-us/library/system.net.security.sslstream(v=vs.110).aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2

                sslStream.AuthenticateAsClient("gateway.push.apple.com", certificate, SslProtocols.Default, false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Message: {0} ", e.Message);
                sslStream.Close();
            }

            //Obviously , this is a method of callback when handshaking
            bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    Console.WriteLine("Specified Certificate is accepted.");
                    return true;
                }
                Console.WriteLine("Certificate error : {0} ", sslPolicyErrors);
                return false;
            }
        }



        //var payload = new Credentials
        //{
        //    Agent = new Agent
        //    {
        //        Name = "Agent Name",
        //        Version = 1
        //    },
        //    Username = "Username",
        //    Password = "User Password",
        //    Token = "xxxxx"
        //};

        //// Serialize our concrete class into a JSON String
        //var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));

        //// Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        //var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

        //    using (var httpClient = new HttpClient()) {

        //    // Do the actual request and await the response
        //    var httpResponse = await httpClient.PostAsync("http://localhost/api/path", httpContent);

        //    // If the response contains content we want to read it!
        //    if (httpResponse.Content != null) {
        //        var responseContent = await httpResponse.Content.ReadAsStringAsync();

        //        // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
        //    }
        //}
    }
}