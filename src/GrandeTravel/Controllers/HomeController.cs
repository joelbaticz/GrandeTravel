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

        private IPackageRepository _packageRepo;
        private IFeedbackRepository _feedbackRepo;

        public HomeController(IPackageRepository packageRepo, IFeedbackRepository feedbackRepo)
        {
            _packageRepo = packageRepo;
            _feedbackRepo = feedbackRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Package> allPackages = _packageRepo.GetAll();

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
        public IActionResult Contact()
        {
            return View();
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
