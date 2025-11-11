using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CarDealer.DTOs.Import;

public class ImportSupplierDto
{
    [Required]
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [Required]
    [JsonProperty("isImporter")]
    public string IsImporter { get; set; } = null!;
}