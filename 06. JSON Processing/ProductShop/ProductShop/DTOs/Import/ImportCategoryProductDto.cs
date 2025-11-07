using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Import;

public class ImportCategoryProductDto
{
    [Required]
    [JsonProperty("CategoryId")]
    public string CategoryId { get; set; } = null!;

    [Required]
    [JsonProperty("ProductId")]
    public string ProductId { get; set; } = null!;
}