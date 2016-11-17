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
        private IEmailService _emailService;
        private ISmsService _smsService;

        public HomeController(UserManager<ApplicationUser> userManagerService, RoleManager<IdentityRole> roleManagerService, IEmailService emailService, ISmsService smsService, IProviderRepository providerRepo, ICustomerRepository customerRepo, IPackageRepository packageRepo, IFeedbackRepository feedbackRepo)
        {
            _userManagerService = userManagerService;
            _roleManagerService = roleManagerService;
            _providerRepo = providerRepo;
            _customerRepo = customerRepo;
            _packageRepo = packageRepo;
            _feedbackRepo = feedbackRepo;
            _emailService = emailService;
            _smsService = smsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //IEnumerable<Package> allPackages = _packageRepo.Query(p => p.IsActive == true).Take(5);
            IEnumerable<Package> allPackages = _packageRepo.Query(p => p.IsActive == true);

            List<PackageWithRating> packagesWithRating = new List<PackageWithRating>();

            if ((allPackages != null) && (allPackages.Count()>0))
            {

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

            }

                       


            HomeIndexViewModel vm = new HomeIndexViewModel
            {
                Destination = "",
                //5 best performing packages

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
        public async Task<IActionResult> Contact(ContactViewModel vm)
        {
            if (ModelState.IsValid)
            {

                //string result = await _emailService.SendEmailAsync("joel.baticz@gmail.com", "subjtest", "test");

                string emailContent = "Dear " + vm.Name + ",\n\nThank you for contacting Grande Travel.\n" +
                                         "We will do our best to come back to you as soon as possible.\n\n" +
                                         "Kind Regards,\nGrande Travel";

                //EmailService es = new EmailService();
                //await es.SendEmailAsync(vm.Email, "Grande Travel - Do not reply", contactResponse);

                await _emailService.SendEmailAsync(vm.Email, "Grande Travel - Do not reply", emailContent);


                //SmsService smsSer = new SmsService();
                //await smsSer.SendSmsAsync("+61451054465", "Hi Joel!");
                                                 

                //return Content(result);

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



        [HttpGet]
        public async Task<IActionResult> SetDefaults()
        {
            //----------=========CREATE ROLES===========-------------
            IdentityRole providerRole = new IdentityRole
            {
                Id = "1",
                Name = "Provider",
                NormalizedName = "PROVIDER"
            };

            await _roleManagerService.CreateAsync(providerRole);

            IdentityRole customerRole = new IdentityRole
            {
                Id = "1",
                Name = "Customer",
                NormalizedName = "CUSTROMER"
            };

            await _roleManagerService.CreateAsync(customerRole);


            ApplicationUser provider1 = await _userManagerService.FindByNameAsync("provider1");

            await _userManagerService.AddToRoleAsync(provider1, "Provider");



            return Content("Roles inserted successfully.");

        }






    }
}
