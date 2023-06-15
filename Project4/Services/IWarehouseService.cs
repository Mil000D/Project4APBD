using Zadanie5.DTOs;

namespace Zadanie5.Services
{
    public interface IWarehouseService
    {
        public Task<string> AddProduct(ProductDTO product);
        public Task<string> AddProductWithProcedure(ProductDTO product);
    }
}
