using Microsoft.AspNetCore.Mvc;
using Zadanie5.DTOs;
using Zadanie5.Services;

namespace Zadanie5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public Warehouses2Controller(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(ProductDTO product)
        {
            var message = await _warehouseService.AddProductWithProcedure(product);
            return Ok(message);
        }

    }
}
