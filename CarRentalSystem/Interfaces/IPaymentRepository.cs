using CarRentalSystem.Data;
using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetByUserIdAsync(string userId);
        Task<Payment?> GetByRentalIdAsync(int id);
        Task<Payment?> GetByIdAsync(int id);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(int id);
    }

}
