using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;

namespace Services
{
    public interface IStoreLocationService
    {
        Task<List<StoreLocation>> GetAllStoreLocationsAsync();
        Task<StoreLocation> GetStoreLocationByIdAsync(int id);
    }

    public class StoreLocationService : IStoreLocationService
    {
        private readonly StoreLocationRepository _storeLocationRepository;

        public StoreLocationService(StoreLocationRepository storeLocationRepository)
        {
            _storeLocationRepository = storeLocationRepository;
        }

        public async Task<List<StoreLocation>> GetAllStoreLocationsAsync()
        {
            return await _storeLocationRepository.GetAllAsync();
        }

        public async Task<StoreLocation> GetStoreLocationByIdAsync(int id)
        {
            return await _storeLocationRepository.GetByIdAsync(id);
        }
    }
}
