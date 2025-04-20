using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentModel>> GetAllAsync();
        Task<List<PaymentModel>> GetByUserAsync(string userId);
        Task<PaymentModel?> GetByIdAsync(int id);
        Task<PaymentModel?> GetByRentalIdAsync(int id);
        Task AddAsync(PaymentModel model);
        Task UpdateAsync(PaymentModel model);
        Task DeleteAsync(int id);
    }

}