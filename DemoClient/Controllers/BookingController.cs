using DemoClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DemoClient.Controllers
{
    public class BookingController : Controller
    {
        public async Task<IActionResult> Index()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            //var sampleContext = _context.BookingTbl.Include(b => b.Movie).Include(b => b.User);
            List<BookingTbl> book = new List<BookingTbl>();
            List<BookingTbl> cart = new List<BookingTbl>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:44341/api/Booking");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<List<BookingTbl>>(response);
                    cart = (from i in book
                            where i.UserId == id
                            select i).ToList();
                    if (cart.Count == 0)
                    {
                        ViewBag.ErrorMessage = "Your Cart Is Empty..";
                        return View(cart);
                    }
                }
                return View(cart);
            }
            //List<BookingTbl> cart = (from i in _context.BookingTbl.Include(b => b.Movie).Include(b => b.User) where i.UserId == id select i).ToList();

        }
        public bool SeatVal(string seatno, int n)//For No of tickets and Seat selection validation
        {
            bool a = true;
            string[] seatArr = seatno.Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (seatArr.Length == n)
            {
                return true;

            }
            else
                return false;

        }
        public bool SeatnoVal(string s) //Seat no validation 
        {

            bool a = true;
            string[] snoArr = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < snoArr.Length; i++)
            {
                if (Convert.ToInt32(snoArr[i]) > 0 && Convert.ToInt32(snoArr[i]) <= 50)
                {
                    a = true;
                }
                else
                {
                    return false;
                }


            }
            return a;

        }
        //public int SeatCheck(string a, DateTime d, int id)//Seat availability
        //{
        //    List<OrderDetails> detail = _context.OrderDetails.ToList();
        //    string[] SeatList = a.Split(",", StringSplitOptions.RemoveEmptyEntries);
        //    for (int b = 0; b < SeatList.Length; b++)
        //    {
        //        foreach (var od in detail)//check in orderdetails
        //        {
        //            string[] Seatnos = od.SeatNo.Split(",", StringSplitOptions.RemoveEmptyEntries);
        //            for (int j = 0; j < Seatnos.Length; j++)
        //            {
        //                if ((od.MovieDate == d) && (od.MovieId == id) && (Seatnos[j] == SeatList[b]))
        //                {
        //                    return 0;
        //                }
        //            }
        //        }
        //    }
        //    return 1;
        //}
        public async Task<IActionResult> Create(int id,BookingTbl book)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookingTbl book)
        {
            try
            {
                book.MovieId = (int)HttpContext.Session.GetInt32("MovieId");
                book.UserId = (int)HttpContext.Session.GetInt32("UserId");
                book.MovieName = HttpContext.Session.GetString("MovieName");
                int capacity = (int)HttpContext.Session.GetInt32("Capacity");
                int cost = (int)HttpContext.Session.GetInt32("Cost");
                book.AmountTotal = book.NoOfTickets * cost;
                book.Date = Convert.ToDateTime(HttpContext.Session.GetString("Date"));
                book.Slot = HttpContext.Session.GetString("Slot");
                HttpContext.Session.SetString("slot", book.Slot);
                string Seat = book.SeatNo;
                string[] seats = Seat.Split(",", StringSplitOptions.RemoveEmptyEntries);
                string SeatNo = string.Join(",", seats);
                MovieTbl movie = new MovieTbl();
                if (book.NoOfTickets < capacity)
                {
                    if (SeatVal(SeatNo, book.NoOfTickets))
                    {
                        if (SeatnoVal(book.SeatNo))
                        {
                            //int i = SeatCheck(book.SeatNo, book.Date, book.MovieId);
                            //if (i == 1)
                            //{
                            //    using (var client = new HttpClient())
                            //    {
                            //        StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                            //        using (var response = await client.PostAsync("https://localhost:44341/api/Booking", content))
                            //        {
                            //            string apiresponse = await response.Content.ReadAsStringAsync();
                            //            movie = JsonConvert.DeserializeObject<MovieTbl>(apiresponse);
                            //        }
                            //    }
                            //}
                            //else
                            //{

                            //    ViewBag.ErrorMessage = "Already Booked.";
                            //    return View();
                            //}
                        }
                        else
                        {
                            ViewBag.SeatErrorMessage = "Please Enter the Valid SeatNo";
                            return View();

                        }
                    }
                    else
                    {
                        ViewBag.ValidationMesssage = "Selected seats and No of seats mismatching....";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrMessage = "Your No of Tickets is greater than that of available tickets\nPlease enter lesser value";
                    return View();
                }
                return View(book);
            }
            catch (Exception e)
            {
                //throw new FormatException("Seat No must be number .please correct it.", e);
                //ViewBag.ErrorMessage = "Seat No must be number .please correct it.";
                ViewBag.Emessage = "Seat no must be number as per the seating arrangement.Please correct it.";
                return View();
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            BookingTbl b = new BookingTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking/Details" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    b = JsonConvert.DeserializeObject<BookingTbl>(apiResponse);
                }
            }
            return View(b);
        }
        public async Task<IActionResult> Delete(int id)
        {
            TempData["Id"] = id;
            BookingTbl? b = new BookingTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    b = JsonConvert.DeserializeObject<BookingTbl>(apiResponse);
                }
            }
            return View(b);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(MovieTbl m)
        {
            int Id = Convert.ToInt32(TempData["Id"]);
            //Employee emp = new Employee();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                await client.DeleteAsync("https://localhost:44341/api/Booking" + Id);

            }
            return RedirectToAction("Index");
        }
    }
}
