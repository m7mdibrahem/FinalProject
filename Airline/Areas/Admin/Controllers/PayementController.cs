using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Stripe;
using Stripe.Checkout;

namespace Airline.Areas.Admin.Controllers
{
    public class PayementController : Controller
    {
        private readonly IPaymentRepo paymentRepo;
        private readonly ISeatRepo seatRepo;
        private readonly ITripRepo tripRepo;
        private readonly ICityRepo cityRepo;
        private readonly IApplicationUserRepo applicationUserRepo;

        public PayementController(IPaymentRepo paymentRepo, ISeatRepo seatRepo, ITripRepo tripRepo, ICityRepo cityRepo, IApplicationUserRepo applicationUserRepo)
        {
            this.paymentRepo = paymentRepo;
            this.seatRepo = seatRepo;
            this.tripRepo = tripRepo;
            this.cityRepo = cityRepo;
            this.applicationUserRepo = applicationUserRepo;
        }
        public IActionResult Index(string stat = PayInd.All)
        {
            var payments = paymentRepo.Get(includation: [e => e.ApplicationUser, e => e.Trip]).ToList();
            if (stat == PayInd.Complete)
            {
                payments = paymentRepo.Get(filter: e => e.Status == PayStatus.Completed, includation: [e => e.ApplicationUser, e => e.Trip]).ToList();
            }
            if (stat == PayInd.Cancel)
            {
                payments = paymentRepo.Get(filter: e => e.Status == PayStatus.Cancelled, includation: [e => e.ApplicationUser, e => e.Trip]).ToList();
            }
            if (stat == PayInd.Refund)
            {
                payments = paymentRepo.Get(filter: e => e.Status == PayStatus.Refunded, includation: [e => e.ApplicationUser, e => e.Trip]).ToList();
            }
            PayVM payVM = new()
            {
                Payments = payments,
                Stat = stat
            };
            return View(model: payVM);
        }
        public IActionResult Summary(int id)
        {
            var payment = paymentRepo.GetOne(filter: e => e.Id == id, includation: [e => e.ApplicationUser, e => e.Trip]);

            var seats = seatRepo.Get(filter: e => e.TripId == payment.TripId).ToList();
            List<int> numbers = new List<int>();
            if (payment.Description != null)
            {
                numbers = payment.Description.Split(',').Select(int.Parse).ToList();
            }
            PaySummVM paySummVM = new()
            {
                Payment = payment,
                Numbers = numbers,
                Seats = seats
            };
            return View(model: paySummVM);
        }
        public IActionResult Fund(int id)
        {
            var payment = paymentRepo.GetOne(filter: e => e.Id == id, includation: [e => e.Trip, e => e.ApplicationUser]);
            List<int> numbers = payment.Description.Split(',').Select(int.Parse).ToList();
            var city = cityRepo.GetOne(filter: e => e.Name == payment.Trip.To);
            city.Visits -= 1;
            cityRepo.Attemp();
            var user = applicationUserRepo.GetOne(filter: e => e.Id == payment.ApplicationUserId);
            user.Miles -= payment.Trip.Miles;
            applicationUserRepo.Attemp();
            var seats = seatRepo.Get(filter: e => e.TripId == payment.TripId).ToList();
            foreach (var item in seats)
            {
                foreach (var number in numbers)
                {
                    if (item.Number == number)
                    {
                        item.availability = Availability.Available;
                        seatRepo.Attemp();
                    }
                }
            }
            var trip = tripRepo.GetOne(filter: e => e.Id == payment.TripId);

            if (payment.First > 0)
            {
                for (int i = 0; i < payment.First; i++)
                {
                    trip.First += 1;
                    tripRepo.Attemp();
                }
            }
            if (payment.Business > 0)
            {
                for (int i = 0; i < payment.Business; i++)
                {
                    trip.Business += 1;
                    tripRepo.Attemp();
                }
            }
            if (payment.Premium > 0)
            {
                for (int i = 0; i < payment.Premium; i++)
                {
                    trip.Premium += 1;
                    tripRepo.Attemp();
                }
            }
            if (payment.Economy > 0)
            {
                for (int i = 0; i < payment.Economy; i++)
                {
                    trip.Economy += 1;
                    tripRepo.Attemp();
                }
            }

            var service = new SessionService();
            var session = service.Get(payment.PayId);
            var refundOptions = new RefundCreateOptions()
            {
                PaymentIntent = session.PaymentIntentId,
                Amount = session.AmountTotal,
            };
            var refundService = new RefundService();
            var refund = refundService.Create(refundOptions);
            payment.Status = PayStatus.Refunded;
            paymentRepo.Attemp();
            TempData["Success"] = "Refunded has been completely done";
            return RedirectToAction(nameof(Index));
        }
    }
}
