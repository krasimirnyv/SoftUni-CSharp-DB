using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CarDealer.DTOs.Import;

public class ImportPartDto
{
    [Required]
    [JsonProperty("name")]
    public string Name { get; set; } = null!; 

    [Required]
    [JsonProperty("price")]
    public string Price { get; set; } = null!;

    [Required]
    [JsonProperty("quantity")]
    public string Quantity { get; set; } = null!;

    [Required]
    [JsonProperty("supplierId")]
    public string SupplierId { get; set; } = null!;
}