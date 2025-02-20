using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Areas.Customer.Controllers
{
    public class mycategoryController : Controller
    {
        private readonly ICategoryRepo categoryRepo;
        private readonly ICityRepo cityRepo;

        public mycategoryController(ICategoryRepo categoryRepo , ICityRepo cityRepo)
        {
            this.categoryRepo = categoryRepo;
            this.cityRepo = cityRepo;
        }
        public IActionResult Index()
        {
            var categories = categoryRepo.Get().ToList();

            return View(categories);
        }

        public IActionResult CategoryContain(int id)
        {
            // Fetch the category with its associated CityCategories and Cities
            var category = categoryRepo.GetOne(filter: e => e.Id == id, includation: [e => e.CityCategories]);

            // Extract the cities from the CityCategories
            var cities = cityRepo.Get(includation: [e => e.CityCategories, e => e.Country]).ToList();

            return View(category);
        }
    }
}
