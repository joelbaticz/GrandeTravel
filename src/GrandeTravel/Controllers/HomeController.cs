using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GrandeTravel.ViewModels;

using GrandeTravel.Models;
using GrandeTravel.ViewModels;
using GrandeTravel.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GrandeTravel.Controllers
{
    public class HomeController : Controller
    {

        private UserManager<ApplicationUser> _userManagerService;
        private RoleManager<IdentityRole> _roleManagerService;

        private IProviderRepository _providerRepo;
        private ICustomerRepository _customerRepo;
        private IPackageRepository _packageRepo;
        private IFeedbackRepository _feedbackRepo;

        public HomeController(UserManager<ApplicationUser> userManagerService, RoleManager<IdentityRole> roleManagerService, IProviderRepository providerRepo, ICustomerRepository customerRepo, IPackageRepository packageRepo, IFeedbackRepository feedbackRepo)
        {
            _userManagerService = userManagerService;
            _roleManagerService = roleManagerService;
            _providerRepo = providerRepo;
            _customerRepo = customerRepo;
            _packageRepo = packageRepo;
            _feedbackRepo = feedbackRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Package> allPackages = _packageRepo.Query(p => p.IsActive == true).Take(5);

            List<PackageWithRating> packagesWithRating = new List<PackageWithRating>();



            foreach (var item in allPackages)
            {
                PackageWithRating packageWithRating = new PackageWithRating
                {
                    Package = item,
                    Rating = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Select(f => f.Rating).DefaultIfEmpty(0).Average()
                };
                packagesWithRating.Add(packageWithRating);
            }


            HomeIndexViewModel vm = new HomeIndexViewModel
            {
                Destination = "",
                BestPackagesWithRatings = packagesWithRating.OrderByDescending(p => p.Rating).Take(5)
            };

            return View(vm);

            //return View();
        }


        [HttpPost]
        public IActionResult Index(HomeIndexViewModel vm)
        {
            string destination = vm.Destination;// HttpContext.Request.Form["destination"];
            return RedirectToAction("Index", "Packages", new { request = "", location = destination });

        }


        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            string name = "";
            string email = "";

            if (HttpContext.User.Identity.IsAuthenticated)
            {

                string userName = HttpContext.User.Identity.Name;
                ApplicationUser specificUser = await _userManagerService.FindByNameAsync(userName);


                if (await _userManagerService.IsInRoleAsync(specificUser, "Provider"))
                {
                    name = _providerRepo.GetSingle(p => p.UserId == specificUser.Id).DisplayName;
                    email = specificUser.Email;
                }
                else if (await _userManagerService.IsInRoleAsync(specificUser, "Customer"))
                {
                    name = _customerRepo.GetSingle(c => c.UserId == specificUser.Id).FirstName;
                    name += " ";
                    name += _customerRepo.GetSingle(c => c.UserId == specificUser.Id).LastName;
                    email = specificUser.Email;
                }
            }

            ContactViewModel vm = new ContactViewModel
            {
                Name = name,
                Email = email,
                Subject = "Subject",
                Message = "Dear Grande Travel,\n\n...\n\nKind Regards,\n" + name
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ContactPost", "Home");
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult ContactPost()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SinglePage()
        {
            return View();
        }
    }
}
