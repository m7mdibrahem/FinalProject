using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles = NameInit.Admin)]
    public class SeatController : Controller
    {
        private readonly ISeatRepo seatRepo;
        private readonly ITripRepo tripRepo;
        private readonly ICompanyRepo companyRepo;

        public SeatController(ISeatRepo seatRepo, ITripRepo tripRepo, ICompanyRepo companyRepo)
        {
            this.seatRepo = seatRepo;
            this.tripRepo = tripRepo;
            this.companyRepo = companyRepo;
        }

        public IActionResult Index()
        {
            var seat = seatRepo.Get(includation: [e => e.Trip]).ToList();
            tripRepo.Get(includation: [e => e.Company]).ToList();
            companyRepo.Get().OrderBy(e => e.Name).ToList();
            return View(model: seat);
        }
    }
}
