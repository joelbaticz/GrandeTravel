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

namespace GrandeTravel.Controllers
{
    public class PackagesController : Controller
    {

        private IPackageRepository _packageRepo;
        private ICustomerRepository _customerRepo;
        private IProviderRepository _providerRepo;
        private IFeedbackRepository _feedbackRepo;

        private UserManager<ApplicationUser> _userManagerService;
        private RoleManager<IdentityRole> _roleManagerService;

        /*private DbContextService _dbContext;

        public PackagesController(DbContextService dbContext)
        {
            _dbContext = dbContext;
        }
        */

        // SignInManager<ApplicationUser> signInManagerService, RoleManager<IdentityRole> roleManagerService, IProviderRepository providerRepo, ICustomerRepository customerRepo
        public PackagesController(UserManager<ApplicationUser> userManagerService, RoleManager<IdentityRole> roleManagerService, ICustomerRepository customerRepo, IProviderRepository providerRepo, IPackageRepository packageRepo, IFeedbackRepository feedbackRepo)
        {
            _packageRepo = packageRepo;
            _customerRepo = customerRepo;
            _providerRepo = providerRepo;
            _feedbackRepo = feedbackRepo;
            _userManagerService = userManagerService;
            _roleManagerService = roleManagerService;


        }


        [HttpGet]
        public async Task<IActionResult> Index(string  request, string location)
        {

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
            }

            IEnumerable<Package> packages=new List<Package>();

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
            
            if (location!=null && location!="")
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

 
        [HttpPost]
        public IActionResult Index(DisplayAllPackagesViewModel vm)
        {
            if (ModelState.IsValid)
            {

                // ----------- You can only filter Active Packages - only customer can ------- //

                string location = vm.Location.ToLower();
                string minPrice = vm.MinPrice;
                string maxPrice = vm.MaxPrice;

                IEnumerable<Package> packages = new List<Package>();
                packages = _packageRepo.Query(p => p.IsActive == true);

                //vm.Packages = _packageRepo.GetAll();

                if (location != null)
                {
                    packages = packages.Where(p => p.Location.ToLower().Contains(location));
                }
                if (minPrice != null)
                {
                    packages = packages.Where(p => p.Price >= Double.Parse(minPrice));
                }
                if (maxPrice != null)
                {
                    packages = packages.Where(p => p.Price <= Double.Parse(maxPrice));
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
   
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ApplicationUser currentUser = await _userManagerService.FindByNameAsync(User.Identity.Name);
            int providerId = _providerRepo.GetSingle(p => p.UserId == currentUser.Id).ProviderId;


            CreatePackageViewModel vm = new CreatePackageViewModel
            {
                ProviderId = providerId,
                IsActive = true
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(CreatePackageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Package newPackage = new Package
                {
                    ProviderId = vm.ProviderId,
                    Name = vm.Name,
                    ThumbnailUrl = vm.ThumbnailUrl,
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

        [HttpPost]
        public async Task<IActionResult> Feedback(LeaveFeedbackViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Package currentPackage = _packageRepo.GetSingle(p => p.PackageId == vm.PackageId);
                Provider currentProvider = _providerRepo.GetSingle(p => p.ProviderId == currentPackage.ProviderId);

                string userFullName="";
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

        [HttpGet]
        public IActionResult FeedbackPost()
        {
            return View();
        }
    }
}
