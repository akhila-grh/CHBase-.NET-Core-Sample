using CHBase.SDK;
using CHBase.SDK.ItemTypes;
using CHBase.SDK.Web.Authentication;
//using CHBase.SDK.Web;
using CHBaseHelloWorld.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Mvc;

namespace CHBaseHelloWorld.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var appID = new Guid(ConfigurationManager.AppSettings["ApplicationId"]);
            var shellUrl = ConfigurationManager.AppSettings["ShellUrl"];
            string authUrl = $"{shellUrl}redirect.aspx?target=AUTH&targetqs=appid%3D{appID}";
            ViewBag.AuthUrl = authUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Index(object wctoken)
        {
            var token = Request.Form["wctoken"];
            var appID = new Guid(ConfigurationManager.AppSettings["ApplicationId"]);

            var certbytes = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ApplicationCertificateFilename"]);
            X509Certificate2 cert = new X509Certificate2(certbytes, ConfigurationManager.AppSettings["AppCertPassword"]);

            WebApplicationCredential cred = new WebApplicationCredential(appID,token,cert);
            CHBase.SDK.Web.WebApplicationConnection connection = new CHBase.SDK.Web.WebApplicationConnection(appID, cred);
            PersonInfo personInfo = CHBasePlatform.GetPersonInfo(connection);
                
            AddRandomHeightEntry(personInfo);

            ViewData["Height"] = ReadHeight(personInfo).FirstOrDefault();
            ViewData["PersonID"] = personInfo.PersonId.ToString();
            ViewData["Name"] = personInfo.Name;
            ViewData["RecordName"] = personInfo.SelectedRecord.Name;
            return View("Height");
        }

        private static ReadOnlyCollection<HealthRecordItemCollection> ReadHeight(PersonInfo personInfo)
        {
            HealthRecordSearcher searcher = personInfo.SelectedRecord.CreateSearcher();
            HealthRecordFilter filter = new HealthRecordFilter();
            filter.TypeIds.Add(Height.TypeId);
            searcher.Filters.Add(filter);

            ReadOnlyCollection<HealthRecordItemCollection> items = searcher.GetMatchingItems();

            return items;
        }
        private void AddRandomHeightEntry(PersonInfo personInfo)
        {
            Random random = new Random();

            double meters = random.NextDouble() * 0.5 + 1.5;

            Length value = new Length(meters);
            Height height = new Height(new HealthServiceDateTime(DateTime.Now), value);

            personInfo.SelectedRecord.NewItem(height);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
