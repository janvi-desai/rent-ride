using AutoMapper;
using CarRentalSystem.Data;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;

namespace CarRentalSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PaymentModel>> GetAllAsync()
        {
            var payments = await _repository.GetAllAsync();
            return _mapper.Map<List<PaymentModel>>(payments);
        }

        public async Task<List<PaymentModel>> GetByUserAsync(string userId)
        {
            var payments = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<List<PaymentModel>>(payments);
        }

        public async Task<PaymentModel?> GetByIdAsync(int id)
        {
            var payment = await _repository.GetByIdAsync(id);
            return payment != null ? _mapper.Map<PaymentModel>(payment) : null;
        }

        public async Task<PaymentModel?> GetByRentalIdAsync(int id)
        {
            var payment = await _repository.GetByRentalIdAsync(id);
            return payment != null ? _mapper.Map<PaymentModel>(payment) : null;
        }

        public async Task AddAsync(PaymentModel model)
        {
            var entity = _mapper.Map<Payment>(model);
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(PaymentModel model)
        {
            var entity = _mapper.Map<Payment>(model);
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
