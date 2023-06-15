using System.ComponentModel.DataAnnotations;

namespace Zadanie5.DTOs
{
    public class ProductDTO
    {
        [Required]
        public int IdProduct { get; set; }
        [Required]
        public int IdWarehouse { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public ProductDTO() 
        {
            CreatedAt = DateTime.Now;
        }
    }
}
