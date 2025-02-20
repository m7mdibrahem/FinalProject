using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Airline.Areas.Customer.Controllers
{
    public class SearchTripController : Controller
    {
        private readonly ITripRepo tripRepo;
        private readonly ICompanyRepo companyRepo;
        private readonly ICityRepo cityRepo;
        private readonly ICountryRepo countryRepo;
        private readonly ICategoryRepo categoryRepo;
        private readonly ICityCategoryRepo cityCategoryRepo;

        public SearchTripController(ITripRepo tripRepo, ICompanyRepo companyRepo, ICityRepo cityRepo, ICountryRepo countryRepo, ICategoryRepo categoryRepo, ICityCategoryRepo cityCategoryRepo)
        {
            this.tripRepo = tripRepo;
            this.companyRepo = companyRepo;
            this.cityRepo = cityRepo;
            this.countryRepo = countryRepo;
            this.categoryRepo = categoryRepo;
            this.cityCategoryRepo = cityCategoryRepo;
        }
        public IActionResult Index(string From, string To)
        {
            var trips = tripRepo.Get(filter: e => e.From == From && e.To == To && e.Status == tripStatus.Pending, includation: [e => e.Seats, e => e.Company]).ToList();
            return View(model: trips);
        }
    }
}
