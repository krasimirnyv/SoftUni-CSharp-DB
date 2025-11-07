using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProductShop.DTOs.Import;

public class ImportCategoryDto
{
    [Required] 
    [JsonProperty("name")] 
    public string Name { get; set; } = null!;
}