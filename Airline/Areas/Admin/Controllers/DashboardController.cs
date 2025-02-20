using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles = (NameInit.Admin))]
    public class DashboardController : Controller
    {
        private readonly ITripRepo tripRepo;
        private readonly ICityRepo cityRepo;
        private readonly IPaymentRepo paymentRepo;
        private readonly IApplicationUserRepo applicationUserRepo;

        public DashboardController(ITripRepo tripRepo, ICityRepo cityRepo, IPaymentRepo paymentRepo, IApplicationUserRepo applicationUserRepo)
        {
            this.tripRepo = tripRepo;
            this.cityRepo = cityRepo;
            this.paymentRepo = paymentRepo;
            this.applicationUserRepo = applicationUserRepo;
        }
        public IActionResult Index()
        {
            var paymentComp = paymentRepo.Get(filter: e => e.Status == PayStatus.Completed).ToList();
            var flightsDone = tripRepo.Get(filter: e => e.Status == tripStatus.Done).ToList();
            var flightsCancel = tripRepo.Get(filter: e => e.Status == tripStatus.Cancelled).ToList();
            var user = applicationUserRepo.Get().OrderByDescending(e => e.Miles).Skip(0).Take(3).ToList();
            var lastPay = paymentRepo.Get(includation: [e => e.ApplicationUser, e => e.Trip]).OrderByDescending(e => e.Datetime).Skip(0).Take(5).ToList();
            var cities = cityRepo.Get(filter: e => e.Visits > 0).OrderByDescending(e => e.Visits).ToList();
            var citiestot = cityRepo.Get().ToList();
            List<double> Percent = new List<double>();
            for (int i = 0; i < cities.Count; i++)
            {
                double percent = ((double)cities[i].Visits / paymentComp.Count) * 100;
                Percent.Add(percent);
            }
            DashVM dashVM = new()
            {
                Booked = paymentComp.Count,
                Done = flightsDone.Count,
                Cancel = flightsCancel.Count,
                TotalPrice = paymentComp.Sum(e => e.price),
                Payments = lastPay,
                ApplicationUsers = user,
                Cities = cities,
                Percentages = Percent
            };
            return View(dashVM);
        }
    }
}
