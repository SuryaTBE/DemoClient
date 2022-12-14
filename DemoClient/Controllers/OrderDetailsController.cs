using DemoClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DemoClient.Controllers
{
    public class OrderDetailsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<OrderDetailTbl> od = new List<OrderDetailTbl>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:44341/api/Booking/OrderListConvert");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    od = JsonConvert.DeserializeObject<List<OrderDetailTbl>>(response);
                }
                return View(od);
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            OrderDetailTbl od = new OrderDetailTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking/OrderDetailsGetById?" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    od = JsonConvert.DeserializeObject<OrderDetailTbl>(apiResponse);
                }
            }
            if (od.MovieDate > DateTime.Now)
            {

                HttpContext.Session.SetInt32("MId", od.MovieId);

                if (od == null)
                {
                    return NotFound();
                }
                return View(od);
            }
            else
            {
                ViewBag.CMessage = "Sorry,Your deadline for canceling is finished.";
                return View("Index");
            }
        }
        public async Task<IActionResult> Cancel(int id)
        {
            OrderDetailTbl od = new OrderDetailTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking/OrderDetailsGetById?" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    od = JsonConvert.DeserializeObject<OrderDetailTbl>(apiResponse);
                }
            }
            int no = od.NoOfTickets;
            int mid = (int)HttpContext.Session.GetInt32("MId");
            MovieTbl m = new MovieTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Movie/" + mid))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    m = JsonConvert.DeserializeObject<MovieTbl>(apiResponse);
                }
            }
            m.capacity += no;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(od), Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:44341/api/Booking/CancelTicket/", content);

            }
            return RedirectToAction("Index");
        }
    }
}
