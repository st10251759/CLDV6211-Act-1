using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnackMVCApp.Models
{
    public enum SnackType
    {
        Chocolate,
        Drink,
        Chips,
        ProteinSnack,
        FastFood,
        Candy
    }

    public class Snack
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public SnackType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Stores the Azurite/Azure Blob URL in the database
        public string? ImageUrl { get; set; }

        // NOT saved to DB - only used for the file upload form
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
