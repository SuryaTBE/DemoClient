using DemoClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace DemoClient.Controllers
{
    public class AdminController : Controller
    {
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminTbl admintbl)
        {
            AdminTbl admin = new AdminTbl();
            using(var client=new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                StringContent content=new StringContent(JsonConvert.SerializeObject(admintbl),Encoding.UTF8,"application/json");
                var response = await client.PostAsync("https://localhost:44341/api/Credentials/AdminLogin", content);
                if(response.IsSuccessStatusCode)
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    admin= JsonConvert.DeserializeObject<AdminTbl>(apiresponse);
                    HttpContext.Session.SetString("AdminName",admin.AdminName);
                    return RedirectToAction("Index", "Movie");
                }
                else
                {
                    ViewBag.ErrorMessageA = "*Invalid Email-ID or Password";
                    //return RedirectToAction("AdminLogin");
                    return View();
                }
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
