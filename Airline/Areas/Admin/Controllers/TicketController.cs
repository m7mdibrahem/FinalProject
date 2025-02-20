using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles = NameInit.Admin)]
    public class TicketController : Controller
    {
        private readonly ITicketRepo ticketRepo;
        private readonly ITripRepo tripRepo;
        private readonly ICompanyRepo companyRepo;
        private readonly IApplicationUserRepo applicationUserRepo;

        public TicketController(ITicketRepo ticketRepo, ITripRepo tripRepo, ICompanyRepo companyRepo,IApplicationUserRepo applicationUserRepo)
        {
            this.ticketRepo = ticketRepo;
            this.tripRepo = tripRepo;
            this.companyRepo = companyRepo;
            this.applicationUserRepo = applicationUserRepo;
        }
        public IActionResult Index()
        {
            var ticket = ticketRepo.Get(includation: [e => e.ApplicationUser, e => e.Trip]).ToList();
            applicationUserRepo.Get().OrderBy(e => e.Name).ToList();
            tripRepo.Get(includation: [e => e.Company]).ToList();
            companyRepo.Get().OrderBy(e => e.Name).ToList();
            return View(model: ticket);
        }
    }
}
