using DemoClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
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
        private readonly DemoContext _context;

        public BookingController(DemoContext context)
        {
            _context = context;
        }
        public int SeatCheck(string a, DateTime d, int id)//Seat availability
        {
            List<OrderDetailTbl> detail = _context.OrderDetails.ToList();
            string[] SeatList = a.Split(",", StringSplitOptions.RemoveEmptyEntries);
            for (int b = 0; b < SeatList.Length; b++)
            {
                foreach (var od in detail)//check in orderdetails
                {
                    string[] Seatnos = od.SeatNo.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < Seatnos.Length; j++)
                    {
                        if ((od.MovieDate == d) && (od.MovieId == id) && (Seatnos[j] == SeatList[b]))
                        {
                            return 0;
                        }
                    }
                }
            }
            return 1;
        }
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
                            int i = SeatCheck(book.SeatNo, book.Date, book.MovieId);
                            if (i == 1)
                            {
                                using (var client = new HttpClient())
                                {
                                    StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                                    using (var response = await client.PostAsync("https://localhost:44341/api/Booking", content))
                                    {
                                        string apiresponse = await response.Content.ReadAsStringAsync();
                                        movie = JsonConvert.DeserializeObject<MovieTbl>(apiresponse);
                                    }
                                }
                            }
                            else
                            {

                                ViewBag.ErrorMessage = "Already Booked.";
                                return View();
                            }
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
        [HttpPost]
        public async Task<IActionResult> ProceedToBuy()
        {
            var userid= HttpContext.Session.GetInt32("UserId");
            BookingTbl book = new BookingTbl();
            book.UserId = userid;
            List<BookingTbl> Cart = new List<BookingTbl>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("https://localhost:44341/api/Booking/ListConvert", content);
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    Cart = JsonConvert.DeserializeObject<List<BookingTbl>>(response);
                }
            }
            foreach (var i in Cart)
            {
                int a = SeatCheck(i.SeatNo, i.Date, i.MovieId);
                if (a == 0)
                {
                    var msg = "Seat is already booked.\n Try selecting other seats";
                    HttpContext.Session.SetString("msg", msg);
                    return RedirectToAction("Index");
                }
            }
            OrderMasterTbl om = new OrderMasterTbl();

            om.OrderDate = DateTime.Today;
            om.UserId = userid;
            om.Amount = 0;
            foreach (var item in Cart)
            {

                om.Amount += item.AmountTotal;
            }
            OrderMasterTbl om1 = new OrderMasterTbl();
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(om), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync("https://localhost:44341/api/Booking/AddToOrderMaster", content))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    om1 = JsonConvert.DeserializeObject<OrderMasterTbl>(apiresponse);
                }
            }
            HttpContext.Session.SetInt32("Total", (int)om1.Amount);
            return RedirectToAction("Payment", new { id = om1.OrderMasterId });
        }
        public async Task<IActionResult> Payment(int id)
        {
            OrderMasterTbl om = new OrderMasterTbl();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking/OrderMasterGetById?id=" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    om = JsonConvert.DeserializeObject<OrderMasterTbl>(apiResponse);
                }
            }
            return View(om);
        }
        [HttpPost]
        public async Task<IActionResult> Payment(OrderMasterTbl om)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            BookingTbl book = new BookingTbl();
            book.UserId = UserId;
            List<BookingTbl> Cart = new List<BookingTbl>();
            List<BookingTbl> booking = new List<BookingTbl>();
            List<OrderDetailTbl> od = new List<OrderDetailTbl>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("https://localhost:44341/api/Booking/ListConvert", content);
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    Cart = JsonConvert.DeserializeObject<List<BookingTbl>>(response);
                }
            }
            if(om.Paid==om.Amount)
            {
                booking = Cart;
                OrderMasterTbl om1 = new OrderMasterTbl();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(om), Encoding.UTF8, "application/json");
                    using (var response = await client.PutAsync("https://localhost:44341/api/Booking", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        om1 = JsonConvert.DeserializeObject<OrderMasterTbl>(apiResponse);
                    }
                }
                foreach (var j in booking)
                {
 
                    MovieTbl m = new MovieTbl();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        using (var response = await client.GetAsync("https://localhost:44341/api/Movie/" + j.MovieId))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            m = JsonConvert.DeserializeObject<MovieTbl>(apiResponse);
                        }

                    }
                    m.capacity -= j.NoOfTickets;
                    MovieTbl m1 = new MovieTbl();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        StringContent content = new StringContent(JsonConvert.SerializeObject(om), Encoding.UTF8, "application/json");
                        using (var response = await client.PutAsync("https://localhost:44341/api/Movie", content))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            om1 = JsonConvert.DeserializeObject<OrderMasterTbl>(apiResponse);
                        }
                    }
                }
                foreach (var item in Cart)
                {
                    OrderDetailTbl detail = new OrderDetailTbl();
                    detail.MovieId = item.MovieId;
                    detail.NoOfTickets = item.NoOfTickets;
                    detail.MovieName = item.MovieName;
                    detail.UserId = (int)HttpContext.Session.GetInt32("UserId");
                    string dt = HttpContext.Session.GetString("Date");
                    detail.MovieDate = Convert.ToDateTime(dt);
                    detail.Slot = HttpContext.Session.GetString("slot");
                    detail.SeatNo = item.SeatNo;
                    detail.Cost = item.AmountTotal;
                    detail.OrderMasterId = om1.OrderMasterId;
                    od.Add(detail);
                }
                List<OrderDetailTbl> od1 = new List<OrderDetailTbl>();
                using (var client = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(od), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync("https://localhost:44341/api/Booking/AddRangeOrderDetails", content))
                    {
                        string apiresponse = await response.Content.ReadAsStringAsync();
                        od1 = JsonConvert.DeserializeObject<List<OrderDetailTbl>>(apiresponse);
                    }
                }
                List<BookingTbl> bt = new List<BookingTbl>();
                using (var client = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync("https://localhost:44341/api/Booking/RemoveRangeCart", content))
                    {
                        string apiresponse = await response.Content.ReadAsStringAsync();
                        bt = JsonConvert.DeserializeObject<List<BookingTbl>>(apiresponse);
                    }
                }
                return RedirectToAction("Thankyou");
            }
            else
            {
                ViewBag.ErrorMessage = "amount not valid";
                return View(om);
            }
        }

        public IActionResult Thankyou()
        {
            return View();
        }
    }
}
