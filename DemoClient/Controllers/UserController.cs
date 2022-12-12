using DemoClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace DemoClient.Controllers
{
    public class UserController : Controller
    {
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserTbl usertbl)
        {
            using(var client=new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(usertbl), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:44341/api/Credentials", content);
                ViewBag.SuccessMessageA = "You are successfully registered...";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserTbl usertbl)
        {
            UserTbl user = new UserTbl();
            using(var client=new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                StringContent content=new StringContent(JsonConvert.SerializeObject(usertbl),Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:44341/api/Credentials/UserLogin", content);
                if (response.IsSuccessStatusCode)
                {
                    string apiresponse=await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<UserTbl>(apiresponse);
                    HttpContext.Session.SetString("UserName", user.UserName);
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
