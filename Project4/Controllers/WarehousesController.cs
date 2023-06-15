using Microsoft.AspNetCore.Mvc;
using Zadanie5.DTOs;
using Zadanie5.Services;
using Zadanie5.Enum;

namespace Zadanie5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(ProductDTO product)
        {
            var message = await _warehouseService.AddProduct(product);
            switch (message)
            {
                case nameof(EnumMessage.PRODUCT_OR_WHOLESALER_NOT_FOUND):
                    return NotFound("The product/wholesaler with the given id does not exist");
                case nameof(EnumMessage.ORDER_NOT_FOUND):
                    return NotFound("There is no corresponding order");
                case nameof(EnumMessage.ORDER_ALREADY_PROCESSED):
                    return BadRequest("The order has already been processed");
                case nameof(EnumMessage.ERROR):
                    return BadRequest("ERROR");
                default:
                    return Ok(message);
            }
        }
    }
}
