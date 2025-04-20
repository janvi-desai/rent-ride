using CarRentalSystem.Data;
using CarRentalSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllAsync() =>
            await _context.Payments.Include(p => p.Rental).ThenInclude(r => r.User).ToListAsync();

        public async Task<List<Payment>> GetByUserIdAsync(string userId) =>
            await _context.Payments.Include(p => p.Rental)
                .Where(p => p.Rental.UserId == userId).ToListAsync();

        public async Task<Payment?> GetByIdAsync(int id) =>
            await _context.Payments.Include(p => p.Rental).FirstOrDefaultAsync(p => p.Id == id);
        
        public async Task<Payment?> GetByRentalIdAsync(int id) =>
            await _context.Payments.Include(p => p.Rental).FirstOrDefaultAsync(p => p.RentalId == id);

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }
    }

}
