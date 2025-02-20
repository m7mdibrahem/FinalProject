using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System.Diagnostics;

namespace Airline.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICityRepo cityRepo;
        private readonly ICountryRepo countryRepo;
        private readonly ICompanyRepo companyRepo;
        private readonly ICategoryRepo categoryRepo;
        private readonly ITripRepo tripRepo;
        private readonly ICityCategoryRepo cityCategoryRepo;

        public HomeController(ILogger<HomeController> logger, ICityRepo cityRepo, ICountryRepo countryRepo, ICompanyRepo companyRepo, ICategoryRepo categoryRepo, ITripRepo tripRepo, ICityCategoryRepo cityCategoryRepo)
        {
            _logger = logger;
            this.cityRepo = cityRepo;
            this.countryRepo = countryRepo;
            this.companyRepo = companyRepo;
            this.categoryRepo = categoryRepo;
            this.tripRepo = tripRepo;
            this.cityCategoryRepo = cityCategoryRepo;
        }

        public IActionResult Index(int? id = 1)
        {
            var cities = cityRepo.Get(includation: [e => e.Country, e => e.CityCategories]).ToList();
            var category = categoryRepo.GetOne(filter: e => e.Id == id, includation: [e => e.CityCategories]);
            cityRepo.Get(includation: [e => e.CityCategories, e => e.Country]).ToList();
            IntroVM introVM = new()
            {
                Cities = cities,
                Companies = companyRepo.Get().Skip(0).Take(8).ToList(),
                Categories = categoryRepo.Get().ToList(),
                Category = category
            };
            return View(model: introVM);
        }
        public IActionResult CategoryCity(int id = 1)
        {
            var category = categoryRepo.GetOne(filter: e => e.Id == id, includation: [e => e.CityCategories]);
            cityRepo.Get(includation: [e => e.CityCategories, e => e.Country]).ToList();
            IntroVM introVM = new()
            {
                Category = category,
                Categories = categoryRepo.Get().ToList(),
            };

            return PartialView("_CategCitPartial", introVM);
        }

        public IActionResult Begin()
        {
            var trips = tripRepo.Get().ToList();
            foreach (var item in trips)
            {
                if (item.DepartDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    item.Status = tripStatus.Done;
                    tripRepo.Attemp();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
