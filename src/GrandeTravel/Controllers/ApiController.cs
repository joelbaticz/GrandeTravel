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
    public class ApiController : Controller
    {
        private IPackageRepository _packageRepo;
        private IFeedbackRepository _feedbackRepo;

        public ApiController(IPackageRepository packageRepo, IFeedbackRepository feedbackRepo)
        {
            _packageRepo = packageRepo;
            _feedbackRepo = feedbackRepo;
        }


        public JsonResult GetAllPackageLocation()
        {
            /*
            LocationData ld = new LocationData
            {
                Locations = _packageRepo.GetAll().Select(p => p.Location).Distinct()
            };

            
            return Json(ld);
            */
            /*return Json(_packageRepo.GetAll().Select(p => p.Location).Distinct());*/

            return Json(_packageRepo.Query(p => p.IsActive == true).Select(p => p.Location).Distinct());
        }

        public JsonResult GetAllPackages(string location)
        {
            IEnumerable<Package> allPackages = new List<Package>();
            List<PackageWithRating> packagesWithRating = new List<PackageWithRating>();
            
            if (String.IsNullOrEmpty(location))
            {
                allPackages = _packageRepo.Query(p => p.IsActive == true); 
            }
            else
            {
                allPackages = _packageRepo.Query(p => p.IsActive == true && p.Location.Contains(location));

            }

            foreach (var item in allPackages)
            {

                PackageWithRating packageWithRating = new PackageWithRating
                {
                    Package = item,
                    Rating = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Select(f => f.Rating).DefaultIfEmpty(0).Average(),
                    NumberOfFeedbacks = _feedbackRepo.Query(f => f.PackageId == item.PackageId).Count()
                };
                packagesWithRating.Add(packageWithRating);
            }

            /*return Json(_packageRepo.Query(p => p.IsActive == true && p.Location.Contains(location)));*/
            /*return Json(_packageRepo.Query(p => p.Location.Contains(location)));*/
            return Json(packagesWithRating);
        }
    }
}
