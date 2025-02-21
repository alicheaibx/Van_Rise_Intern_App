using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Van_Rise_Intern_App.Models;

namespace Van_Rise_Intern_App.Controllers
{
    public class HomeController : Controller
    {
        private ActionResult CheckSessionToken()
        {
            if (Session["Token"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return null;
        }

        public ActionResult Index()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult Devices()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult Privacy()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult Phone()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult Client()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult PhoneNumberReservation()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult Report()
        {
            var result = CheckSessionToken();
            if (result != null) return result;
            return View();
        }

        public ActionResult Login()
        {
            if (Session["Token"] != null)
            {
                return RedirectToAction("Devices", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53211/");
                var loginRequest = new LoginRequest { Username = username, Password = password };
                var response = await client.PostAsJsonAsync("api/user/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadAsAsync<LoginResponse>();
                    if (loginResponse.IsValid)
                    {
                        // Store token in session
                        Session["Token"] = loginResponse.Token;
                        Session["Username"] = username;

                        return RedirectToAction("Devices", "Home");
                    }
                }

                ViewBag.ErrorMessage = "Invalid username or password";
                return View();
            }
        }

        public ActionResult Logout()
        {
            // Remove session token
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Home");
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Error()
        {
            var errorModel = new ErrorViewModel
            {
                RequestId = Guid.NewGuid().ToString(),
                ErrorMessage = TempData["ErrorMessage"] as string
            };
            return View(errorModel);
        }
    }
}