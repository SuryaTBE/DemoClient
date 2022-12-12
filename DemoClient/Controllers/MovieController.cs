using DemoClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DemoClient.Controllers
{
    public class MovieController : Controller
    {
       

        public async Task<IActionResult> Index()
        {
            List<MovieTbl> movie = new List<MovieTbl>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:44341/api/Movie");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    movie = JsonConvert.DeserializeObject<List<MovieTbl>>(response);
                }
                return View(movie);
            }
        }
        public async Task<IActionResult> Search(DateTime date)
        {
            List<MovieTbl> movie = new List<MovieTbl>();
            using(var client=new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("applicaation/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:44341/api/Movie/" + date);
                if(Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    movie = JsonConvert.DeserializeObject<List<MovieTbl>>(response);
                }
                return View(movie);
            }
        }
        //public async Task<IActionResult> Details(int id)
        {
            using (var client=new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("applicaation/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:44341/api/Movie/" + id);
            }
        }
        //public async Task<IActionResult> Search(DateTime searchdate)
        //{
        //    if (searchdate > DateTime.Now)
        //    {
        //        var movies = from m in _context.MovieTbls
        //                     select m;
        //        movies = movies.Where(s => s.Date!.Equals(searchdate));
        //        return View(await movies.ToListAsync());
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Please enter the valid date..";
        //        return View("Index");
        //    }


        //}
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieTbl movietbl)
        {
            MovieTbl movie = new MovieTbl();
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(movietbl), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync("https://localhost:44341/api/Movie", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    movie = JsonConvert.DeserializeObject<MovieTbl>(apiresponse);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
