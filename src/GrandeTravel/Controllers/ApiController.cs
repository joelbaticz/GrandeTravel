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

        public ApiController(IPackageRepository packageRepo)
        {
            _packageRepo = packageRepo;
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
    }
}
