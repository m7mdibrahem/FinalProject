using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace Airline.Areas.Customer.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            ProfileVM profileVM = new ProfileVM()
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
            };
            return View(profileVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileVM profileVM)
        {
            var user = await userManager.GetUserAsync(User);

            user.UserName = profileVM.UserName;
            user.Name = profileVM.Name;
            user.Email = profileVM.Email;

            await userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> UpdatePhoto(IFormFile ImageUrl)
        {
            var user = await userManager.GetUserAsync(User);

            if (ImageUrl != null && ImageUrl.Length > 0)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);



                //// Save in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {

                    ImageUrl.CopyTo(stream);

                }

                // Save in db
                user.ImageUrl = fileName;
                await userManager.UpdateAsync(user);


            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Password()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Password(EditpasswordVM editpasswordVM)
        {
            var user = await userManager.GetUserAsync(User);

            await userManager.ChangePasswordAsync(user, editpasswordVM.OldPassword, editpasswordVM.NewPassword);

            return RedirectToAction("Index", "Home");
        }
    }
}
