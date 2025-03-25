using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Services;

    namespace BE.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class StoreLocationController : ControllerBase
        {
            private readonly IStoreLocationService _storeLocationService;

            public StoreLocationController(IStoreLocationService storeLocationService)
            {
                _storeLocationService = storeLocationService;
            }

            // Lấy danh sách vị trí cửa hàng
            [HttpGet]
            public async Task<IActionResult> GetStoreLocations()
            {
                var locations = await _storeLocationService.GetAllStoreLocationsAsync();
                return Ok(locations);
            }

            // Lấy vị trí cửa hàng theo ID (nếu cần chi tiết)
            [HttpGet("{id}")]
            public async Task<IActionResult> GetStoreLocationById(int id)
            {
                var location = await _storeLocationService.GetStoreLocationByIdAsync(id);
                if (location == null)
                    return NotFound("Store location not found");
                return Ok(location);
            }
        }
    }
}
