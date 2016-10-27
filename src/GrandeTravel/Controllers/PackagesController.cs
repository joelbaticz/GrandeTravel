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
        private IProviderRepository _providerRepo;

        private UserManager<ApplicationUser> _userManagerService;
        private RoleManager<IdentityRole> _roleManagerService;

        /*private DbContextService _dbContext;

        public PackagesController(DbContextService dbContext)
        {
            _dbContext = dbContext;
        }
        */

        // SignInManager<ApplicationUser> signInManagerService, RoleManager<IdentityRole> roleManagerService, IProviderRepository providerRepo, ICustomerRepository customerRepo
        public PackagesController(UserManager<ApplicationUser> userManagerService, RoleManager<IdentityRole> roleManagerService, IProviderRepository providerRepo, IPackageRepository packageRepo)
        {
            _packageRepo = packageRepo;
            _providerRepo = providerRepo;
            _userManagerService = userManagerService;
            _roleManagerService = roleManagerService;


        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                DisplayAllPackagesViewModel vm = new DisplayAllPackagesViewModel
                {
                    Location = null,
                    MinPrice = null,
                    MaxPrice = null,

                    Packages = _packageRepo.GetAll()
                };

                return View(vm);
            }
            else
            {
                string userName = HttpContext.User.Identity.Name;

                ApplicationUser user = await _userManagerService.FindByNameAsync(userName);

                bool result = await _userManagerService.IsInRoleAsync(user, "Provider");

                if (result)
                {
                    //return Content("You are a Provider bro!");
                    Provider specficProvider = _providerRepo.GetSingle(p => p.UserId == user.Id);

                    DisplayAllPackagesViewModel vm = new DisplayAllPackagesViewModel
                    {
                        Location = null,
                        MinPrice = null,
                        MaxPrice = null,

                        Packages = _packageRepo.Query(p => p.ProviderId == specficProvider.ProviderId)
                    };

                    return View(vm);
                }

                result = await _userManagerService.IsInRoleAsync(user, "Customer");

                if (result)
                {
                    //return Content("You are a Customer bro!");
                    DisplayAllPackagesViewModel vm = new DisplayAllPackagesViewModel
                    {
                        Location = null,
                        MinPrice = null,
                        MaxPrice = null,

                        Packages = _packageRepo.GetAll()
                    };

                    return View(vm);

                }

                return Content("You are an Unknow User bro!");


                //var result = await _roleManagerService.us
                //Provider provider = _providerRepo.GetSingle(p => p.UserId == user.Id);

                
            }
        }

        [HttpPost]
        public IActionResult Index(DisplayAllPackagesViewModel vm)
        {
            if (ModelState.IsValid)
            {

                vm.Packages = _packageRepo.GetAll();

                

                if (vm.Location!= null)
                {
                    string loc = vm.Location.ToLower();
                    vm.Packages = vm.Packages.Where(p => p.Location.ToLower().Contains(loc));
                }
                if (vm.MinPrice != null)
                {
                    vm.Packages = vm.Packages.Where(p => p.Price >= Double.Parse(vm.MinPrice));
                }
                if (vm.MaxPrice != null)
                {
                    vm.Packages = vm.Packages.Where(p => p.Price <= Double.Parse(vm.MaxPrice));
                }

                /*
                if (vm.Location=="")
                {
                    vm.Packages = _packageRepo.Query(p => (p.Price > vm.MinPrice && p.Price < vm.MaxPrice));
                }
                else
                {
                    vm.Packages = _packageRepo.Query(p => p.Location == vm.Location);
                }
                */


                return View(vm);

            }
            return View(vm);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
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
                    Price = vm.Price
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
                Price = editablePackage.Price
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
                    Price = vm.Price
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
                Name = _packageRepo.GetSingle( p => p.PackageId == id).Name
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



    }
}
