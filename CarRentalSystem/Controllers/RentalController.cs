// RentalController.cs
using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class RentalController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IRentalService _rentalService;
        private readonly IAuthService _authService;

        public RentalController(IRentalService rentalService, IAuthService authService, IPaymentService paymentService)
        {
            _rentalService = rentalService;
            _authService = authService;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("Admin"))
            {
                return Unauthorized("Access Denied: Admin role required.");
            }

            // Get all rentals
            var rentals = await _rentalService.GetAllRentalsAsync();

            // Get all payments in one call
            var payments = await _paymentService.GetAllAsync();

            // Create a dictionary to quickly lookup payment status by rental Id
            var rentalPayments = payments.ToDictionary(p => p.RentalId, p => p);

            // Create rental models and add payment status
            var rentalModels = rentals.Select(r => new RentalModel
            {
                Id = r.Id,
                CarId = r.CarId,
                UserId = r.UserId,
                RentDate = r.RentDate,
                ReturnDate = r.ReturnDate,
                PickupLocation = r.PickupLocation,
                DropLocation = r.DropLocation,
                Status = r.Status,
                TotalAmount = r.TotalAmount,
                // Check if payment exists for the rental
                IsPaid = rentalPayments.ContainsKey(r.Id)
            }).ToList();

            return View(rentalModels);
        }


        public async Task<IActionResult> MyRentals(string userId)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            var user = await _authService.GetUserByEmailAsync(HttpContext.Session.GetString("UserEmail") ?? "");
            if (userId == null)
            {
                userId = $"{user?.Id}";
            }

            // Get all rentals for the user
            var rentals = await _rentalService.GetAllRentalsAsync();

            // Get payments for all rentals that belong to the user (in one call)
            var payments = await _paymentService.GetByUserAsync(user?.Id);

            // Create a dictionary to quickly lookup payment status by rental Id
            var rentalPayments = payments.ToDictionary(p => p.RentalId, p => p);

            // Create rental models and add payment status
            var rentalModels = rentals.Select(r => new RentalModel
            {
                Id = r.Id,
                CarId = r.CarId,
                UserId = r.UserId,
                RentDate = r.RentDate,
                ReturnDate = r.ReturnDate,
                PickupLocation = r.PickupLocation,
                DropLocation = r.DropLocation,
                Status = r.Status,
                TotalAmount = r.TotalAmount,
                // Check if payment exists for the rental
                IsPaid = rentalPayments.ContainsKey(r.Id)
            }).ToList();

            return View("MyRentals", rentalModels.Where(r => r.UserId == userId));
        }


        public IActionResult Create(int carId)
        {
            var user = _authService.GetUserByEmailAsync(HttpContext.Session.GetString("UserEmail") ?? "");

            return View(new RentalModel { CarId = carId, RentDate = DateTime.Today, ReturnDate = DateTime.Today.AddDays(1), UserId = $"{user?.Result?.Id}" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentalModel model)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            if (!ModelState.IsValid)
                return View(model);

            await _rentalService.CreateRentalAsync(model);
            return RedirectToAction("MyRentals", new { userId = model.UserId });
        }

        public async Task<IActionResult> Return(int id)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null) return NotFound();
            return View(rental);
        }

        [HttpPost]
        public async Task<IActionResult> Return(RentalModel model)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            model = await _rentalService.GetRentalByIdAsync(model.Id);
            model.Status = "Returned";
            await _rentalService.UpdateRentalAsync(model);
            return RedirectToAction("MyRentals", new { userId = model.UserId });
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }
            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null) return NotFound();
            return View(rental);
        }

        [HttpPost]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }
            await _rentalService.DeleteRentalAsync(id);
            return RedirectToAction("MyRentals", new { userId = User.Identity.Name });
        }
    }
}
