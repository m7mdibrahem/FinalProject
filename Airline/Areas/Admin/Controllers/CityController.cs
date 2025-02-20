using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles = NameInit.Admin)]
    public class CityController : Controller
    {
        private readonly ICityRepo cityRepo;
        private readonly ICategoryRepo categoryRepo;
        private readonly ICountryRepo countryRepo;
        private readonly ICityCategoryRepo cityCategoryRepo;

        public CityController(ICityRepo cityRepo, ICategoryRepo categoryRepo, ICountryRepo countryRepo, ICityCategoryRepo cityCategoryRepo)
        {
            this.cityRepo = cityRepo;
            this.categoryRepo = categoryRepo;
            this.countryRepo = countryRepo;
            this.cityCategoryRepo = cityCategoryRepo;
        }
        public IActionResult Index()
        {
            var city = cityRepo.Get(includation: [e => e.Country, e => e.CityCategories]).OrderBy(e => e.Name).ToList();
            categoryRepo.Get(includation: [e => e.CityCategories]).ToList();
            return View(model: city);
        }
        public IActionResult Creation()
        {
            CityVM cityVM = new()
            {
                Countries = countryRepo.Get().OrderBy(e => e.Name).ToList(),
                Categories = categoryRepo.Get().OrderBy(e => e.Name).ToList()
            };
            return View(cityVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creation(CityVM cityVM, int countryId, List<int> categoriesId, IFormFile file)
        {
            ModelState.Remove("City.ImgUrl");
            ModelState.Remove("City.Country");
            cityVM.Countries = countryRepo.Get().OrderBy(e => e.Name).ToList();
            cityVM.Categories = categoryRepo.Get().OrderBy(e => e.Name).ToList();
            if (categoriesId.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "categories field is required");
            }
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
                    City city = new()
                    {
                        Name = cityVM.City.Name,
                        ImgUrl = fileName,
                        CountryId = countryId
                    };
                    cityRepo.Create(city);
                    cityRepo.Attemp();
                    //var cityName = cityRepo.GetOne(filter: e => e.Name == cityVM.City.Name, tracked: false);
                    foreach (var item in categoriesId)
                    {
                        cityCategoryRepo.Create(new()
                        {
                            CityId = city.Id,
                            CategoryId = item
                        });
                        cityCategoryRepo.Attemp();
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(cityVM);
        }
        public IActionResult Edition(int id)
        {
            CityVM cityVM = new()
            {
                City = cityRepo.GetOne(filter: e => e.Id == id),
                Countries = countryRepo.Get().OrderBy(e => e.Name).ToList(),
                Categories = categoryRepo.Get().OrderBy(e => e.Name).ToList(),
                CityCategories = cityCategoryRepo.Get(filter: e => e.CityId == id).ToList()
            };
            return View(cityVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edition(CityVM cityVM, int countryId, List<int> categoriesId, IFormFile file)
        {
            cityVM.Countries = countryRepo.Get().OrderBy(e => e.Name).ToList();
            cityVM.Categories = categoryRepo.Get().OrderBy(e => e.Name).ToList();
            ModelState.Remove("file");
            ModelState.Remove("City.ImgUrl");
            ModelState.Remove("City.Country");
            var oldImg = cityRepo.GetOne(filter: e => e.Id == cityVM.City.Id, tracked: false);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldImg.ImgUrl);
            if (ModelState.IsValid)
            {
                City city = new City();
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
                    city.ImgUrl = fileName;
                }
                else
                {
                    city.ImgUrl = oldImg.ImgUrl;
                }
                city.Id = cityVM.City.Id;
                city.Name = cityVM.City.Name;
                city.CountryId = countryId;
                cityRepo.Edit(city);
                cityRepo.Attemp();
                var cityall = cityCategoryRepo.Get(filter: e => e.CityId == cityVM.City.Id, tracked: false).ToList();
                cityCategoryRepo.DeleteAll(cityall);
                cityCategoryRepo.Attemp();
                foreach (var item in categoriesId)
                {
                    cityCategoryRepo.Create(new()
                    {
                        CityId = cityVM.City.Id,
                        CategoryId = item
                    });
                    cityCategoryRepo.Attemp();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cityVM);
        }
        public IActionResult Deletion(int id)
        {
            var city = cityRepo.GetOne(e => e.Id == id);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", city.ImgUrl);
            System.IO.File.Delete(oldPath);
            cityRepo.Delete(city);
            cityRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
    }
}
