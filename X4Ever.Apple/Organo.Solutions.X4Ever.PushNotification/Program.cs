using System;

namespace Organo.Solutions.XChallenge.PushNotification
{
    class Program
    {
        private string APNSRequestUrl => new RemoteClient.ConfigurationData().Get(Keys.APNS_REQUEST_URL);
        private short APNSRequestUrlPort => new RemoteClient.ConfigurationData().GetShort(Keys.APNS_REQUEST_URL_PORT);
        private string CertificatePath => new RemoteClient.ConfigurationData().Get(Keys.CERTIFICATE_PATH);
        private string CertificatePass => new RemoteClient.ConfigurationData().Get(Keys.CERTIFICATE_PASSWORD);

        static int Main(string[] args)
        {
            //CLIENT SIDE CODE
            args = new string[2];
            args[0] = new RemoteClient.ConfigurationData().Get(Keys.APNS_REQUEST_URL);
            args[1] = new RemoteClient.ConfigurationData().GetShort(Keys.APNS_REQUEST_URL_PORT).ToString();
            //args[2] = new RemoteClient.ConfigurationData().Get(Keys.CERTIFICATE_PATH);

            string serverCertificateName = null;
            string machineName = null;
            short machinePort = 0;
            if (args == null || args.Length < 1)
            {
                SslTcpClient.DisplayUsage();
            }

            // User can specify the machine name and server name.
            // Server name must match the name on the server's certificate. 
            machineName = args[0];
            machinePort = Convert.ToInt16(args[1]);
            if (args.Length < 3)
            {
                serverCertificateName = machineName;
            }
            else
            {
                serverCertificateName = args[2];
            }

            SslTcpClient.RunClient(machineName, machinePort, serverCertificateName);
            return 0;



            // SERVER SIDE CODE
            //string certificate = null;
            //if (args == null || args.Length < 1)
            //{
            //    SslTcpServer.DisplayUsage();
            //}

            //certificate = args[0];
            //SslTcpServer.RunServer(certificate);
            //return 0;
        }
    }
}