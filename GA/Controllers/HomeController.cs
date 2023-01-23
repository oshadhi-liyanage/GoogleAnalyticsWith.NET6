using GA.Models;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.AspNetCore3;
using Google.Analytics.Data.V1Beta;


namespace GA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()

        {
            return View();
        }


        [GoogleScopedAuthorize("https://www.googleapis.com/auth/analytics.readonly")]
        public async void RetriveReports([FromServices] IGoogleAuthProvider auth)
        {
            //paste your property id here
            string propertyId = "";
            var clientBuilder = new BetaAnalyticsDataClientBuilder()
            {
                Credential = await auth.GetCredentialAsync()
            };
            var client = await clientBuilder.BuildAsync();

            RunReportRequest request = new RunReportRequest
            {
                Property = "properties/" + propertyId,
                Dimensions = { new Dimension { Name = "country" }, },
                Metrics = { new Metric { Name = "activeUsers" }, },
                DateRanges = {new DateRange { StartDate = "2023-01-17", EndDate = "2023-01-18" }, },
            };

            // Make the request
            var response = client.RunReport(request);
            Console.WriteLine("Report result:");
            foreach (Row row in response.Rows)
            {
                Console.WriteLine("{0}, {1}", row.DimensionValues[0].Value, row.MetricValues[0].Value);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}