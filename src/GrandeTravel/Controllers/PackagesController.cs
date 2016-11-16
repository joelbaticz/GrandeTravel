using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GrandeTravel.Models;
using GrandeTravel.ViewModels;
using GrandeTravel.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace GrandeTravel.Controllers
{
    public class PackagesController : Controller
    {
        private IHostingEnvironment _hostingEnv;

        private IPackageRepository _packageRepo;
        private ICustomerRepository _customerRepo;
        private IProviderRepository _providerRepo;
        private IFeedbackRepository _feedbackRepo;
        private IBookingRepository _bookingRepo;
        private IEmailService _emailService;
        private ISmsService _smsService;

        private UserManager<ApplicationUser> _userManagerService;
        private RoleManager<IdentityRole> _roleManagerService;

        /*private DbContextService _dbContext;

        public PackagesController(DbContextService dbContext)
        {
            _dbContext = dbContext;
        }
        */

        // SignInManager<ApplicationUser> signInManagerService, RoleManager<IdentityRole> roleManagerService, IProviderRepository providerRepo, ICustomerRepository customerRepo
        public PackagesController(IHostingEnvironment hostingEnv, UserManager<ApplicationUser> userManagerService, RoleManager<IdentityRole> roleManagerService, IEmailService emailService, ISmsService smsService, ICustomerRepository customerRepo, IProviderRepository providerRepo, IPackageRepository packageRepo, IFeedbackRepository feedbackRepo, IBookingRepository bookingRepo)
        {
            _hostingEnv = hostingEnv;

            _packageRepo = packageRepo;
            _customerRepo = customerRepo;
            _providerRepo = providerRepo;
            _feedbackRepo = feedbackRepo;
            _bookingRepo = bookingRepo;
            _emailService = emailService;
            _smsService = smsService;
            _userManagerService = userManagerService;
            _roleManagerService = roleManagerService;


        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string request, string location)
        {

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
            }

            IEnumerable<Package> packages = new List<Package>();

            if (request == null || request == "" || request == "GetActive")
            {
                packages = _packageRepo.Query(p => p.IsActive == true);
            }
            else if (request == "GetDiscontinued")
            {
                packages = _packageRepo.Query(p => p.IsActive == false);
            }
            else if (request == "GetAll")
            {
                packages = _packageRepo.GetAll();
            }

            if (location != null && location != "")
            {
                packages = packages.Where(p => p.Location.ToLower().Contains(location.ToLower()));
            }


            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string userName = HttpContext.User.Identity.Name;
                ApplicationUser user = await _userManagerService.FindByNameAsync(userName);

                bool result = await _userManagerService.IsInRoleAsync(user, "Provider");
                if (result)
                {
                    Provider specficProvider = _providerRepo.GetSingle(p => p.UserId == user.Id);

                    packages = packages.Where(p => p.ProviderId == specficProvider.ProviderId);
                }
            }


            // -------- Packages gathered -> collect Ratings and Feedbacks ----------

            List<double> packageRatings = new List<double>();
            List<int> packageNumberOfReviews = new List<int>();

            foreach (var item in packages)
            {
                /*IEnumerable<Feedback> feedbacks = _feedbackRepo.Query(f => f.PackageId == item.PackageId);*/

                double actualRating = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Select(f => f.Rating).DefaultIfEmpty(0).Average();
                packageRatings.Add(actualRating);

                int actualNumberOfReviews = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Select(f => f.FeedbackId).Count();
                packageNumberOfReviews.Add(actualNumberOfReviews);
            }

            DisplayAllPackagesViewModel vm = new DisplayAllPackagesViewModel
            {
                Location = location,
                MinPrice = null,
                MaxPrice = null,

                Packages = packages,
                Ratings = packageRatings,
                NumberOfReviews = packageNumberOfReviews
            };


            return View(vm);

        }

        [Authorize]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index(DisplayAllPackagesViewModel vm)
        {
            if (ModelState.IsValid)
            {

                // ----------- You can only filter Active Packages - only customer can ------- //

                IEnumerable<Package> packages = new List<Package>();
                packages = _packageRepo.Query(p => p.IsActive == true);

                //vm.Packages = _packageRepo.GetAll();

                if (vm.Location != null)
                {
                    packages = packages.Where(p => p.Location.ToLower().Contains(vm.Location.ToLower()));
                }
                if (vm.MinPrice != null)
                {
                    packages = packages.Where(p => p.Price >= Double.Parse(vm.MinPrice));
                }
                if (vm.MaxPrice != null)
                {
                    packages = packages.Where(p => p.Price <= Double.Parse(vm.MaxPrice));
                }

                // -------- Packages gathered -> collect Ratings and Feedbacks ----------

                List<double> packageRatings = new List<double>();
                List<int> packageNumberOfReviews = new List<int>();

                foreach (var item in packages)
                {
                    //IEnumerable<Feedback> feedbacks = _feedbackRepo.Query(f => f.PackageId == item.PackageId);

                    double actualRating = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Select(f => f.Rating).DefaultIfEmpty(0).Average();
                    packageRatings.Add(actualRating);

                    int actualNumberOfReviews = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Select(f => f.FeedbackId).Count();
                    packageNumberOfReviews.Add(actualNumberOfReviews);
                }

                vm.Packages = packages;
                vm.Ratings = packageRatings;
                vm.NumberOfReviews = packageNumberOfReviews;
            };

            return View(vm);
        }

        [Authorize(Roles = "Provider")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
            Provider currentProvider = _providerRepo.GetSingle(p => p.UserId == currentUser.Id);
            


            //Look up images on the server
            IEnumerable<string> imagePaths = new List<string>();
            List<string> imageNames = new List<string>();
            List<string> imageCorrectPaths = new List<string>();

            string folderPath = _hostingEnv.WebRootPath + $@"\images\" + currentProvider.UserId;

            if (Directory.Exists(folderPath))
            {
                imagePaths = Directory.GetFiles(folderPath);
                /*
                for(int i=0; i< imagePaths.Count(); i++)
                {
                    int endPos = imagePaths.ElementAt(i).LastIndexOf("\\")+1;
                    string newString = imagePaths.ElementAt(i).Substring(endPos);
                    imageNames.Add(newString);
                }
                */
                foreach (var item in imagePaths)
                {
                    int endPos = item.LastIndexOf("\\");
                    string newString = item.Substring(endPos);
                    imageCorrectPaths.Add($@"\images\" + currentProvider.UserId + newString);
                }

            }
                
            CreatePackageViewModel vm = new CreatePackageViewModel
            {
                ProviderId = currentProvider.ProviderId,
                IsActive = true,
                ImageNames = imageCorrectPaths //imageNames

            };

            return View(vm);
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePackageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
                Provider currentProvider = _providerRepo.GetSingle(p => p.UserId == currentUser.Id);

                string thumbnailUrl = "";

                if (vm.ThumbnailUrl != null && vm.ThumbnailUrl!="")
                {
                    thumbnailUrl = $@"\images\" + currentProvider.UserId + "\\" + vm.ThumbnailUrl;
                }

                Package newPackage = new Package
                {
                    ProviderId = vm.ProviderId,
                    Name = vm.Name,
                    ThumbnailUrl = thumbnailUrl,
                    Location = vm.Location,
                    Description = vm.Description,
                    Price = vm.Price,
                    IsActive = vm.IsActive
                };

                _packageRepo.Create(newPackage);

                return RedirectToAction("Index");
            }
            return View(vm);
        }


        [Authorize(Roles = "Provider")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Package editablePackage = _packageRepo.GetSingle(p => p.PackageId == id);

            EditPackageViewModel vm = new EditPackageViewModel
            {
                PackageId = id,
                ProviderId = editablePackage.ProviderId,
                Name = editablePackage.Name,
                ThumbnailUrl = editablePackage.ThumbnailUrl,
                Location = editablePackage.Location,
                Description = editablePackage.Description,
                Price = editablePackage.Price,
                IsActive = editablePackage.IsActive
            };

            return View(vm);
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        public IActionResult Edit(EditPackageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Package editedPackage = new Package
                {
                    PackageId = vm.PackageId,
                    ProviderId = vm.ProviderId,
                    Name = vm.Name,
                    ThumbnailUrl = vm.ThumbnailUrl,
                    Location = vm.Location,
                    Description = vm.Description,
                    Price = vm.Price,
                    IsActive = vm.IsActive
                };

                _packageRepo.Update(editedPackage);

                return RedirectToAction("Index");
            }
            return View(vm);
        }

        [Authorize(Roles = "Provider")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            DeletePackageViewModel vm = new DeletePackageViewModel
            {
                PackageId = id,
                Name = _packageRepo.GetSingle(p => p.PackageId == id).Name
            };

            return View(vm);
        }

        [Authorize(Roles = "Provider")]
        [HttpGet]
        public IActionResult DeleteCommit(int id)
        {
            Package toBeDeletedPackage = _packageRepo.GetSingle(p => p.PackageId == id);

            if (toBeDeletedPackage != null)
            {
                _packageRepo.Delete(toBeDeletedPackage);
            }

            return RedirectToAction("Index", "Packages");

        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Details(int id)
        {
            Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == id);

            IEnumerable<Feedback> packageFeedbacks = _feedbackRepo.Query(f => f.PackageId == currentPackage.PackageId);

            double packageRating = 0;
            int packageFeedbackNumber = 0;

            if (packageFeedbacks.Count() > 0)
            {
                packageRating = packageFeedbacks.Average(f => f.Rating);
                packageFeedbackNumber = packageFeedbacks.Count();
            }

            PackageDetailsViewModel vm = new PackageDetailsViewModel
            {
                ProviderId = currentPackage.ProviderId,
                PackageId = currentPackage.PackageId,
                Name = currentPackage.Name,
                CompanyName = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId).DisplayName,
                Rating = packageRating,
                NumberOfReviews = packageFeedbackNumber,
                Location = currentPackage.Location,
                Description = currentPackage.Description,
                ThumbnailUrl = currentPackage.ThumbnailUrl,
                Price = currentPackage.Price,
                Feedbacks = packageFeedbacks
            };

            return View(vm);
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Feedback(int id)
        {
            Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == id);
            Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);

            LeaveFeedbackViewModel vm = new LeaveFeedbackViewModel
            {
                PackageId = id,
                PackageName = currentPackage.Name,
                CompanyName = currentProvider.DisplayName

            };

            return View(vm);
        }

        [Authorize]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Feedback(LeaveFeedbackViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == vm.PackageId);
                Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);

                string userFullName = "";
                if (!User.Identity.IsAuthenticated)
                {
                    userFullName = "Anonymous";
                }
                else
                {
                    if (User.IsInRole("Customer"))
                    {
                        ApplicationUser user = await _userManagerService.FindByNameAsync(User.Identity.Name);
                        Customer customer = _customerRepo.GetSingle(c => c.UserId == user.Id);

                        userFullName = customer.FirstName + " " + customer.LastName;
                    }
                    else if (User.IsInRole("Provider"))
                    {
                        userFullName = currentProvider.DisplayName;
                    }
                }

                Feedback newFeedback = new Feedback
                {
                    PackageId = vm.PackageId,
                    Rating = vm.Rating,
                    Content = vm.Content,
                    FullName = userFullName,
                    Date = DateTime.Now
                };

                _feedbackRepo.Create(newFeedback);

                return RedirectToAction("FeedbackPost");
            }
            return View(vm);

        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult FeedbackPost()
        {
            return View();
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult Book(int id)
        {
            Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == id);
            Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);

            BookPackageViewModel vm = new BookPackageViewModel
            {
                PackageId = currentPackage.PackageId,
                ProviderId = currentProvider.ProviderId,
                CompanyName = currentProvider.DisplayName,
                PackageName = currentPackage.Name,
                NumberOfPeople = 1,
                DateFor = DateTime.Now,
                Price = currentPackage.Price
            };
            return View(vm);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Book(BookPackageViewModel vm)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Content("Login required!");
            }
            else if (User.IsInRole("Customer"))
            {
                if (ModelState.IsValid)
                {
                    Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == vm.PackageId);
                    Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);
                    ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
                    Customer currentCustomer = _customerRepo.GetSingle(c => c.UserId == currentUser.Id);

                    Booking actualBooking = new Booking
                    {
                        CustomerId = currentCustomer.CustomerId,
                        PackageId = currentPackage.PackageId,
                        DateMade = DateTime.Now,
                        CompanyName = currentProvider.DisplayName,
                        PackageName = currentPackage.Name,
                        DateFor = vm.DateFor,
                        NumberOfPeople = vm.NumberOfPeople,
                        SpecialRequirements = vm.SpecialRequirements,
                        Price = currentPackage.Price,
                        IsPaid = false
                    };

                    _bookingRepo.Create(actualBooking);

                    string emailContent = "Dear " + currentCustomer.FirstName + ",\n\nThank you for your order with Grande Travel.\n" +
                                         "Please don't hesitate to check your booking details here:\n" +
                                         "http://localhost:5000/Packages/BookedPackages [Login required]\n\n" +
                                         "Kind Regards,\nGrande Travel";

                    await _emailService.SendEmailAsync(currentUser.Email, "Grande Travel - Do not reply", emailContent);

                    return RedirectToAction("BookedPackages", "Packages");
                }
                return View(vm);
            }
            return Content("Must be a Customer to book a package! Nice try though! ;)");
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> BookedPackages()
        {
            ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
            Customer currentCustomer = _customerRepo.GetSingle(c => c.UserId == currentUser.Id);

            DisplayAllBookingsViewModel vm = new DisplayAllBookingsViewModel
            {
                BookedPackages = _bookingRepo.Query(b => b.CustomerId == currentCustomer.CustomerId).ToList()
            };
            return View(vm);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult PayPackage(int id, int numberOfPeople)
        {
            Booking currentBooking = _bookingRepo.GetSingle(b => b.BookingId == id);
            Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == currentBooking.PackageId);
            Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);
            //ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
            //Customer currentCustomer = _customerRepo.GetSingle(c => c.UserId == currentUser.Id);

            currentBooking.NumberOfPeople = numberOfPeople;

            PayPackageViewModel vm = new PayPackageViewModel
            {
                BookingId = id,
                CompanyName = currentProvider.DisplayName,
                PackageName = currentPackage.Name,
                DateFor = currentBooking.DateFor,
                NumberOfPeople = currentBooking.NumberOfPeople,
                TotalPrice = currentBooking.NumberOfPeople * currentPackage.Price,
                PaymentType = 1
            };

            _bookingRepo.Update(currentBooking);            

            return View(vm);
        }
        
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> PayPackage(PayPackageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
                Customer currentCustomer = _customerRepo.GetSingle(c => c.UserId == currentUser.Id);

                Booking currentBooking = _bookingRepo.GetSingle(b => b.BookingId == vm.BookingId);
                Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == currentBooking.PackageId);
                Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);

                currentBooking.IsPaid = true;

                _bookingRepo.Update(currentBooking);

                //--------======= Email Notification ========-----------

                //Create Unique Voucher Code
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                int voucherCodeLength = 8;

                Random random = new Random();
                string uniqueVoucherCode = new string(Enumerable.Repeat(chars, voucherCodeLength).Select(s => s[random.Next(s.Length)]).ToArray());

                DateTime voucherExpDate = DateTime.Today.AddMonths(3);

             

                string emailContent = "Dear " + currentCustomer.FirstName + ",\n\n"+
                                      "Thank you for your order.  \n\n" +
                                      "Please find the tour details below:\n\n" +

                                      "Company Name: " + currentProvider.DisplayName + "\n" +
                                      "Package Name: " + currentPackage.Name + "\n" +
                                      "Tour Date: " + currentBooking.DateFor.ToString("yyyy-MM-dd") + "\n" +
                                      "Booked On: " + currentBooking.DateMade.ToString("yyyy-MM-dd hh:mm:ss") + "\n" +
                                      "Number of People: " + currentBooking.NumberOfPeople + "\n" +
                                      "Total Cost: $" + (currentBooking.Price * currentBooking.NumberOfPeople) + "\n\n" +

                                      "Please also find the voucher included:\n\n" +
                                      "Voucher Code: " + uniqueVoucherCode + "\n" +
                                      "Voucher Validity: 3 months from purchase\n" +
                                      "Voucher Expiry: " + voucherExpDate.ToString("yyyy-MM-dd") + "\n\n" +

                                      "Kind Regards,\nGrande Travel";

                await _emailService.SendEmailAsync(currentUser.Email, "Grande Travel - Order Confirmation - Do not reply", emailContent);

                //--------======= SMS Notification ========-----------

                string smsContent = "Dear " + currentCustomer.FirstName + ", thank you for your order! Grande Travel";

                await _smsService.SendSmsAsync(currentUser.PhoneNumber, smsContent);

                return RedirectToAction("BookedPackages", "Packages");

            }
            return View(vm);
        }

        /*
        [HttpPost]
        public IActionResult UploadFiles(IList<IFormFile> files)
        {

            long size = 0;
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                filename = _hostingEnv.WebRootPath + $@"\"+filename;
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            ViewBag.Message = files.Count + " file(s) / " + size + " bytes uploaded successfully!";
            return View();
        }
        */


        [HttpPost]
        public async Task<IActionResult> UploadFilesAjax()
        {
            ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
            Provider currentProvider = _providerRepo.GetSingle(p => p.UserId == currentUser.Id);

            string folderPath = _hostingEnv.WebRootPath + $@"\images\" + currentProvider.UserId;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            long size = 0;
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                filename = folderPath + $@"\{filename}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            string message = $"{files.Count} file(s) / { size} bytes uploaded successfully!";
    return Json(message);
        }


    }
}