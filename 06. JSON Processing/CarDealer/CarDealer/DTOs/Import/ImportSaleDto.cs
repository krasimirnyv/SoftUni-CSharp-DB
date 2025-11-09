using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CarDealer.DTOs.Import;

public class ImportSaleDto
{
    [Required]
    [JsonProperty("discount")]
    public string Discount { get; set; } = null!;

    [Required]
    [JsonProperty("carId")]
    public string CarId { get; set; } = null!;

    [Required]
    [JsonProperty("customerId")]
    public string CustomerId { get; set; } = null!;
}