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
        //string BaseUrl="link";
        //use this by using client.BaseAddress=new Uri(BaseUrl);
        string Baseurl = "https://localhost:44341/api/";
        public async Task<IActionResult> Index()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            List<BookingTbl> bookings = new List<BookingTbl>();
            List<BookingTbl> book=new List<BookingTbl>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("Booking");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<List<BookingTbl>>(response);
                    bookings = (from i in book
                            where i.UserId == id
                            select i).ToList();
                    if (bookings.Count == 0)
                    {
                        ViewBag.ErrorMessage = "Your Cart Is Empty..";
                        return View(bookings);
                    }
                }
                return View(bookings);
            }
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
                book.MovieName = HttpContext.Session.GetString("Moviename");
                int capacity = (int)HttpContext.Session.GetInt32("Capacity");
                int cost = (int)HttpContext.Session.GetInt32("Cost");
                book.AmountTotal = book.NoOfTickets * cost;
                book.Date = Convert.ToDateTime(HttpContext.Session.GetString("Date"));
                book.Slot = HttpContext.Session.GetString("Slot");
                HttpContext.Session.SetString("slot", book.Slot);
                string Seat = book.SeatNo;
                string[] seats = Seat.Split(",", StringSplitOptions.RemoveEmptyEntries);
                string SeatNo = string.Join(",", seats);
                book.SeatNo = SeatNo;
                BookingTbl book1 = new BookingTbl();
                if (book.NoOfTickets < capacity)
                {
                    if (SeatVal(SeatNo, book.NoOfTickets))
                    {
                        if (SeatnoVal(book.SeatNo))
                        {
                            using (var client = new HttpClient())
                            {
                                StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                                using (var response = await client.PostAsync("https://localhost:44341/api/Booking/Create", content))
                                {
                                    string apiresponse = await response.Content.ReadAsStringAsync();
                                    book1 = JsonConvert.DeserializeObject<BookingTbl>(apiresponse);
                                }
                            }
                            if (book.SeatNo == "Null")
                            {

                                ViewBag.ErrorMessage = "Already Booked.";
                                return View();
                            }
                            return Redirect("Index");
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
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking/" + id))
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
            //List<BookingTbl> b = new List<BookingTbl>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("https://localhost:44341/api/Booking/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    b = JsonConvert.DeserializeObject<BookingTbl>(apiResponse);
                }
            }
            return View(b);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(BookingTbl m)
        {
            int Id = Convert.ToInt32(TempData["Id"]);
            //Employee emp = new Employee();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                await client.DeleteAsync("https://localhost:44341/api/Booking/" + Id);

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> ProceedToBuy()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            OrderMasterTbl om = new OrderMasterTbl();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync("Booking/ProceedtoBuy?id=" + id, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    om = JsonConvert.DeserializeObject<OrderMasterTbl>(apiResponse);
                    HttpContext.Session.SetInt32("Masterid", om.OrderMasterId);
                }

            }
            if(om.Amount==1)
            {
                var msg = "Seat is already booked.\n Try selecting other seats";
                HttpContext.Session.SetString("msg", msg);
                return RedirectToAction("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("Total", (int)om.Amount);
                return RedirectToAction("Payment", new { id = om.OrderMasterId });
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> Payment(int? id)
        {
            OrderMasterTbl om = new OrderMasterTbl();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("Booking/GetPaymentById?id=" + id))
                {
                    var apiresponse = response.Content.ReadAsStringAsync().Result;
                    om = JsonConvert.DeserializeObject<OrderMasterTbl>(apiresponse);
                }
            }
            return View(om);
        }
        [HttpPost]
        public async Task<IActionResult> Payment(OrderMasterTbl om)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            om.UserId = (int)UserId;
            OrderMasterTbl om1 = new OrderMasterTbl();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(om), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync("Booking/Payment", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    om1 = JsonConvert.DeserializeObject<OrderMasterTbl>(apiResponse);
                }
                //return RedirectToAction("GetAllProduct", "Product");
            }
            if(om1.Paid!=0)
            {
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
