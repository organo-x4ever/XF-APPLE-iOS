using System;
using System.Configuration;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Organo.Solutions.XChallenge.PushNotification
{
    class RemoteClient
    {
        private string APNSRequestUrl => new ConfigurationData().Get(Keys.APNS_REQUEST_URL);
        private short APNSRequestUrlPort => new ConfigurationData().GetShort(Keys.APNS_REQUEST_URL_PORT);
        private string CertificatePath => new ConfigurationData().Get(Keys.CERTIFICATE_PATH);
        private string CertificatePass => new ConfigurationData().Get(Keys.CERTIFICATE_PASSWORD);
        private string Host => new ConfigurationData().Get(Keys.APNS_HOST);
        private readonly X509CertificateCollection certificate;
        private SslStream _sslStream;

        //We gave values to it to consisting of the payload content
        PushNotificationPayload _payload;

        public RemoteClient()
        {
            X509Certificate2 cert = new X509Certificate2(CertificatePath, CertificatePass);
            certificate = new X509CertificateCollection();
            certificate.Add(cert);

            _payload = new PushNotificationPayload();
            _payload.deviceToken = "dc67b56c eb5dd9f9 782c37fd cfdcca87 3b7bc77c 3b090ac4 c538e007 a2f23a24";
            _payload.badge = 56789;
            _payload.sound = "default";
            _payload.message = "This message was pushed by C# platform.";
        }

        public void SendRequest()
        {
            //For distribution mode, the host is gateway.push.apple.com    
            //For development mode, the host is gateway.sandbox.push.apple.com
            TcpClient client = new TcpClient(APNSRequestUrl, APNSRequestUrlPort);

            _sslStream = new SslStream(client.GetStream(), false,
                new RemoteCertificateValidationCallback(ServerCertificateValidationCallback), null);

            //The method AuthenticateAsClient() may cause an exception, so we need to try..catch.. it

            try
            {
                // Reference of SSL
                //https://msdn.microsoft.com/en-us/library/system.net.security.sslstream(v=vs.110).aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2

                _sslStream.AuthenticateAsClient(Host, certificate, SslProtocols.Default, false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Message: {0} ", e.Message);
                _sslStream.Close();
            }
        }

        //Obviously , this is a method of callback when handshaking
        bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                Console.WriteLine("Specified Certificate is accepted.");
                return true;
            }

            Console.WriteLine("Certificate error : {0} ", sslPolicyErrors);
            return false;
        }

        //This is definition of PushNotificationPayload of struct 

        public struct PushNotificationPayload
        {
            public string deviceToken;
            public string message;
            public string sound;
            public int badge;

            public string PushPayload()
            {
                return "{\"aps\":{\"alert\":\"" + message + "\",\"badge\":" + badge + ",\"sound\":\"" + sound + "\"}}";
            }
        }

        //And then, calling Push() method to invoke it
        public void Push(PushNotificationPayload payload)
        {
            string payloadStr = payload.PushPayload();
            string deviceToken = payload.deviceToken;

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write((byte) 0); //The command
            writer.Write((byte) 0); //The first byte of deviceId length (Big-endian first byte)
            writer.Write((byte) 32); //The deviceId length (Big-endian second type)

            //Method of DataWithDeviceToken() , see source code in this repo.
            byte[] deviceTokenBytes = DataWithDeviceToken(deviceToken.ToUpper());
            writer.Write(deviceTokenBytes);

            writer.Write((byte) 0); //The first byte of payload length (Big-endian first byte)
            writer.Write((byte) payloadStr.Length); //payload length (Big-endian second byte)

            byte[] bytes = Encoding.UTF8.GetBytes(payloadStr);
            writer.Write(bytes);
            writer.Flush();

            _sslStream.Write(memoryStream.ToArray());
            _sslStream.Flush();

            Thread.Sleep(3000);

            //Method of ReadMessage() , see source code in this repo.
            string result = ReadMessage(_sslStream);
            Console.WriteLine("server said: " + result);

            _sslStream.Close();
        }

        string ReadMessage(SslStream stream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = stream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>", StringComparison.Ordinal) != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        byte[] DataWithDeviceToken(string deviceToken)
        {
            return Encoding.UTF8.GetBytes(deviceToken);
        }

        public struct ConfigurationData
        {
            public string Get(string key)
            {
                return ConfigurationManager.AppSettings[key];
            }

            public short GetShort(string key)
            {
                var data = ConfigurationManager.AppSettings[key];
                if (data != null)
                {
                    short.TryParse(data, out var d);
                    return d;
                }

                return 0;
            }
        }
    }
}