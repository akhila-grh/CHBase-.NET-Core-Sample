using CHBase.SDK;
using CHBase.SDK.ItemTypes;
using CHBase.SDK.Web;
using CHBase.SDK.Web.Authentication;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace OfflineAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid appId = new Guid(ConfigurationManager.AppSettings["ApplicationId"]);
            Guid personID = new Guid(ConfigurationManager.AppSettings["PersonID"]);

            var certbytes = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ApplicationCertificateFilename"]);            
            X509Certificate2 cert = new X509Certificate2(certbytes, ConfigurationManager.AppSettings["AppCertPassword"]);

            WebApplicationCredential cred = new WebApplicationCredential(appId, cert);
            OfflineWebApplicationConnection connection = new OfflineWebApplicationConnection(cred, personID);
            PersonInfo personInfo = connection.GetPersonInfo();
            AddRandomHeightEntry(personInfo);
            var items = ReadHeight(personInfo);

            foreach(var item in items[0])
            {
                Console.WriteLine("Heights :");
                Console.WriteLine(((Height)item).Value.ToString());
            }

            Console.ReadKey();
        }

        private static ReadOnlyCollection<HealthRecordItemCollection> ReadHeight(PersonInfo personInfo)
        {
            HealthRecordSearcher searcher = personInfo.SelectedRecord.CreateSearcher();
            HealthRecordFilter filter = new HealthRecordFilter();
            filter.TypeIds.Add(Height.TypeId);
            searcher.Filters.Add(filter);

            //To get blob
            //var item = searcher.GetSingleItem(new Guid("ef21ac67-e525-4a6e-ae5a-e54d3b75c80e"), HealthRecordItemSections.All);
            //var itemBlob = item.GetBlobStore(personInfo.SelectedRecord);
            //foreach(var blob in itemBlob.Values)
            //{
            //    Console.WriteLine(blob.Name);
            //}

            ReadOnlyCollection<HealthRecordItemCollection> items = searcher.GetMatchingItems();
            return items;
        }
        private static void AddRandomHeightEntry(PersonInfo personInfo)
        {
            Random random = new Random();

            double meters = random.NextDouble() * 0.5 + 1.5;

            Length value = new Length(meters);
            Height height = new Height(new HealthServiceDateTime(DateTime.Now), value);

            //var blobstore = height.GetBlobStore(personInfo.SelectedRecord);
            //var blobItem = blobstore.NewBlob("test", "text");
            //var blobBytes = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["SampleBlobFile"]);
            //var blobStream = blobItem.GetWriterStream();
            //blobStream.Write(blobBytes, 0, blobBytes.Length);
            //blobStream.Close();
            personInfo.SelectedRecord.NewItem(height);
        }

    }
}
