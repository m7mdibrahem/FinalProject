using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles = NameInit.Admin)]
    public class TripController : Controller
    {
        private readonly ITripRepo tripRepo;
        private readonly ICompanyRepo companyRepo;
        private readonly ICityRepo cityRepo;
        private readonly ICountryRepo countryRepo;
        private readonly ISeatRepo seatRepo;
        private readonly ITicketRepo ticketRepo;

        public TripController(ITripRepo tripRepo, ICompanyRepo companyRepo, ICityRepo cityRepo, ICountryRepo countryRepo, ISeatRepo seatRepo, ITicketRepo ticketRepo)
        {
            this.tripRepo = tripRepo;
            this.companyRepo = companyRepo;
            this.cityRepo = cityRepo;
            this.countryRepo = countryRepo;
            this.seatRepo = seatRepo;
            this.ticketRepo = ticketRepo;
        }
        public IActionResult Index()
        {
            var trip = tripRepo.Get(includation: [e => e.Company]).ToList();
            companyRepo.Get().OrderBy(e => e.Name).ToList();
            return View(model: trip);
        }
        public IActionResult Creation()
        {
            var cities = cityRepo.Get(includation: [e => e.Country]).ToList();
            countryRepo.Get().OrderBy(e => e.Name).ToList();
            TripVM tripVM = new()
            {
                Cities = cities,
                Companies = companyRepo.Get().OrderBy(e => e.Name).ToList()
            };
            return View(tripVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creation(TripVM tripVM, int categId)
        {
            ModelState.Remove("Trip.ReturnDate");
            var cities = cityRepo.Get(includation: [e => e.Country]).ToList();
            countryRepo.Get().OrderBy(e => e.Name).ToList();
            tripVM.Cities = cities;
            tripVM.Companies = companyRepo.Get().OrderBy(e => e.Name).ToList();
            if (tripVM.Trip.From == tripVM.Trip.To)
            {
                ModelState.AddModelError(string.Empty, "from and to have the same inputs");
            }
            if (tripVM.Trip.DepartDate < DateOnly.FromDateTime(DateTime.Now) || tripVM.Trip.DepartDate == null)
            {
                ModelState.AddModelError("Trip.DepartDate", "this time is expired");
            }
            if (tripVM.Trip.ReturnDate != null)
            {
                if (tripVM.Trip.ReturnDate < tripVM.Trip.DepartDate)
                {
                    ModelState.AddModelError("Trip.ReturnDate", "return date greater than depart date");
                }
            }
            if (ModelState.IsValid)
            {
                Trip trip = new()
                {
                    CompanyId = categId,
                    From = tripVM.Trip.From,
                    To = tripVM.Trip.To,
                    Price = tripVM.Trip.Price,
                    Miles = tripVM.Trip.Miles,
                    DepartDate = tripVM.Trip.DepartDate,
                    ReturnDate = tripVM.Trip.ReturnDate,
                    First = tripVM.Trip.First,
                    Business = tripVM.Trip.Business,
                    Premium = tripVM.Trip.Premium,
                    Economy = tripVM.Trip.Economy
                };
                tripRepo.Create(trip);
                tripRepo.Attemp();

                int number = 0;

                if (tripVM.Trip.First > 0)
                {
                    for (int i = 0; i < tripVM.Trip.First; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.FirstClass,
                            Price = (tripVM.Trip.Price * (20.0 / 100.0)) + tripVM.Trip.Price,
                            TripId = trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                if (tripVM.Trip.Business > 0)
                {
                    for (int i = 0; i < tripVM.Trip.Business; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.BusinessClass,
                            Price = (tripVM.Trip.Price * (15.0 / 100.0)) + tripVM.Trip.Price,
                            TripId = trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                if (tripVM.Trip.Premium > 0)
                {
                    for (int i = 0; i < tripVM.Trip.Premium; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.PremiumEconomy,
                            Price = (tripVM.Trip.Price * (10.0 / 100.0)) + tripVM.Trip.Price,
                            TripId = trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                if (tripVM.Trip.Economy > 0)
                {
                    for (int i = 0; i < tripVM.Trip.Economy; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.Economy,
                            Price = tripVM.Trip.Price,
                            TripId = trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tripVM);
        }
        public IActionResult Edition(int id)
        {
            ModelState.Remove("Trip.ReturnDate");
            var trip = tripRepo.GetOne(filter: e => e.Id == id);
            var cities = cityRepo.Get(includation: [e => e.Country]).ToList();
            countryRepo.Get().OrderBy(e => e.Name).ToList();
            var companies = companyRepo.Get().OrderBy(e => e.Name).ToList();
            TripVM tripVM = new()
            {
                Trip = trip,
                Cities = cities,
                Companies = companies,
            };
            return View(tripVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edition(TripVM tripVM, int categId)
        {
            tripVM.Cities = cityRepo.Get(includation: [e => e.Country]).ToList();
            countryRepo.Get().OrderBy(e => e.Name).ToList();
            tripVM.Companies = companyRepo.Get().OrderBy(e => e.Name).ToList();

            if (tripVM.Trip.From == tripVM.Trip.To)
            {
                ModelState.AddModelError(string.Empty, "from and to have the same inputs");
            }
            if (tripVM.Trip.DepartDate < DateOnly.FromDateTime(DateTime.Now) || tripVM.Trip.DepartDate == null)
            {
                ModelState.AddModelError("Trip.DepartDate", "this time is expired");
            }
            if (tripVM.Trip.ReturnDate != null)
            {
                if (tripVM.Trip.ReturnDate < tripVM.Trip.DepartDate)
                {
                    ModelState.AddModelError("Trip.ReturnDate", "return date greater than depart date");
                }
            }
            if (ModelState.IsValid)
            {
                tripRepo.Edit(tripVM.Trip);
                tripRepo.Attemp();

                var seats = seatRepo.Get(filter: e => e.TripId == tripVM.Trip.Id).ToList();
                seatRepo.DeleteAll(seats);
                seatRepo.Attemp();
                int number = 0;
                if (tripVM.Trip.First > 0)
                {
                    for (int i = 0; i < tripVM.Trip.First; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.FirstClass,
                            Price = (tripVM.Trip.Price * (20.0 / 100.0)) + tripVM.Trip.Price,
                            TripId = tripVM.Trip.Id,
                            Number=number
                        });
                        seatRepo.Attemp();
                    }
                }
                if (tripVM.Trip.Business > 0)
                {
                    for (int i = 0; i < tripVM.Trip.Business; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.BusinessClass,
                            Price = (tripVM.Trip.Price * (15.0 / 100.0)) + tripVM.Trip.Price,
                            TripId = tripVM.Trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                if (tripVM.Trip.Premium > 0)
                {
                    for (int i = 0; i < tripVM.Trip.Premium; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.PremiumEconomy,
                            Price = (tripVM.Trip.Price * (10.0 / 100.0)) + tripVM.Trip.Price,
                            TripId = tripVM.Trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                if (tripVM.Trip.Economy > 0)
                {
                    for (int i = 0; i < tripVM.Trip.Economy; i++)
                    {
                        number += 1;
                        seatRepo.Create(new()
                        {
                            Degree = TripType.Economy,
                            Price = tripVM.Trip.Price,
                            TripId = tripVM.Trip.Id,
                            Number = number
                        });
                        seatRepo.Attemp();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tripVM);
        }
        public IActionResult Deletion(int id)
        {
            var trip = tripRepo.GetOne(e => e.Id == id);
            tripRepo.Delete(trip);
            tripRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Cancellation(int id)
        {
            var trip = tripRepo.GetOne(filter: e => e.Id == id);
            trip.Status = tripStatus.Cancelled;
            tripRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
    }
}
