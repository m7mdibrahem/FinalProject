using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles =NameInit.Admin)]
    public class CountryController : Controller
    {
        private readonly ICountryRepo countryRepo;

        public CountryController(ICountryRepo countryRepo)
        {
            this.countryRepo = countryRepo;
        }
        public IActionResult Index()
        {
            var country = countryRepo.Get(includation: [e => e.Cities]).OrderBy(e=>e.Name).ToList();
            return View(model: country);
        }
        public IActionResult Creation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creation(Country country, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    country.ImgUrl = fileName;
                    countryRepo.Create(country);
                    countryRepo.Attemp();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(country);
        }
        public IActionResult Edition(int id)
        {
            var country = countryRepo.GetOne(filter: e => e.Id == id);
            return View(model: country);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edition(Country country, IFormFile file)
        {
            ModelState.Remove("file");
            var oldImg = countryRepo.GetOne(filter: e => e.Id == country.Id, tracked: false);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldImg.ImgUrl);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    country.ImgUrl = fileName;
                }
                else
                {
                    country.ImgUrl = oldImg.ImgUrl;
                }
                countryRepo.Edit(country);
                countryRepo.Attemp();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }
        public IActionResult Deletion(int id)
        {
            var country = countryRepo.GetOne(e => e.Id == id);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", country.ImgUrl);
            System.IO.File.Delete(oldPath);
            countryRepo.Delete(country);
            countryRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
    }
}
