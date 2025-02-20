using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Areas.Customer.Controllers
{
    public class myCityController : Controller
    {
        private readonly ICityRepo cityRepo;
        private readonly ICategoryRepo categoryRepo;

        public myCityController(ICityRepo cityRepo , ICategoryRepo categoryRepo)
        {
            this.cityRepo = cityRepo;
            this.categoryRepo = categoryRepo;
        }
        public IActionResult Index()
        {
            var cities = cityRepo.Get(includation: [e => e.Country, e => e.CityCategories]).ToList();
            categoryRepo.Get(includation: [e => e.CityCategories]).ToList();
            return View(cities);
        }
    }
}
