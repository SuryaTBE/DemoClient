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
        string Baseurl = "https://localhost:44341/api/";
        public async Task<IActionResult> Index()
        {
            List<OrderDetailTbl> detail = new List<OrderDetailTbl>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("OrderDetails");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    detail = JsonConvert.DeserializeObject<List<OrderDetailTbl>>(response);
                }
                return View(detail);
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            OrderDetailTbl od = new OrderDetailTbl();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("OrderDetails/Details?id=" + id))
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
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Baseurl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("OrderDetails?id="+id, content);
            }
                return RedirectToAction("Index");
        }
    }
}
