using Bometh.FirebaseDatabase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using TugwellApp.Models;

namespace TugwellApp.Controllers
{
    public class HomeController : Controller
    {
        #region GuestLanding

        public IActionResult GuestLanding()
        {
            ViewData["HideNavbar"] = true; // Hide navbar on the Guest Landing page

            var model = new SignUpModel
            {
                WelcomeMessage = "Welcome to the TugwellApp!",

            };

            return View(model);
        }
        #endregion

        #region Authentication & Authorisation
        private readonly ILogger<HomeController> _logger;

        FirebaseAuthProvider _firebaseAuth;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCPnAN23NhQeHdQnyFV6w2LMz1bmBqOQ8c"));
        }

        public IActionResult AddTWUser()
        {
            ViewData["HideNavbar"] = true; // This hides the navbar on the Guest Landing page

            // Create a new TWUser model with drop-down options
            var model = new TWUser
            {
                FloorOptions = GetFloorOptions(),
                RoleOptions = GetRoleOptions()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddTWUser(TWUser model)
        {
            if (ModelState.IsValid)
            {
                // Logic where this saves to the User model

                // Redirect to the FBLoginCred page upon successful registration
                return RedirectToAction("FBLoginCred");
            }

            // If model state is invalid, repopulate drop-down lists and return the view
            model.FloorOptions = GetFloorOptions();
            model.RoleOptions = GetRoleOptions();
            return View(model);
        }

        public IActionResult FBLoginCred()
        {
            ViewData["HideNavbar"] = true; // Hides the navbar on the Guest Landing page

            return View();
        }

        private IEnumerable<SelectListItem> GetFloorOptions()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "0" },
            new SelectListItem { Value = "1", Text = "1" },
            new SelectListItem { Value = "2", Text = "2" },
            new SelectListItem { Value = "3", Text = "3" },
            new SelectListItem { Value = "4", Text = "4" },
            new SelectListItem { Value = "5", Text = "5" },
            new SelectListItem { Value = "6", Text = "6" },
            new SelectListItem { Value = "7", Text = "7" },
            new SelectListItem { Value = "8", Text = "8" },
            new SelectListItem { Value = "9", Text = "9" },
            new SelectListItem { Value = "10", Text = "10" },
            new SelectListItem { Value = "11", Text = "11" },
        };
        }

        private IEnumerable<SelectListItem> GetRoleOptions()
        {
            return new List<SelectListItem>
            {
            //new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Student", Text = "Student" },

            };
        }

        public IActionResult SubmitTWUSer(TWUser vm)
        {
            return View();
        }

        //SignUp
        public async Task<IActionResult> RegisterUser(SignUpModel vm)
        {
            try
            {
                // Create the user in Firebase
                var authResult = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(vm.EmailAddress, vm.Password);

                // After creating the user, assign them the "Student" role in the database
                var firebaseDB = new FirebaseClient("https://tugwellhallwebapp-default-rtdb.firebaseio.com/", new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authResult.FirebaseToken)
                });

                await firebaseDB
                    .Child("roles/Student")
                    .Child(authResult.User.LocalId)
                    .PutAsync(true);

                // Set success message and redirect to the login page
                TempData["SuccessMessage"] = "You have successfully signed up! Please log in.";
                return RedirectToAction("GuestLanding"); // Redirect to login page
            }
            catch (FirebaseAuthException ex)
            {
                // Log the exception for debugging
                _logger.LogError(ex, "Firebase Authentication Exception");

                var firebaseEx = JsonConvert.DeserializeObject<ErrorModel>(ex.RequestData);
                string errorMessage = firebaseEx?.message ?? "An unknown error occurred.";

                // Check if the error is related to email address already in use
                if (errorMessage.Contains("EMAIL_EXISTS") || ex.Reason == AuthErrorReason.EmailExists)
                {
                    // Error message for existing email
                    TempData["ErrorMessage"] = "The email address is already in use. Try logging in.";
                    return RedirectToAction("GuestLanding"); // Redirect to GuestLanding page
                }
                else
                {
                    // Handle other Firebase exceptions
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToAction("GuestLanding"); // Redirect to GuestLanding page
                }
            }

        }

        #endregion

        #region Login

        private readonly string apiKey = "AIzaSyCPnAN23NhQeHdQnyFV6w2LMz1bmBqOQ8c"; //TugwellHallWebApp API key, Double-check!

        [HttpPost]
        public async Task<IActionResult> Login(SignUpModel model)
        {
            ViewData["HideNavbar"] = true; // Hide the navbar on this page

            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                var authResult = await auth.SignInWithEmailAndPasswordAsync(model.EmailAddress, model.Password);

                if (authResult != null)
                {
                    var user = authResult.User;

                    // Initialize firebaseDB with your database URL

                    var firebaseDB = new FirebaseClient("https://tugwellhallwebapp-default-rtdb.firebaseio.com/", new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(authResult.FirebaseToken)
                    });

                    // Query the Realtime Database for the user's role
                    var adminSnapshot = await firebaseDB
                        .Child("roles/Administrator")
                        .Child(user.LocalId)
                        .OnceSingleAsync<string>();

                    bool isAdmin = adminSnapshot != null && bool.TryParse(adminSnapshot, out var result) && result;

                    // If adminSnapshot is null or not a valid boolean, default to Student
                    bool isStudent = !isAdmin; //If user is not Admin, then they are a student.

                    // Store the user's role in session
                    HttpContext.Session.SetString("UserRole", isAdmin ? "Administrator" : "Student");

                    // Redirect based on role
                    if (isAdmin)
                    {
                        TempData["SuccessMessage"] = "Welcome, Admin!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Welcome, Student!";
                        return RedirectToAction("Index", "Home");
                    }
                }

                TempData["ErrorMessage"] = "Invalid login attempt.";
                return RedirectToAction("GuestLanding");
            }
            catch (FirebaseAuthException ex)
            {
                var errorResponse = JsonConvert.DeserializeObject<FirebaseErrorResponse>(ex.ResponseData);
                string errorMessage = errorResponse?.Error?.Message ?? "An unknown error occurred.";

                if (errorMessage.Contains("INVALID_LOGIN_CREDENTIALS"))
                {
                    TempData["ErrorMessage"] = "Incorrect email address or password. Please try again.";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred during login. Please try again later.";
                }

                return RedirectToAction("GuestLanding");

            }
        }

        #endregion

        #region Logout
        //public async Task<IActionResult> LogOut()
        //{
        //    // Sign out the user
        //    await HttpContext.SignOutAsync("Firebase");

        //    //// Clear the session
        //    //HttpContext.Session.Clear();

        //    // Redirect to the GuestLanding page
        //    return RedirectToAction("GuestLanding");
        //}

        #endregion

        #region Firebase Error Deserialisation

        //Handling Firebase error responses
        public class FirebaseErrorResponse
        {
            public FirebaseError Error { get; set; }
        }

        public class FirebaseError
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public List<FirebaseErrorDetail> Errors { get; set; }
        }

        public class FirebaseErrorDetail
        {
            public string Message { get; set; }
            public string Domain { get; set; }
            public string Reason { get; set; }
        }
        #endregion


        public IActionResult Index()
        {
            //if(User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Dashboard");
            //}
            //else
            //{
            //    return RedirectToAction("GuestLanding");
            //}
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
