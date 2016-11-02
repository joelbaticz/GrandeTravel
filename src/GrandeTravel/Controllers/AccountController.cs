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
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManagerService;
        private SignInManager<ApplicationUser> _signInManagerService;
        private RoleManager<IdentityRole> _roleManagerService;

        private IProviderRepository _providerRepo;
        private ICustomerRepository _customerRepo;

        public AccountController(UserManager<ApplicationUser> userManagerService, SignInManager<ApplicationUser> signInManagerService, RoleManager<IdentityRole> roleManagerService, IProviderRepository providerRepo, ICustomerRepository customerRepo)
        {
            _userManagerService = userManagerService;
            _signInManagerService = signInManagerService;
            _roleManagerService = roleManagerService;
            _providerRepo = providerRepo;
            _customerRepo = customerRepo;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterProvider()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProvider(RegisterProviderViewModel vm)
        {
            //We added validation
            //So that is where isValid comes into action
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = vm.Username,
                    PhoneNumber = vm.Phone,
                    Email = vm.Email
                };

                //save to db
                //you don't want to do it in your main thread
                //this method needs to be an async method
                IdentityResult result = await _userManagerService.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {

                    if (!await _userManagerService.IsInRoleAsync(user, "Provider"))
                    {
                        await _userManagerService.AddToRoleAsync(user, "Provider");
                    }

                    IdentityUser specificUser = await _userManagerService.FindByNameAsync(user.UserName);

                    Provider newProvider = new Provider
                    {
                        UserId = specificUser.Id,
                        DisplayName = vm.DisplayName,
                        ABN = vm.ABN
                    };

                    _providerRepo.Create(newProvider);

                    //return RedirectToAction("Index", "Packages");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        //ModelState.AddModelError("" , "error");
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerViewModel vm)
        {
            //We added validation
            //So that is where isValid comes into action
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = vm.Username,
                    PhoneNumber = vm.Phone,
                    Email = vm.Email
                };

                //save to db
                //you don't want to do it in your main thread
                //this method needs to be an async method
                IdentityResult result = await _userManagerService.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {

                    if (!await _userManagerService.IsInRoleAsync(user, "Customer"))
                    {
                        await _userManagerService.AddToRoleAsync(user, "Customer");
                    }

                    IdentityUser specificUser = await _userManagerService.FindByNameAsync(user.UserName);

                    Customer newCustomer = new Customer
                    {
                        UserId = specificUser.Id,
                        FirstName = vm.FirstName,
                        LastName = vm.LastName
                    };

                    _customerRepo.Create(newCustomer);

                    //return RedirectToAction("Index", "Packages");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        //ModelState.AddModelError("" , "error");
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return View(vm);
        }


        //ReturnUrl is returning the user to page which required authentication
        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Packages");
            }

            LoginViewModel vm = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //Sign in
                //Persistent user = rememberMe, Lock account = false
                var result = await _signInManagerService.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, false);

                if (result.Succeeded)
                {
                    //If retunrUrl empty is going to create a 404
                    //Only redirect if the returnUrl is local to my app
                    //Because returnUrl is a query string which can be hijacked
                    //ViewData.Add("DudeName", "Lajos");

                    /*
                        string userName = HttpContext.User.Identity.Name;

                        ApplicationUser user = await _userManagerService.FindByNameAsync(userName);

                        Provider provider = _providerRepo.GetSingle(p => p.UserId == user.Id);
                    
                    
                    ViewData["DisplayName"] = "Lajcsi";
                    */



                    if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Packages");

                        /*
                        ApplicationUser user = await _userManagerService.FindByNameAsync(vm.Username);
                        IEnumerable<string> roles = await _userManagerService.GetRolesAsync(user);

                        if (roles.Contains("Provider"))
                            return RedirectToAction("ProviderSearch", "Packages");
                        //if (HttpContext.User.IsInRole("Customer"))
                        if (roles.Contains("Customer"))
                            return RedirectToAction("Index", "Packages");

                        return Content("Must be logged in!");
                        */
                    }
                }
            }
            ModelState.AddModelError("", "Username or Password incorrect");
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManagerService.SignOutAsync();

            return RedirectToAction("Index", "Packages");
        }

        [HttpGet]
        public IActionResult EditAccount()
        {

            bool IsUserAuthenticated = HttpContext.User.Identity.IsAuthenticated;


            

            if (IsUserAuthenticated)
            {
                if (HttpContext.User.IsInRole("Provider"))
                {
                    return RedirectToAction("EditProvider");
                }
                if (HttpContext.User.IsInRole("Customer"))
                {
                    return RedirectToAction("EditCustomer");
                }
                return Content("Unknown Role!");
            }
            return Content("Must be logged in!");

            /*
            string UserName = HttpContext.User.Identity.Name;

            var user = HttpContext.User;


            //var UserID = HttpContext.User.GetUserId();
            var userRoles = db.Roles.Include(r => r.Users).ToList();

            var RolesForUser = await UserManager.GetRolesAsync(UserID);



        /*
        var claimsIdentity = (ClaimsIdentity)this.User.Identity;
        var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        var userId = claim.Value;



        var user = await _userManagerService.GetUserAsync();

        string currentUserId = User.Identity.GetUserId();
        ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

        var userId = await _userManagerService.GetUserId(IUserClaimsPrincipalFactory<);
        _userManagerService.IsInRoleAsync()
*/
            //         return RedirectToAction("Index", "Packages");
        }

        [HttpGet]
        public async Task<IActionResult> EditProvider()
        {
            bool IsUserAuthenticated = HttpContext.User.Identity.IsAuthenticated;

            if (IsUserAuthenticated)
            {
                string userName = HttpContext.User.Identity.Name;

                ApplicationUser user = await _userManagerService.FindByNameAsync(userName);

                Provider provider = _providerRepo.GetSingle(p => p.UserId == user.Id);

                EditProviderViewModel vm = new EditProviderViewModel
                {
                    Username = user.UserName,
                    Phone = user.PhoneNumber,
                    Email = user.Email,
                    DisplayName = provider.DisplayName,
                    ABN = provider.ABN
                };

                return View(vm);
            }
            return Content("Must be logged in!");

        }

        [HttpPost]
        public async Task<IActionResult> EditProvider(EditProviderViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //return Content("So far so good.");
                /*
                var user = new ApplicationUser
                {
                    UserName = vm.Username,
                    PhoneNumber = vm.Phone,
                    Email = vm.Email
                };
                */

                //save to db
                //you don't want to do it in your main thread
                //this method needs to be an async method
                ApplicationUser specificUser = await _userManagerService.FindByNameAsync(vm.Username);

                specificUser.PhoneNumber = vm.Phone;
                specificUser.Email = vm.Email;

                IdentityResult result = await _userManagerService.UpdateAsync(specificUser);

                if (result.Succeeded)
                {
                    
                    /*
                    Provider updatedProvier = new Provider
                    {
                        UserId = specificUser.Id,
                        DisplayName = vm.DisplayName,
                        ABN = vm.ABN
                    };
                    */

                    Provider updatedProvider = _providerRepo.GetSingle(p => p.UserId == specificUser.Id);
                    updatedProvider.DisplayName = vm.DisplayName;
                    updatedProvider.ABN = vm.ABN;

                    _providerRepo.Update(updatedProvider);
                    
                    return RedirectToAction("Index", "Packages");
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        //ModelState.AddModelError("" , "error");
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return View(vm);
     
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer()
        {
            bool IsUserAuthenticated = HttpContext.User.Identity.IsAuthenticated;

            if (IsUserAuthenticated)
            {
                string userName = HttpContext.User.Identity.Name;

                ApplicationUser user = await _userManagerService.FindByNameAsync(userName);

                Customer customer = _customerRepo.GetSingle(c => c.UserId == user.Id);

                EditCustomerViewModel vm = new EditCustomerViewModel
                {
                    Username = user.UserName,
                    Phone = user.PhoneNumber,
                    Email = user.Email,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };

                return View(vm);
            }
            return Content("Must be logged in!");

        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(EditCustomerViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser specificUser = await _userManagerService.FindByNameAsync(vm.Username);

                specificUser.PhoneNumber = vm.Phone;
                specificUser.Email = vm.Email;

                IdentityResult result = await _userManagerService.UpdateAsync(specificUser);

                if (result.Succeeded)
                {

                    Customer updatedCustomer = _customerRepo.GetSingle(c => c.UserId == specificUser.Id);
                    updatedCustomer.FirstName = vm.FirstName;
                    updatedCustomer.LastName = vm.LastName;

                    _customerRepo.Update(updatedCustomer);

                    return RedirectToAction("Index", "Packages");
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        //ModelState.AddModelError("" , "error");
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return View(vm);

        }


    }
}


     