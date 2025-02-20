using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Stripe;
using Stripe.Checkout;

namespace Airline.Areas.Customer.Controllers
{
    public class SeatbookController : Controller
    {
        private readonly ISeatRepo seatRepo;
        private readonly ITripRepo tripRepo;
        private readonly ICompanyRepo companyRepo;
        private readonly ITicketRepo ticketRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public SeatbookController(ISeatRepo seatRepo, ITripRepo tripRepo, ICompanyRepo companyRepo, ITicketRepo ticketRepo, UserManager<ApplicationUser> userManager)
        {
            this.seatRepo = seatRepo;
            this.tripRepo = tripRepo;
            this.companyRepo = companyRepo;
            this.ticketRepo = ticketRepo;
            this.userManager = userManager;
        }
        public IActionResult Index(int id)
        {
            var trip = tripRepo.GetOne(filter: e => e.Id == id, includation: [e => e.Seats]);
            return View(model: trip);
        }
        public IActionResult Book(int tripId, List<int> Fseat, List<int> Bseat, List<int> Pseat, List<int> Eseat)
        {
            if (Fseat.Count <= 0 && Bseat.Count <= 0 && Pseat.Count <= 0 && Eseat.Count <= 0)
            {
                return RedirectToAction(nameof(Index), new { id = tripId });
            }
            var trip = tripRepo.GetOne(filter: e => e.Id == tripId);
            Ticket ticket = new()
            {
                TripId = tripId,
                ApplicationUserId = userManager.GetUserId(User)
            };
            ticketRepo.Create(ticket);
            ticketRepo.Attemp();

            List<int> Des = new List<int>();

            if (Fseat.Count > 0)
            {
                foreach (var item in Fseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    Des.Add(seat.Number);
                    ticket.First += 1;
                    ticket.price += seat.Price;
                    ticketRepo.Attemp();
                }
            }

            if (Bseat.Count > 0)
            {
                foreach (var item in Bseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    Des.Add(seat.Number);
                    ticket.Business += 1;
                    ticket.price += seat.Price;
                    ticketRepo.Attemp();
                }
            }
            if (Pseat.Count > 0)
            {
                foreach (var item in Pseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    Des.Add(seat.Number);
                    ticket.Premium += 1;
                    ticket.price += seat.Price;
                    ticketRepo.Attemp();
                }
            }
            if (Eseat.Count > 0)
            {
                foreach (var item in Eseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    Des.Add(seat.Number);
                    ticket.Economy += 1;
                    ticket.price += seat.Price;
                    ticketRepo.Attemp();
                }
            }
            ticket.Description = string.Join(',', Des);
            ticketRepo.Attemp();
            Response.Cookies.Append("TicketId", $"{ticket.Id.ToString()}");
            Response.Cookies.Append("Fseat", $"{string.Join(",", Fseat)}");
            Response.Cookies.Append("Bseat", $"{string.Join(",", Bseat)}");
            Response.Cookies.Append("Pseat", $"{string.Join(",", Pseat)}");
            Response.Cookies.Append("Eseat", $"{string.Join(",", Eseat)}");
            return RedirectToAction(nameof(Pay));
        }
        public async Task<IActionResult> Pay()
        {
            var req = Request.Cookies["TicketId"];
            int ticketId = int.Parse(req);
            var ticket = ticketRepo.GetOne(filter: e => e.Id == ticketId, includation: [e => e.Trip]);

            var service = new SessionService();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"From:{ticket.Trip.From} to:{ticket.Trip.To}",
                            Description=$"Seats booked: {ticket.Description}"
                        },
                        UnitAmount =(long) ticket.price*100,
                    },
                    Quantity =1,
                },
            },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success/?payment_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel/?payment_id={{CHECKOUT_SESSION_ID}}",
                CustomerEmail = userManager.GetUserAsync(User).Result.Email
            };

            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
