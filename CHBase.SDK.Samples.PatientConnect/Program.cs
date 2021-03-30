using CHBase.SDK.Web;
using CHBase.SDK.Web.Authentication;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace CHBase.SDK.Samples.PatientConnect
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            Guid appId = new Guid(appSettings["ApplicationId"]);            
            var certbytes = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ApplicationCertificateFilename"]);
            X509Certificate2 cert = new X509Certificate2(certbytes, ConfigurationManager.AppSettings["AppCertPassword"]);
            WebApplicationCredential cred = new WebApplicationCredential(appId, cert);

            OfflineWebApplicationConnection connection = new OfflineWebApplicationConnection(cred, Guid.Empty);
            string id = CHBase.SDK.PatientConnect.PatientConnection.Create(connection, "John Doe", "Question", "Answer", null, "some-patient-id");
            
            Console.WriteLine(id);
            Console.ReadKey();
        }
    }
}
