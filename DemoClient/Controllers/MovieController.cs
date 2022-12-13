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
        [HttpPost]
        public async Task<IActionResult> Search(DateTime date)
        {
            List<MovieTbl> movie = new List<MovieTbl>();
            MovieTbl m=new MovieTbl();
            m.Date=date;
            using(var client=new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content=new StringContent(JsonConvert.SerializeObject(m),Encoding.UTF8,"application/json");
                HttpResponseMessage Res = await client.PostAsync("https://localhost:44341/api/Movie?", content);
                //HttpResponseMessage Res = await client.GetAsync("https://localhost:44341/api/Movie?" + date);
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    movie = JsonConvert.DeserializeObject<List<MovieTbl>>(response);
                }                    
                return View(movie);
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            MovieTbl m = new MovieTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Movie/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    m = JsonConvert.DeserializeObject<MovieTbl>(apiResponse);
                }
            }
            return View(m);
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
        public async Task<IActionResult> BookNow(int? id)
        {
            MovieTbl m = new MovieTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Movie/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    m = JsonConvert.DeserializeObject<MovieTbl>(apiResponse);
                }
            }
            HttpContext.Session.SetString("Moviename", m.MovieName);
            HttpContext.Session.SetInt32("MovieId", m.MovieId);
            HttpContext.Session.SetString("MovieName", m.MovieName);
            HttpContext.Session.SetInt32("Cost", m.Cost);
            HttpContext.Session.SetString("Date", m.Date.ToString());
            HttpContext.Session.SetString("Slot",   m.Slot);
            HttpContext.Session.SetInt32("Capacity", m.capacity);
            if (m.capacity <= 0)
            {
                ViewBag.ErrMessage = "HouseFull";
                return RedirectToAction("Search");
            }
            if (m == null)
            {
                return NotFound();
            }
            return RedirectToAction("Create", "Booking");
        }
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
                using (var response = await client.PostAsync("https://localhost:44341/api/Movie/PostMovieTbl", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    movie = JsonConvert.DeserializeObject<MovieTbl>(apiresponse);
                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            TempData["Id"] = id;
            MovieTbl? m = new MovieTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Movie/"+id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    m = JsonConvert.DeserializeObject<MovieTbl>(apiResponse);
                }
            }
            return View(m);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(MovieTbl m)
        {
            int Id = Convert.ToInt32(TempData["Id"]);
            //Employee emp = new Employee();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                await client.DeleteAsync("https://localhost:44341/api/Movie/" + Id);

            }
            return RedirectToAction("Index");
        }
    }
}
