using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Net.Sockets;
using Utility;

namespace Airline.Areas.Customer.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ITicketRepo ticketRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPaymentRepo paymentRepo;
        private readonly ISeatRepo seatRepo;
        private readonly ICityRepo cityRepo;
        private readonly ITripRepo tripRepo;
        private readonly IApplicationUserRepo applicationUserRepo;
        private readonly IEmailSender emailSender;

        public CheckoutController(ITicketRepo ticketRepo, UserManager<ApplicationUser> userManager, IPaymentRepo paymentRepo, ISeatRepo seatRepo, ICityRepo cityRepo, ITripRepo tripRepo, IApplicationUserRepo applicationUserRepo, IEmailSender emailSender)
        {
            this.ticketRepo = ticketRepo;
            this.userManager = userManager;
            this.paymentRepo = paymentRepo;
            this.seatRepo = seatRepo;
            this.cityRepo = cityRepo;
            this.tripRepo = tripRepo;
            this.applicationUserRepo = applicationUserRepo;
            this.emailSender = emailSender;
        }
        public async Task<IActionResult> Success(string payment_id)
        {
            var req = Request.Cookies["TicketId"];
            int ticketId = int.Parse(req);
            var Freq = Request.Cookies["Fseat"];
            var Breq = Request.Cookies["Bseat"];
            var Preq = Request.Cookies["Pseat"];
            var Ereq = Request.Cookies["Eseat"];
            List<int> Fseat = new List<int>();
            List<int> Bseat = new List<int>();
            List<int> Pseat = new List<int>();
            List<int> Eseat = new List<int>();
            if (Freq != null)
            {
                Fseat = Freq.Split(',').Select(int.Parse).ToList();
            }
            if (Breq != null)
            {
                Bseat = Breq.Split(',').Select(int.Parse).ToList();
            }
            if (Preq != null)
            {
                Pseat = Preq.Split(',').Select(int.Parse).ToList();
            }
            if (Ereq != null)
            {
                Eseat = Ereq.Split(',').Select(int.Parse).ToList();
            }
            var ticket = ticketRepo.GetOne(filter: e => e.Id == ticketId, includation: [e => e.Trip, e => e.ApplicationUser]);
            var trip = tripRepo.GetOne(filter: e => e.Id == ticket.TripId);
            var city = cityRepo.GetOne(filter: e => e.Name == trip.To);
            city.Visits += 1;
            cityRepo.Attemp();
            var user = applicationUserRepo.GetOne(filter: e => e.Id == ticket.ApplicationUserId);
            user.Miles += trip.Miles;
            applicationUserRepo.Attemp();

            if (Fseat.Count > 0)
            {
                foreach (var item in Fseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    seat.availability = Availability.Busy;
                    seatRepo.Attemp();
                    trip.First -= 1;
                    tripRepo.Attemp();
                }
            }
            if (Bseat.Count > 0)
            {
                foreach (var item in Bseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    seat.availability = Availability.Busy;
                    seatRepo.Attemp();
                    trip.Business -= 1;
                    tripRepo.Attemp();
                }
            }
            if (Pseat.Count > 0)
            {
                foreach (var item in Pseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    seat.availability = Availability.Busy;
                    seatRepo.Attemp();
                    trip.Premium -= 1;
                    tripRepo.Attemp();
                }
            }
            if (Eseat.Count > 0)
            {
                foreach (var item in Eseat)
                {
                    var seat = seatRepo.GetOne(filter: e => e.Id == item);
                    seat.availability = Availability.Busy;
                    seatRepo.Attemp();
                    trip.Economy -= 1;
                    tripRepo.Attemp();
                }
            }
            Payment payment = new()
            {
                PayId = payment_id,
                ApplicationUserId = ticket.ApplicationUserId,
                TripId = ticket.TripId,
                First = ticket.First,
                Business = ticket.Business,
                Premium = ticket.Premium,
                Economy = ticket.Economy,
                price = ticket.price,
                Status = PayStatus.Completed,
                Description = ticket.Description,
                Datetime = DateTime.Now
            };
            paymentRepo.Create(payment);
            paymentRepo.Attemp();
            //email starts

            var payuser = paymentRepo.GetOne(filter: e => e.Id == payment.Id, includation: [e=>e.ApplicationUser,e=>e.Trip]);
            tripRepo.Get(includation: [e => e.Company]).ToList();
            string message = $@"
             <!DOCTYPE html>
             <html lang='en'>

             <head>
                <meta charset='UTF-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
             </head>

             <body>
                <div style='width:100%;padding:1rem;'>
                    <div style='box-shadow: 0px 0px 3px 0px;padding:1rem 7rem 1rem 1rem;'>
                        <img src='{Request.Scheme}://{Request.Host}/images/logo.png' style='width:7rem' />
                        <p style='padding-top: 1rem;'>Dear, {payuser.ApplicationUser.Name}. Thank you for your payment</p>
                        <p>Ticket Details,</p>
                        <p style='margin:0'>Ticket id: <strong>{payuser.Id}</strong> </p>
                        <p style='margin:0'>Seats No: <strong>{payuser.Description}</strong> </p>
                        <p style='margin:0'>Comapny name: <strong>{payuser.Trip.Company.Name}</strong></p>
                        <p style='margin:0'>From: <strong>{payuser.Trip.From}</strong></p>
                        <p>To: <strong>{payuser.Trip.To}</strong></p>
                        <p>We wish a great trip for you.</p>
                    </div>
                </div>
             </body>

             </html>
             ";
            await emailSender.SendEmailAsync($"{payuser.ApplicationUser.Email}", "success", message);
            //email ends
            return View();
        }
        public IActionResult Cancel(string payment_id)
        {
            var req = Request.Cookies["TicketId"];
            int ticketId = int.Parse(req);
            var ticket = ticketRepo.GetOne(filter: e => e.Id == ticketId, includation: [e => e.Trip, e => e.ApplicationUser]);
            paymentRepo.Create(new()
            {
                PayId = payment_id,
                ApplicationUserId = ticket.ApplicationUserId,
                TripId = ticket.TripId,
                First = ticket.First,
                Business = ticket.Business,
                Premium = ticket.Premium,
                Economy = ticket.Economy,
                price = ticket.price,
                Status = PayStatus.Cancelled,
                Description = ticket.Description,
                Datetime = DateTime.Now
            });
            paymentRepo.Attemp();
            return View();
        }
        public async Task<IActionResult> Test()
        {

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
