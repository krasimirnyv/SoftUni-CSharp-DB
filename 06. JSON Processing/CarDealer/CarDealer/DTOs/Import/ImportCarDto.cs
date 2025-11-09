using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CarDealer.DTOs.Import;

public class ImportCarDto
{
    [Required]
    [JsonProperty("make")]
    public string Make { get; set; } = null!;

    [Required]
    [JsonProperty("model")]
    public string Model { get; set; } = null!;

    [Required]
    [JsonProperty("traveledDistance")]
    public string TraveledDistance { get; set; } = null!;
    
    [Required]
    [JsonProperty("partsId")]
    public string[] PartsIds { get; set; } = null!;
}