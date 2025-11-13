using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import;

[XmlType("Car")]
public class ImportCarDto
{
    [Required]
    [XmlElement("make")]
    public string Make { get; set; } = null!;

    [Required]
    [XmlElement("model")]
    public string Model { get; set; } = null!;

    [Required]
    [XmlElement("traveledDistance")]
    public string TraveledDistance { get; set; } = null!;
    
    [Required]
    [XmlArray("parts")]
    [XmlArrayItem("partId")]
    public ImportPartIdDto[] Parts { get; set; } = null!;
}