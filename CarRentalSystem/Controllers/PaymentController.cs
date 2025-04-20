using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CarRentalSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;

        public PaymentController(IPaymentService paymentService, UserManager<IdentityUser> userManager, IAuthService authService)
        {
            _paymentService = paymentService;
            _authService = authService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("Admin"))
            {
                return Unauthorized("Access Denied: Admin role required.");
            }

            var isAdmin = roles.Contains("Admin");

            var user = await _authService.GetUserByEmailAsync(HttpContext.Session.GetString("UserEmail") ?? "");
            
            var payments = isAdmin
                ? await _paymentService.GetAllAsync()
                : await _paymentService.GetByUserAsync(user?.Id);

            return View(payments);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("Admin"))
            {
                return Unauthorized("Access Denied: Admin role required.");
            }


            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("Admin"))
            {
                return Unauthorized("Access Denied: Admin role required.");
            }

            await _paymentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create(int rentalId)
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            var paymentModel = new PaymentModel
            {
                RentalId = rentalId,
                PaymentDate = DateTime.Now,
                PaymentMethod = "UPI",
                Status = "Completed"
            };
            return View(paymentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            await _paymentService.AddAsync(model);
            return RedirectToAction("MyPayments");
        }
        
        public async Task<IActionResult> MyPayments()
        {
            var rolesString = HttpContext.Session.GetString("UserRoles");
            var roles = rolesString?.Split(','); // Convert back to a list of roles

            if (roles == null || !roles.Contains("User"))
            {
                return Unauthorized("Access Denied: User role required.");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _authService.GetUserByEmailAsync(userEmail ?? "");
            var payments = await _paymentService.GetByUserAsync(user?.Id);
            return View("MyPayments", payments); // Make sure you have MyPayments.cshtml view
        }

    }
}
