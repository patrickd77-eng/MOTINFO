using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MOTINFO.Models;
using Newtonsoft.Json;

namespace MOTINFO.Controllers
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
            ViewData["SearchMade"] = false;
            ViewData["Error"] = false;
            return View();
        }


        [HttpPost]
        public IActionResult Index(string registration)
        {

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://beta.check-mot.service.gov.uk/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", "H45cVkA7VzAzeSxFGdhP5NSVA6VgFHOaMq0CnDba");
            var response = client.GetAsync("trade/vehicles/mot-tests?registration=" + registration).Result;


            if (response.IsSuccessStatusCode)
            {
                ViewData["SearchMade"] = true;
                ViewData["Error"] = false;

                Car[] car = JsonConvert.DeserializeObject<Car[]>(response.Content.ReadAsStringAsync().Result);

                ViewData["make"] = car.FirstOrDefault().Make;
                ViewData["model"] = car.FirstOrDefault().Model;
                ViewData["registered"] = car.FirstOrDefault().FirstUsedDate;
                ViewData["fuel"] = car.FirstOrDefault().FuelType;
                ViewData["color"] = car.FirstOrDefault().PrimaryColour;
                ViewData["license"] = car.FirstOrDefault().Registration;

                var motTests = new List<String>();

                StringBuilder failures = new StringBuilder();


                foreach (var item in car[0].MotTests)
                {
                    if (item.TestResult.ToString().Contains("Failed"))
                    {

                        for (var i = 0; i < item.RfrAndComments.Length; i++)
                        {
                            failures.Append(item.RfrAndComments[i].Text + " ");
                        }
                        motTests.Add("Test date: " +
                         item.CompletedDate.ToString()
                         + ", which " +
                         item.TestResult.ToString().ToLower()
                         + ". The mileage was: " +
                         item.OdometerValue + ". Reason(s) for failure: " + failures);
                    }
                    else
                    {
                        motTests.Add("Test date: " +
                         item.CompletedDate.ToString()
                         + ", which " +
                         item.TestResult.ToString().ToLower()
                         + ". The mileage was: " +
                         item.OdometerValue + " " + item.OdometerUnit + ".");

                    }
                }
                ViewData["MotResults"] = motTests;
            }
            else
            {
                ViewData["SearchMade"] = false;
                ViewData["Error"] = true;
                ViewData["ErrorMessage"] = registration + " is not a valid registration or does not exist. Please try again.";
                return View();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
